using Mango.Web.Models;
using Mango.Web.Services.Interfaces;
using Mango.Web.Utility;

namespace Mango.Web.Services;

public class OrderService : IOrderService
{
    private readonly IBaseService _baseService;

    public OrderService(IBaseService baseService)
    {
        _baseService = baseService;
    }

    public async Task<ResponseDto?> CreateOrderAsync(ShoppingCartDto shoppingCartDto)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = SD.ApiType.POST,
            URL = SD.OrderAPIBase + $"/api/order",
            Data = shoppingCartDto
        });
    }

    public async Task<ResponseDto?> CreateStripeSessionAsync(StripeRequestDto stripeRequestDto)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = SD.ApiType.POST,
            URL = SD.OrderAPIBase + $"/api/order/create-stripe-session",
            Data = stripeRequestDto
        });
    }

    public async Task<ResponseDto?> GetOrderById(int orderHeaderId)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = SD.ApiType.GET,
            URL = SD.OrderAPIBase + $"/api/order/{orderHeaderId}"
        });
    }

    public async Task<ResponseDto?> GetOrders(string? userId)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = SD.ApiType.GET,
            URL = SD.OrderAPIBase + $"/api/order?userid={userId}"
        });
    }

    public async Task<ResponseDto?> UpdateStatus(int orderHeaderId, string status)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = SD.ApiType.PUT,
            URL = SD.OrderAPIBase + $"/api/order/{orderHeaderId}/status",
            Data = status
        });
    }

    public async Task<ResponseDto?> ValidateStripeSessionAsync(int orderHeaderId)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = SD.ApiType.POST,
            URL = SD.OrderAPIBase + $"/api/order/validate-stripe-session",
            Data = orderHeaderId
        });
    }
}
