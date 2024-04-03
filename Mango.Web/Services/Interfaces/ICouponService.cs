using Mango.Web.Models;

namespace Mango.Web.Services.Interfaces;

public interface ICouponService
{
    Task<ResponseDto?> GetAllCouponsAsync();
    Task<ResponseDto?> GetCouponByIdAsync(int couponId);
    Task<ResponseDto?> GetCouponByCodeAsync(string couponCode);
    Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto);
    Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto);
    Task<ResponseDto?> DeleteCouponAsync(int couponId);
}
