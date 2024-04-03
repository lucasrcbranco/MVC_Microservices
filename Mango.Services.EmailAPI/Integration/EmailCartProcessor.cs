using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Models.Dtos;
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace EmailAPI.Integration;

public class EmailCartProcessor : IEmailCartProcessor
{
    private readonly string _connectionString;
    private readonly string _emailCartQueueName;
    private readonly EmailService _emailService;

    private readonly ServiceBusProcessor _emailCartProcessor;

    public EmailCartProcessor(IConfiguration configuration, EmailService emailService)
    {
        _connectionString = configuration.GetConnectionString("ServiceBusConnection") ?? string.Empty;
        _emailCartQueueName = configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue") ?? string.Empty;

        if (string.IsNullOrEmpty(_connectionString)) throw new ArgumentException("Could not load the connection string for the message bus");
        if (string.IsNullOrEmpty(_emailCartQueueName)) throw new ArgumentException("Could not load the name of the email queue");

        var client = new ServiceBusClient(_connectionString);
        _emailCartProcessor = client.CreateProcessor(_emailCartQueueName);
        _emailService = emailService;
    }

    public async Task ListenAsync()
    {
        _emailCartProcessor.ProcessMessageAsync += ProcessEmailCartAsync;
        _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
        await _emailCartProcessor.StartProcessingAsync();
    }

    public async Task StopListeningAsync()
    {
        await _emailCartProcessor.StopProcessingAsync();
        await _emailCartProcessor.DisposeAsync();
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