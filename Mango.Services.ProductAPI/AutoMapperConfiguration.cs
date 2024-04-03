using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dtos;

namespace ProductAPI;

public static class AutoMapperConfiguration
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(configuration =>
        {
            configuration.CreateMap<Product, ProductDto>().ReverseMap();
        });

        return mappingConfig;
    }
}
