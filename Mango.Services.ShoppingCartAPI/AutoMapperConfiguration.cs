using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dtos;

namespace ShoppingCartAPI;

public static class AutoMapperConfiguration
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(configuration =>
        {
            configuration.CreateMap<ShoppingCartHeader, ShoppingCartHeaderDto>().ReverseMap();
            configuration.CreateMap<ShoppingCartDetail, ShoppingCartDetailDto>().ReverseMap();
        });

        return mappingConfig;
    }
}
