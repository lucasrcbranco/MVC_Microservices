using System.ComponentModel.DataAnnotations;

namespace Mango.Services.CouponAPI.Models;

public class Coupon
{
    [Key]
    public int CouponId { get; private set; }

    [Required]
    public string CouponCode { get; private set; } = null!;

    [Required]
    public double DiscountAmount { get; private set; }

    public double MinimalAmount { get; private set; }

    private Coupon()
    {

    }

    public Coupon(int id, string couponCode, double discountAmount, int minimalAmount)
    {
        CouponId = id;
        CouponCode = couponCode;
        DiscountAmount = discountAmount;
        MinimalAmount = minimalAmount;
    }

}
