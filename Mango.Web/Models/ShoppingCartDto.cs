namespace Mango.Web.Models;

public class ShoppingCartDto
{
    public ShoppingCartHeaderDto ShoppingCartHeader { get; set; } = new();
    public IEnumerable<ShoppingCartDetailDto> ShoppingCartDetails { get; set; } = new List<ShoppingCartDetailDto>();

}
