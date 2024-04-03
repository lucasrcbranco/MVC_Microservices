using RewardAPI.Integration;

namespace Mango.Services.RewardAPI.Extensions;

public static class ApplicationBuilderExtensions
{
    private static IRewardProcessor? _rewardProcessor { get; set; }

    public static IApplicationBuilder ConfigureServiceBusConsumer(this IApplicationBuilder applicationBuilder)
    {
        _rewardProcessor = applicationBuilder.ApplicationServices.GetService<IRewardProcessor>();
        var hostApplicationLife = applicationBuilder.ApplicationServices.GetService<IHostApplicationLifetime>();

        if (hostApplicationLife != null)
        {
            hostApplicationLife.ApplicationStarted.Register(OnStart);
            hostApplicationLife.ApplicationStopping.Register(OnStop);
        }

        return applicationBuilder;
    }

    private static async void OnStop()
    {
        if (_rewardProcessor is null) throw new ArgumentException("Could not create an instance of message bus service");
        await _rewardProcessor.StopListeningAsync();
    }

    private static async void OnStart()
    {
        if (_rewardProcessor is null) throw new ArgumentException("Could not create an instance of message bus service");
        await _rewardProcessor.ListenAsync();
    }
}
