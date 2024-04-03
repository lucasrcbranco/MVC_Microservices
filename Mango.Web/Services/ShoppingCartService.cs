using Mango.Web.Models;
using Mango.Web.Services.Interfaces;
using Mango.Web.Utility;

namespace Mango.Web.Services;

public class ShoppingCartService : IShoppingCartService
{
    private readonly IBaseService _baseService;

    public ShoppingCartService(IBaseService baseService)
    {
        _baseService = baseService;
    }

    public async Task<ResponseDto?> GetCartByUserIdAsync(string userId)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = SD.ApiType.GET,
            URL = SD.ShoppingCartAPIBase + "/api/shopping-cart/" + userId
        });
    }

    public async Task<ResponseDto?> UpsertCartAsync(ShoppingCartDto shoppingCartDto)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = SD.ApiType.POST,
            URL = SD.ShoppingCartAPIBase + "/api/shopping-cart/upsert",
            Data = shoppingCartDto
        });
    }

    public async Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsId)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = SD.ApiType.DELETE,
            URL = SD.ShoppingCartAPIBase + "/api/shopping-cart/details/" + cartDetailsId
        });
    }

    public async Task<ResponseDto?> ApplyCouponToCartAsync(ShoppingCartDto shoppingCartDto)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = SD.ApiType.POST,
            URL = SD.ShoppingCartAPIBase + "/api/shopping-cart/applyCoupon",
            Data = shoppingCartDto
        });
    }

    public async Task<ResponseDto?> EmailCartAsync(ShoppingCartDto shoppingCartDto)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = SD.ApiType.POST,
            URL = SD.ShoppingCartAPIBase + "/api/shopping-cart/emailCart",
            Data = shoppingCartDto
        });
    }
}
