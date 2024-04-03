using Mango.Services.ShoppingCartAPI.Models.Dtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.ShoppingCartAPI.Models;

public class ShoppingCartDetail
{
    public ShoppingCartDetail()
    {

    }

    [Key]
    public int ShoppingCartDetailId { get; set; }
    public int ShoppingCartHeaderId { get; set; }

    [ForeignKey(nameof(ShoppingCartHeaderId))]
    public ShoppingCartHeader? ShoppingCartHeader { get; set; }

    public int ProductId { get; set; }

    [NotMapped]
    public ProductDto? Product { get; set; }

    public int Amount { get; set; }
}
