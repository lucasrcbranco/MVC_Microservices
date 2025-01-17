﻿using Mango.Services.ShoppingCartAPI.Models.Dtos;

namespace Mango.Services.ShoppingCartAPI.Services.IServices;

public interface ICouponService
{
    Task<CouponDto> GetCoupon(string couponCode);
}
