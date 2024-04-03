using Azure.Messaging.ServiceBus;
using Mango.Services.RewardAPI.Messages;
using Mango.Services.RewardAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace RewardAPI.Integration;

public class RewardProcessor : IRewardProcessor
{
    private readonly string _connectionString;
    private readonly string _orderCreatedTopic;
    private readonly string _orderCreatedRewardSubscription;
    private readonly RewardService _rewardService;

    private readonly ServiceBusProcessor _rewardProcessor;

    public RewardProcessor(IConfiguration configuration, RewardService rewardService)
    {
        _connectionString = configuration.GetConnectionString("ServiceBusConnection") ?? string.Empty;
        _orderCreatedTopic = configuration.GetValue<string>("TopicsAndQueueNames:OrderCreatedTopic") ?? string.Empty;
        _orderCreatedRewardSubscription = configuration.GetValue<string>("TopicsAndQueueNames:OrderCreated_Rewards_Subscription") ?? string.Empty;

        if (string.IsNullOrEmpty(_connectionString)) throw new ArgumentException("Could not load the connection string for the message bus");
        if (string.IsNullOrEmpty(_orderCreatedTopic)) throw new ArgumentException("Could not load the name of the order created topic queue");

        var client = new ServiceBusClient(_connectionString);
        _rewardProcessor = client.CreateProcessor(_orderCreatedTopic, _orderCreatedRewardSubscription);
        _rewardService = rewardService;
    }

    public async Task ListenAsync()
    {
        _rewardProcessor.ProcessMessageAsync += ProcessRewardAsync;
        _rewardProcessor.ProcessErrorAsync += ErrorHandler;
        await _rewardProcessor.StartProcessingAsync();
    }

    public async Task StopListeningAsync()
    {
        await _rewardProcessor.StopProcessingAsync();
        await _rewardProcessor.DisposeAsync();
    }

    private async Task ProcessRewardAsync(ProcessMessageEventArgs args)
    {
        RewardMessage rewardDto = JsonConvert.DeserializeObject<RewardMessage>(Encoding.UTF8.GetString(args.Message.Body));
        await _rewardService.UpdateRewardsAsync(rewardDto);
        await args.CompleteMessageAsync(args.Message);
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        return Task.CompletedTask;
    }
}