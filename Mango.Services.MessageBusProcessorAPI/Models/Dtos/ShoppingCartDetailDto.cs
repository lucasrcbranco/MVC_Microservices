namespace Mango.Services.EmailAPI.Models.Dtos;

public class ShoppingCartDetailDto
{
    public int? ShoppingCartDetailId { get; set; }
    public int ShoppingCartHeaderId { get; set; }
    public ShoppingCartHeaderDto? ShoppingCartHeader { get; set; }
    public int ProductId { get; set; }
    public ProductDto? Product { get; set; }
    public int Amount { get; set; }
}
