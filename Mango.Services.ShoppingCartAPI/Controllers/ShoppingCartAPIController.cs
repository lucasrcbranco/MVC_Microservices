using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dtos;
using Mango.Services.ShoppingCartAPI.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.Integration;
using ShoppingCartAPI.Models.Dtos;

namespace ShoppingCartAPI.Controllers;

[Route("api/shopping-cart")]
[ApiController]
public class ShoppingCartAPIController : ControllerBase
{
    private readonly AppDbContext _dbCtx;
    private readonly IMapper _mapper;
    private readonly ResponseDto _response;
    private readonly IProductService _productService;
    private readonly ICouponService _couponService;
    private readonly IMessageBusService _messageBusService;
    private readonly IConfiguration _configuration;

    public ShoppingCartAPIController(
        AppDbContext dbCtx,
        IMapper mapper,
        IProductService productService,
        ICouponService couponService,
        IMessageBusService messageBusService,
        IConfiguration configuration)
    {
        _dbCtx = dbCtx;
        _mapper = mapper;
        _response = new ResponseDto();
        _productService = productService;
        _couponService = couponService;
        _messageBusService = messageBusService;
        _configuration = configuration;
    }

    [HttpGet("{userId}")]
    public async Task<ResponseDto> Get(string userId)
    {
        try
        {
            var shoppingCartHeader = await _dbCtx.Headers.AsNoTracking().FirstAsync(h => h.UserId == userId);
            var shoppingCartDetails = await _dbCtx.Details.AsNoTracking().Where(h => h.ShoppingCartHeaderId == shoppingCartHeader.ShoppingCartHeaderId).ToListAsync();

            var cartDto = new ShoppingCartDto()
            {
                ShoppingCartHeader = _mapper.Map<ShoppingCartHeaderDto>(shoppingCartHeader),
                ShoppingCartDetails = _mapper.Map<List<ShoppingCartDetailDto>>(shoppingCartDetails),
            };

            var productsDtos = await _productService.GetProducts();
            foreach (var detail in cartDto.ShoppingCartDetails)
            {
                detail.Product = productsDtos.FirstOrDefault(p => p.ProductId == detail.ProductId);
                cartDto.ShoppingCartHeader.Price += detail.Amount * (detail.Product?.Price ?? 0);
            }

            cartDto.ShoppingCartHeader.CartTotal = cartDto.ShoppingCartHeader.Price;

            if (!string.IsNullOrEmpty(cartDto.ShoppingCartHeader.CouponCode))
            {
                var coupon = await _couponService.GetCoupon(cartDto.ShoppingCartHeader.CouponCode);
                if (coupon != null && cartDto.ShoppingCartHeader.Price >= coupon.MinimalAmount)
                {
                    cartDto.ShoppingCartHeader.Discount = coupon.DiscountAmount;
                    cartDto.ShoppingCartHeader.CartTotal = cartDto.ShoppingCartHeader.Price - coupon.DiscountAmount;
                }
            }

            cartDto.ShoppingCartHeader.Price = Math.Round(cartDto.ShoppingCartHeader.Price, 2);
            cartDto.ShoppingCartHeader.CartTotal = Math.Round(cartDto.ShoppingCartHeader.CartTotal, 2);
            _response.Data = cartDto;
            _response.IsSuccess = true;
            return _response;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
            return _response;
        }


    }

    [HttpPost("applyCoupon")]
    public async Task<ResponseDto> ApplyCoupon(ShoppingCartDto shoppingCartDto)
    {
        try
        {
            var shoppingCartHeader = await _dbCtx.Headers.FirstAsync(h => h.UserId == shoppingCartDto.ShoppingCartHeader.UserId);
            var shoppingCartDetails = await _dbCtx.Details.Where(d => d.ShoppingCartHeaderId == shoppingCartHeader.ShoppingCartHeaderId).ToListAsync();

            var productsDtos = await _productService.GetProducts();
            foreach (var detail in shoppingCartDetails)
            {
                detail.Product = productsDtos.FirstOrDefault(p => p.ProductId == detail.ProductId);
            }

            var coupon = await _couponService.GetCoupon(shoppingCartDto.ShoppingCartHeader.CouponCode);
            if (!string.IsNullOrEmpty(shoppingCartDto.ShoppingCartHeader.CouponCode) && coupon.CouponId == 0)
            {
                throw new Exception("The coupon could not be found");
            }

            if (shoppingCartDetails.Sum(d => d.Amount * d.Product.Price) < coupon.MinimalAmount)
            {
                throw new Exception("The minimal amount required to apply the coupon was not achieved");
            }

            shoppingCartHeader.CouponCode = shoppingCartDto.ShoppingCartHeader.CouponCode;
            _dbCtx.Headers.Update(shoppingCartHeader);

            await _dbCtx.SaveChangesAsync();

            _response.IsSuccess = true;
            return _response;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
            return _response;
        }
    }

