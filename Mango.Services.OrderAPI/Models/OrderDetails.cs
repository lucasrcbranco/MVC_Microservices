using Mango.Services.OrderAPI.Models.Dtos;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.OrderAPI.Models;

public class OrderDetails
{
    public int? OrderDetailsId { get; set; }
    public int? OrderHeaderId { get; set; }
    public string? ProductId { get; set; }

    [NotMapped]
    public ProductDto? Product { get; set; }

    public int Amount { get; set; }
    public string ProductName { get; set; } = null!;
    public double ProductPrice { get; set; }
}
