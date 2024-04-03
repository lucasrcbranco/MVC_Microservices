using AutoMapper;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dtos;

namespace Mango.Services.CouponAPI;

public static class AutoMapperConfiguration
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(configuration =>
        {
            configuration.CreateMap<Coupon, CouponDto>().ReverseMap();
        });

        return mappingConfig;
    }
}
