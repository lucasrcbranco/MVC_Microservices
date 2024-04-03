using Mango.Web.Models;

namespace Mango.Web.Services.Interfaces;

public interface IOrderService
{
    Task<ResponseDto?> CreateOrderAsync(ShoppingCartDto shoppingCartDto);
    Task<ResponseDto?> CreateStripeSessionAsync(StripeRequestDto stripeRequestDto);
    Task<ResponseDto?> ValidateStripeSessionAsync(int orderHeaderId);
    Task<ResponseDto?> GetOrders(string? userId);
    Task<ResponseDto?> GetOrderById(int orderHeaderId);
    Task<ResponseDto?> UpdateStatus(int orderHeaderId, string status);

}