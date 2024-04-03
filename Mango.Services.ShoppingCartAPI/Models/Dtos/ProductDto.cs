namespace Mango.Services.ShoppingCartAPI.Models.Dtos;

public class ProductDto
{
    public int? ProductId { get; set; }
    public string Name { get; set; } = null!;
    public double Price { get; set; }
    public string? Description { get; set; }
    public string? CategoryName { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageLocalPath { get; set; }
}
