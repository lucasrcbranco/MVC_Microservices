﻿namespace Mango.Services.RewardAPI.Messages;

public class RewardMessage
{
    public string? UserId { get; set; }
    public int OrderId { get; set; }
    public int Points { get; set; }
}
