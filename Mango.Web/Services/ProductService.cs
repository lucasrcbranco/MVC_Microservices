using Mango.Web.Models;
using Mango.Web.Services.Interfaces;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Services;

public class ProductService : IProductService
{
    private readonly IBaseService _baseService;

    public ProductService(IBaseService baseService)
    {
        _baseService = baseService;
    }

    public async Task<ResponseDto?> GetAllProductsAsync()
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = ApiType.GET,
            URL = ProductAPIBase + "/api/Product"
        });
    }

    public async Task<ResponseDto?> GetProductByIdAsync(int ProductId)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = ApiType.GET,
            URL = ProductAPIBase + $"/api/Product/{ProductId}"
        });
    }

    public async Task<ResponseDto?> GetProductByCodeAsync(string ProductCode)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = ApiType.GET,
            URL = ProductAPIBase + $"/api/Product/GetByCode/{ProductCode}"
        });
    }

    public async Task<ResponseDto?> CreateProductAsync(ProductDto ProductDto)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = ApiType.POST,
            URL = ProductAPIBase + $"/api/Product",
            Data = ProductDto,
            ContentType = ContentType.MultipartFormData
        });
    }

    public async Task<ResponseDto?> UpdateProductAsync(ProductDto ProductDto)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = ApiType.PUT,
            URL = ProductAPIBase + $"/api/Product",
            Data = ProductDto,
            ContentType = ContentType.MultipartFormData
        });
    }

    public async Task<ResponseDto?> DeleteProductAsync(int ProductId)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = ApiType.DELETE,
            URL = ProductAPIBase + $"/api/Product/{ProductId}"
        });
    }
}
