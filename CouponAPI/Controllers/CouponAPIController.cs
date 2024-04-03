using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dtos;
using Mango.Services.CouponAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers;

[Route("api/coupon")]
[ApiController]
[Authorize]
public class CouponAPIController : ControllerBase
{
    private readonly AppDbContext _dbCtx;
    private readonly IMapper _mapper;
    private readonly ResponseDto _response;

    public CouponAPIController(AppDbContext dbCtx, IMapper mapper)
    {
        _dbCtx = dbCtx;
        _mapper = mapper;
        _response = new ResponseDto();
    }

    [HttpGet]
    public ResponseDto Get()
    {
        try
        {
            var coupons = _dbCtx.Coupons.ToList();
            _response.Data = _mapper.Map<List<CouponDto>>(coupons);
            _response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            _response.Message = ex.Message;
            _response.IsSuccess = false;
        }

        return _response;
    }

    [HttpGet("{id:int}")]
    public ResponseDto Get(int id)
    {
        try
        {
            var coupon = _dbCtx.Coupons.First(c => c.CouponId == id);
            _response.Data = _mapper.Map<CouponDto>(coupon);
            _response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            _response.Message = ex.Message;
            _response.IsSuccess = false;
        }

        return _response;
    }

    [HttpGet("get-by-code/{code}")]
    public ResponseDto GetByCode(string code)
    {
        try
        {
            var coupon = _dbCtx.Coupons.First(c => c.CouponCode.ToLower() == code.ToLower());
            _response.Data = _mapper.Map<CouponDto>(coupon);
            _response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            _response.Message = ex.Message;
            _response.IsSuccess = false;
        }

        return _response;
    }

    [HttpPost]
    [Authorize(Roles = SD.RoleAdmin)]
    public ResponseDto Create([FromBody] CouponDto couponDto)
    {
        try
        {
            var coupon = _dbCtx.Coupons.FirstOrDefault(c => c.CouponCode == couponDto.CouponCode);
            if (coupon is not null)
            {
                _response.IsSuccess = false;
                _response.Message = $"There is already one coupon registered for the code {couponDto.CouponCode}";
                return _response;
            }

            var options = new Stripe.CouponCreateOptions
            {
                AmountOff = (long)(couponDto.DiscountAmount * 100),
                Name = couponDto.CouponCode,
                Currency = "USD",
                Id = couponDto.CouponCode
            };

            var service = new Stripe.CouponService();
            service.Create(options);

            coupon = _mapper.Map<Coupon>(couponDto);
            _dbCtx.Coupons.Add(coupon);
            _dbCtx.SaveChanges();
            _response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            _response.Message = ex.Message;
            _response.IsSuccess = false;
        }

        return _response;
    }

    [HttpPut]
    [Authorize(Roles = SD.RoleAdmin)]
    public ResponseDto Update([FromBody] CouponDto couponDto)
    {
        try
        {
            var service = new Stripe.CouponService();

            service.Delete(couponDto.CouponCode);

            var options = new Stripe.CouponCreateOptions
            {
                AmountOff = (long)(couponDto.DiscountAmount * 100),
                Name = couponDto.CouponCode,
                Currency = "USD",
                Id = couponDto.CouponCode
            };

            service.Create(options);

            var coupon = _mapper.Map<Coupon>(couponDto);
            _dbCtx.Coupons.Update(coupon);
            _dbCtx.SaveChanges();
            _response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            _response.Message = ex.Message;
            _response.IsSuccess = false;
        }

        return _response;
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = SD.RoleAdmin)]
    public ResponseDto Delete(int id)
    {
        try
        {
            var coupon = _dbCtx.Coupons.First(c => c.CouponId == id);

            _dbCtx.Coupons.Remove(coupon);
            _dbCtx.SaveChanges();

            var service = new Stripe.CouponService();
            service.Delete(coupon.CouponCode);

            _response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            _response.Message = ex.Message;
            _response.IsSuccess = false;
        }

        return _response;
    }
}
