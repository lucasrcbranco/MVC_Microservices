using Mango.Services.ShoppingCartAPI.Models.Dtos;
using Mango.Services.ShoppingCartAPI.Services.IServices;
using Newtonsoft.Json;
using ShoppingCartAPI.Models.Dtos;
using ShoppingCartAPI.Utility;

namespace Mango.Services.ShoppingCartAPI.Services;

public class ProductService : IProductService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ProductService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IEnumerable<ProductDto>> GetProducts()
    {
        var client = _httpClientFactory.CreateClient(nameof(SD.ProductAPIHttpClientURL));
        var clientResponse = await client.GetAsync("/api/product");
        var clientResponseContent = await clientResponse.Content.ReadAsStringAsync();


        var response = JsonConvert.DeserializeObject<ResponseDto>(clientResponseContent);
        if (response != null && response.IsSuccess)
        {
            return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(response.Data));
        }

        return new List<ProductDto>();
    }
}
