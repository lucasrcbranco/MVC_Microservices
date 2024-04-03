namespace Mango.Web.Models;

public class ShoppingCartHeaderDto
{
    public int? ShoppingCartHeaderId { get; set; }
    public string? UserId { get; set; }
    public string? CouponCode { get; set; }
    public double Discount { get; set; }
    public double Price { get; set; }
    public double CartTotal { get; set; }

    public string? Name { get; set; }
    public string? Email { get; set; }
}