    [HttpPost("upsert")]
    public async Task<ResponseDto> Upsert(ShoppingCartDto shoppingCartDto)
    {
        try
        {
            var shoppingCartHeader = await _dbCtx.Headers.AsNoTracking().FirstOrDefaultAsync(h => h.UserId == shoppingCartDto.ShoppingCartHeader.UserId);
            if (shoppingCartHeader is null)
            {
                shoppingCartHeader = _mapper.Map<ShoppingCartHeader>(shoppingCartDto.ShoppingCartHeader);
                await _dbCtx.Headers.AddAsync(shoppingCartHeader);
                await _dbCtx.SaveChangesAsync();

                shoppingCartDto.ShoppingCartDetails!.First().ShoppingCartHeaderId = shoppingCartHeader.ShoppingCartHeaderId;
                await _dbCtx.Details.AddAsync(_mapper.Map<ShoppingCartDetail>(shoppingCartDto.ShoppingCartDetails!.First()));
                await _dbCtx.SaveChangesAsync();
            }
            else
            {
                var shoppingCartDetails = await _dbCtx.Details.AsNoTracking().FirstOrDefaultAsync(
                    d => d.ShoppingCartHeaderId == shoppingCartHeader.ShoppingCartHeaderId
                    && d.ProductId == shoppingCartDto.ShoppingCartDetails!.First().ProductId);

                if (shoppingCartDetails is null)
                {
                    shoppingCartDto.ShoppingCartDetails!.First().ShoppingCartHeaderId = shoppingCartHeader.ShoppingCartHeaderId;
                    shoppingCartDetails = _mapper.Map<ShoppingCartDetail>(shoppingCartDto.ShoppingCartDetails!.First());
                    await _dbCtx.Details.AddAsync(_mapper.Map<ShoppingCartDetail>(shoppingCartDto.ShoppingCartDetails!.First()));
                    await _dbCtx.SaveChangesAsync();
                }
                else
                {
                    shoppingCartDto.ShoppingCartDetails!.First().Amount += shoppingCartDetails.Amount;
                    shoppingCartDto.ShoppingCartDetails!.First().ShoppingCartHeaderId = shoppingCartDetails.ShoppingCartHeaderId;
                    shoppingCartDto.ShoppingCartDetails!.First().ShoppingCartDetailId = shoppingCartDetails.ShoppingCartDetailId;

                    var teste = _mapper.Map<ShoppingCartDetail>(shoppingCartDto.ShoppingCartDetails!.First());
                    _dbCtx.Details.Update(teste);
                    await _dbCtx.SaveChangesAsync();
                }
            }

            _response.Data = shoppingCartDto;
            _response.IsSuccess = true;
            return _response;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
            return _response;
        }
    }

    /// <summary>
    /// Deletes a detail from the cart, and if there are no details left, it removes the cart header.
    /// </summary>
    /// <param name="cartDetailsId"></param>
    /// <returns></returns>
    [HttpDelete("details/{cartDetailsId}")]
    public async Task<ResponseDto> RemoveCart(int cartDetailsId)
    {
        try
        {
            var shoppingCartDetail = await _dbCtx.Details.FirstAsync(d => d.ShoppingCartDetailId == cartDetailsId);
            _dbCtx.Details.Remove(shoppingCartDetail);

            int totalCountCart = await _dbCtx.Details.Where(d => d.ShoppingCartHeaderId == shoppingCartDetail.ShoppingCartHeaderId).CountAsync();
            if (totalCountCart == 1)
            {
                var shoppingCartHeader = await _dbCtx.Headers.FirstAsync(d => d.ShoppingCartHeaderId == shoppingCartDetail.ShoppingCartHeaderId);
                _dbCtx.Headers.Remove(shoppingCartHeader);
            }

            await _dbCtx.SaveChangesAsync();

            _response.IsSuccess = true;
            return _response;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
            return _response;
        }
    }

    [HttpPost("emailCart")]
    public async Task<ResponseDto> EmailCart(ShoppingCartDto shoppingCartDto)
    {
        try
        {
            await _messageBusService.PublishAsync(_configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"), shoppingCartDto);
            _response.IsSuccess = true;
            return _response;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
            return _response;
        }
    }
}
