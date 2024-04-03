using Mango.Web.Models;

namespace Mango.Web.Services.Interfaces;

public interface IShoppingCartService
{
    Task<ResponseDto?> GetCartByUserIdAsync(string userId);
    Task<ResponseDto?> UpsertCartAsync(ShoppingCartDto shoppingCartDto);
    Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsId);
    Task<ResponseDto?> ApplyCouponToCartAsync(ShoppingCartDto shoppingCartDto);
    Task<ResponseDto?> EmailCartAsync(ShoppingCartDto shoppingCartDto);
}
