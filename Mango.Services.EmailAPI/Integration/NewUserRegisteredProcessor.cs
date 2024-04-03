using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace EmailAPI.Integration;

public class NewUserRegisteredProcessor : INewUserRegisteredProcessor
{
    private readonly string _connectionString;
    private readonly string _newUserRegisteredQueueName;
    private readonly EmailService _emailService;

    private readonly ServiceBusProcessor _newUserRegisteredProcessor;

    public NewUserRegisteredProcessor(IConfiguration configuration, EmailService emailService)
    {
        _connectionString = configuration.GetConnectionString("ServiceBusConnection") ?? string.Empty;
        _newUserRegisteredQueueName = configuration.GetValue<string>("TopicAndQueueNames:NewUserRegisteredQueue") ?? string.Empty;

        if (string.IsNullOrEmpty(_connectionString)) throw new ArgumentException("Could not load the connection string for the message bus");
        if (string.IsNullOrEmpty(_newUserRegisteredQueueName)) throw new ArgumentException("Could not load the name of the new user registered queue");

        var client = new ServiceBusClient(_connectionString);
        _newUserRegisteredProcessor = client.CreateProcessor(_newUserRegisteredQueueName);
        _newUserRegisteredProcessor = client.CreateProcessor(_newUserRegisteredQueueName);
        _emailService = emailService;
    }

    public async Task ListenAsync()
    {
        _newUserRegisteredProcessor.ProcessMessageAsync += ProcessNewUserRegisteredAsync;
        _newUserRegisteredProcessor.ProcessErrorAsync += ErrorHandler;
        await _newUserRegisteredProcessor.StartProcessingAsync();
    }

    public async Task StopListeningAsync()
    {
        await _newUserRegisteredProcessor.StopProcessingAsync();
        await _newUserRegisteredProcessor.DisposeAsync();
    }

    private async Task ProcessNewUserRegisteredAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var newUserRegisteredEmail = JsonConvert.DeserializeObject<string>(Encoding.UTF8.GetString(args.Message.Body));
            await _emailService.LogNewUserRegisteredAsync(newUserRegisteredEmail);
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