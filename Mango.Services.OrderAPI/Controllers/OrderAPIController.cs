using AutoMapper;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dtos;
using Mango.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Data;
using OrderAPI.Integration;
using Stripe;
using Stripe.Checkout;

namespace Mango.Services.OrderAPI.Controllers;

[Route("api/order")]
[ApiController]
public class OrderAPIController : ControllerBase
{
    private ResponseDto _responseDto;
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly IMessageBusService _messageBusService;

    public OrderAPIController(
        IMapper mapper,
        AppDbContext db,
        IConfiguration configuration,
        IMessageBusService messageBusService)
    {
        _responseDto = new ResponseDto();
        _mapper = mapper;
        _db = db;
        _configuration = configuration;
        _messageBusService = messageBusService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ResponseDto> Get([FromQuery] string? userId = "")
    {
        try
        {
            IEnumerable<OrderHeader> orderHeaders;
            if (User.IsInRole(SD.RoleAdmin))
            {
                orderHeaders = await _db.OrderHeaders.Include(oh => oh.Details).OrderByDescending(oh => oh.OrderHeaderId).ToListAsync();
            }
            else
            {
                orderHeaders = await _db.OrderHeaders.Include(oh => oh.Details).Where(oh => oh.UserId == userId).ToListAsync();
            }

            _responseDto.IsSuccess = true;
            _responseDto.Data = _mapper.Map<IEnumerable<OrderHeaderDto>>(orderHeaders);
        }
        catch (Exception ex)
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = ex.Message;
        }

        return _responseDto;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ResponseDto> GetByid(int id)
    {
        try
        {
            OrderHeader orderHeader = await _db.OrderHeaders.Include(oh => oh.Details).FirstAsync(oh => oh.OrderHeaderId == id);
            _responseDto.IsSuccess = true;
            _responseDto.Data = _mapper.Map<OrderHeaderDto>(orderHeader);
        }
        catch (Exception ex)
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = ex.Message;
        }

        return _responseDto;
    }


    [HttpPost]
    [Authorize]
    public async Task<ResponseDto> CreateOrder([FromBody] ShoppingCartDto shoppingCartDto)
    {
        try
        {
            OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(shoppingCartDto.ShoppingCartHeader);
            orderHeaderDto.OrderTime = DateTime.UtcNow;
            orderHeaderDto.Status = SD.StatusPending;
            orderHeaderDto.Details = _mapper.Map<IEnumerable<OrderDetailsDto>>(shoppingCartDto.ShoppingCartDetails);

            OrderHeader orderHeader = _mapper.Map<OrderHeader>(orderHeaderDto);

            await _db.AddAsync(orderHeader);
            await _db.SaveChangesAsync();

            orderHeaderDto.OrderHeaderId = orderHeader.OrderHeaderId;
            _responseDto.IsSuccess = true;
            _responseDto.Data = orderHeaderDto;
        }
        catch (Exception ex)
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = ex.Message;
        }

        return _responseDto;
    }

    [HttpPost("create-stripe-session")]
    [Authorize]
    public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
    {
        try
        {
            var options = new SessionCreateOptions
            {
                SuccessUrl = stripeRequestDto.ApprovedUrl,
                CancelUrl = stripeRequestDto.CancelUrl,
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment"
            };

            if (stripeRequestDto.OrderHeader.Discount > 0)
            {
                options.Discounts = new List<SessionDiscountOptions>()
                {
                    new()
                    {
                        Coupon=stripeRequestDto.OrderHeader.CouponCode
                    }
                }; ;
            }

            foreach (var item in stripeRequestDto.OrderHeader.Details)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.ProductPrice * 100),
                        Currency = "USD",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.ProductName
                        },
                    },
                    Quantity = item.Amount
                });
            }

            var service = new SessionService();
            Session stripeSession = service.Create(options);
            stripeRequestDto.StripeSessionUrl = stripeSession.Url;

            OrderHeader orderHeader = await _db.OrderHeaders.FirstAsync(o => o.OrderHeaderId == stripeRequestDto.OrderHeader.OrderHeaderId);
            orderHeader.StripeSessionId = stripeSession.Id;

            await _db.SaveChangesAsync();
            _responseDto.IsSuccess = true;
            _responseDto.Data = stripeRequestDto;
        }
        catch (Exception ex)
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = ex.Message;
        }

        return _responseDto;
    }

    [HttpPost("validate-stripe-session")]
    [Authorize]
    public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId)
    {
        try
        {
            OrderHeader orderHeader = await _db.OrderHeaders.FirstAsync(o => o.OrderHeaderId == orderHeaderId);

            var service = new SessionService();
            Session stripeSession = service.Get(orderHeader.StripeSessionId);

            PaymentIntentService paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent = paymentIntentService.Get(stripeSession.PaymentIntentId);

            if (paymentIntent.Status == "succeeded")
            {
                orderHeader.PaymentIntentId = paymentIntent.Id;
                orderHeader.Status = SD.StatusApproved;
                _db.Update(orderHeader);
                await _db.SaveChangesAsync();

                RewardDto rewardDto = new RewardDto()
                {
                    OrderId = orderHeader.OrderHeaderId!.Value,
                    UserId = orderHeader.UserId!,
                    Points = (int)orderHeader.OrderTotal
                };

                await _messageBusService.PublishAsync(_configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic"), rewardDto);
                _responseDto.Data = _mapper.Map<OrderHeaderDto>(orderHeader);
            }

            _responseDto.IsSuccess = true;
        }
        catch (Exception ex)
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = ex.Message;
        }

        return _responseDto;
    }

    [HttpPut("{orderHeaderId}/status")]
    [Authorize]
    public async Task<ResponseDto> UpdateStatus(int orderHeaderId, [FromBody] string status)
    {
        try
        {
            OrderHeader orderHeader = await _db.OrderHeaders.FirstAsync(o => o.OrderHeaderId == orderHeaderId);

            if (status == SD.StatusCancelled)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var refundService = new RefundService();
                Refund refund = await refundService.CreateAsync(options);
            }

            orderHeader.Status = status;
            _db.OrderHeaders.Update(orderHeader);
            await _db.SaveChangesAsync();
            _responseDto.IsSuccess = true;
        }
        catch (Exception ex)
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = ex.Message;
        }

        return _responseDto;
    }
}
