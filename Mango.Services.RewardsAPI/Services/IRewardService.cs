﻿using Mango.Services.RewardAPI.Messages;

namespace Mango.Services.RewardAPI.Services;

public interface IRewardService
{
    Task UpdateRewardsAsync(RewardMessage rewardMessage);
}
