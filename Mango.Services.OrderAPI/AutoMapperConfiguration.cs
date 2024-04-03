using AutoMapper;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dtos;

namespace OrderAPI;

public static class AutoMapperConfiguration
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(configuration =>
        {
            configuration.CreateMap<OrderHeaderDto, ShoppingCartHeaderDto>()
            .ForMember(destination => destination.CartTotal, x => x.MapFrom(source => source.OrderTotal))
            .ReverseMap();

            configuration.CreateMap<OrderDetailsDto, ShoppingCartDetailDto>()
            .ForPath(destination => destination.Product!.Name, x => x.MapFrom(source => source.ProductName))
            .ForPath(destination => destination.Product!.Price, x => x.MapFrom(source => source.ProductPrice));

            configuration.CreateMap<ShoppingCartDetailDto, OrderDetailsDto>();

            configuration.CreateMap<OrderHeaderDto, OrderHeader>().ReverseMap();
            configuration.CreateMap<OrderDetailsDto, OrderDetails>().ReverseMap();
        });

        return mappingConfig;
    }
}
