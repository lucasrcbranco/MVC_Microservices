namespace RewardAPI.Integration;

public interface IRewardProcessor
{
    Task ListenAsync();
    Task StopListeningAsync();
}