namespace Mango.Services.RewardAPI.Models;

public class Reward
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public int OrderId { get; set; }
    public DateTime EarnedAt { get; set; }
    public int Points { get; set; }
}
