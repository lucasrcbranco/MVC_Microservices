using Mango.Services.OrderAPI.Models.Dtos;
using Mango.Services.OrderAPI.Services.IServices;
using Mango.Services.OrderAPI.Utility;
using Newtonsoft.Json;

namespace Mango.Services.OrderAPI.Services;

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
