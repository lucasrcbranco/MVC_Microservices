using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Web.Models;

public class OrderDetailsDto
{
    public int? OrderDetailsId { get; set; }
    public int? OrderHeaderId { get; set; }
    public OrderHeaderDto? OrderHeader { get; set; }
    public string? ProductId { get; set; }
    public ProductDto? Product { get; set; }
    public int Amount { get; set; }
    public string ProductName { get; set; } = null!;
    public double ProductPrice { get; set; }
}
