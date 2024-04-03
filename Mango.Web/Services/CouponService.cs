﻿using Mango.Web.Models;
using Mango.Web.Services.Interfaces;
using Mango.Web.Utility;

namespace Mango.Web.Services;

public class CouponService : ICouponService
{
    private readonly IBaseService _baseService;

    public CouponService(IBaseService baseService)
    {
        _baseService = baseService;
    }

    public async Task<ResponseDto?> GetAllCouponsAsync()
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = SD.ApiType.GET,
            URL = SD.CouponAPIBase + "/api/coupon"
        });
    }

    public async Task<ResponseDto?> GetCouponByIdAsync(int couponId)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = SD.ApiType.GET,
            URL = SD.CouponAPIBase + $"/api/coupon/{couponId}"
        });
    }

    public async Task<ResponseDto?> GetCouponByCodeAsync(string couponCode)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = SD.ApiType.GET,
            URL = SD.CouponAPIBase + $"/api/coupon/GetByCode/{couponCode}"
        });
    }

    public async Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = SD.ApiType.POST,
            URL = SD.CouponAPIBase + $"/api/coupon",
            Data = couponDto
        });
    }

    public async Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = SD.ApiType.PUT,
            URL = SD.CouponAPIBase + $"/api/coupon",
            Data = couponDto
        });
    }

    public async Task<ResponseDto?> DeleteCouponAsync(int couponId)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = SD.ApiType.DELETE,
            URL = SD.CouponAPIBase + $"/api/coupon/{couponId}"
        });
    }
}
