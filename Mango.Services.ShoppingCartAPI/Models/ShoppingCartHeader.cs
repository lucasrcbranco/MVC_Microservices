using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.ShoppingCartAPI.Models;

public class ShoppingCartHeader
{
    public ShoppingCartHeader()
    {

    }

    [Key]
    public int ShoppingCartHeaderId { get; set; }
    public string? UserId { get; set; }
    public string? CouponCode { get; set; }

    [NotMapped]
    public double Discount { get; set; }

    [NotMapped]
    public double CartTotal { get; set; }
}
