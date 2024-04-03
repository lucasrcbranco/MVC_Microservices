using Mango.Services.RewardAPI.Messages;
using Mango.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;
using RewardAPI.Data;

namespace Mango.Services.RewardAPI.Services;

public class RewardService : IRewardService
{
    private DbContextOptions<AppDbContext> _appDbCtxOptions;

    public RewardService(DbContextOptions<AppDbContext> appDbCtxOptions)
    {
        _appDbCtxOptions = appDbCtxOptions;
    }

    public async Task UpdateRewardsAsync(RewardMessage rewardMessage)
    {
        Reward reward = new Reward
        {
            OrderId = rewardMessage.OrderId,
            UserId = rewardMessage.UserId,
            Points = rewardMessage.Points,
            EarnedAt = DateTime.UtcNow
        };

        try
        {
            await using var _db = new AppDbContext(_appDbCtxOptions);
            await _db.Rewards.AddAsync(reward);
            await _db.SaveChangesAsync();
        }
        catch(Exception ex)
        {
            throw ex;
        }
    }
}
