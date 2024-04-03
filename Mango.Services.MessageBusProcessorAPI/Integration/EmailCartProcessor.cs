using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Messages;
using Mango.Services.EmailAPI.Models.Dtos;
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace EmailAPI.Integration;

public class EmailCartProcessor : IEmailCartProcessor
{
    private readonly string _connectionString;
    private readonly string _emailCartQueueName;
    private readonly string _orderCreatedTopic;
    private readonly string _orderCreatedEmailSubscrption;

    private readonly EmailService _emailService;

    private readonly ServiceBusProcessor _emailCartProcessor;
    private readonly ServiceBusProcessor _emailOrderProcessor;

    public EmailCartProcessor(IConfiguration configuration, EmailService emailService)
    {
        _connectionString = configuration.GetConnectionString("ServiceBusConnection") ?? string.Empty;
        _emailCartQueueName = configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue") ?? string.Empty;
        _orderCreatedTopic = configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic") ?? string.Empty;
        _orderCreatedEmailSubscrption = configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Email_Subscription") ?? string.Empty;

        if (string.IsNullOrEmpty(_connectionString)) throw new ArgumentException("Could not load the connection string for the message bus");
        if (string.IsNullOrEmpty(_emailCartQueueName)) throw new ArgumentException("Could not load the name of the email queue");
        if (string.IsNullOrEmpty(_orderCreatedTopic)) throw new ArgumentException("Could not load the name of the order created queue");
        if (string.IsNullOrEmpty(_orderCreatedEmailSubscrption)) throw new ArgumentException("Could not load the name of the order created email subscription");

        var client = new ServiceBusClient(_connectionString);
        _emailCartProcessor = client.CreateProcessor(_emailCartQueueName);
        _emailOrderProcessor = client.CreateProcessor(_orderCreatedTopic, _orderCreatedEmailSubscrption);
        _emailService = emailService;
    }

    public async Task ListenAsync()
    {
        _emailCartProcessor.ProcessMessageAsync += ProcessEmailCartAsync;
        _emailCartProcessor.ProcessErrorAsync += ErrorHandler;

        _emailOrderProcessor.ProcessMessageAsync += ProcessEmailOrderAsync;
        _emailOrderProcessor.ProcessErrorAsync += ErrorHandler;

        await _emailCartProcessor.StartProcessingAsync();
        await _emailOrderProcessor.StartProcessingAsync();
    }

    public async Task StopListeningAsync()
    {
        await _emailCartProcessor.StopProcessingAsync();
        await _emailCartProcessor.DisposeAsync();

        await _emailOrderProcessor.StopProcessingAsync();
        await _emailOrderProcessor.DisposeAsync();
    }

    private async Task ProcessEmailOrderAsync(ProcessMessageEventArgs args)
    {
        try
        {
            RewardMessage rewardMessage = JsonConvert.DeserializeObject<RewardMessage>(Encoding.UTF8.GetString(args.Message.Body));
            await _emailService.LogOrderPlaced(rewardMessage);
            await args.CompleteMessageAsync(args.Message);
        }
        catch
        {
            throw;
        }
    }

    private async Task ProcessEmailCartAsync(ProcessMessageEventArgs args)
    {
        try
        {
            ShoppingCartDto cartDto = JsonConvert.DeserializeObject<ShoppingCartDto>(Encoding.UTF8.GetString(args.Message.Body));
            await _emailService.LogEmailAsync(cartDto);
            await args.CompleteMessageAsync(args.Message);
        }
        catch
        {
            throw;
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        return Task.CompletedTask;
    }
}