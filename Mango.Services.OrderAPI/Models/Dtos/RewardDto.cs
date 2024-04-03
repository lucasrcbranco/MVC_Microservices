namespace Mango.Services.OrderAPI.Models.Dtos;

public class RewardDto
{
    public string? UserId { get; set; }
    public int OrderId { get; set; }
    public int Points { get; set; }
}
