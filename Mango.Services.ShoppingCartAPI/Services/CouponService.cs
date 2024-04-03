using Mango.Services.ShoppingCartAPI.Models.Dtos;
using Mango.Services.ShoppingCartAPI.Services.IServices;
using Newtonsoft.Json;
using ShoppingCartAPI.Models.Dtos;
using ShoppingCartAPI.Utility;

namespace Mango.Services.ShoppingCartAPI.Services;

public class CouponService : ICouponService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CouponService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<CouponDto> GetCoupon(string couponCode)
    {
        var client = _httpClientFactory.CreateClient(nameof(SD.CouponAPIHttpClientURL));
        var clientResponse = await client.GetAsync($"/api/coupon/get-by-code/{couponCode}");
        var clientResponseContent = await clientResponse.Content.ReadAsStringAsync();


        var response = JsonConvert.DeserializeObject<ResponseDto>(clientResponseContent);
        if (response != null && response.IsSuccess)
        {
            return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Data));
        }

        return new CouponDto();
    }
}
