using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace ShoppingCartAPI.Integration;

public class MessageBusService : IMessageBusService
{
    private readonly IConfiguration _configuration;
    private string _connectionString;

    public MessageBusService(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = configuration.GetConnectionString("ServiceBusConnection") ?? string.Empty;

        if (string.IsNullOrEmpty(_connectionString)) throw new ArgumentException("Could not load the connection string for the message bus");
    }

    public async Task PublishAsync(string topicOrQeueName, object message)
    {
        await using var client = new ServiceBusClient(_connectionString);
        ServiceBusSender sender = client.CreateSender(topicOrQeueName);

        string jsonMessage = JsonConvert.SerializeObject(message);
        await sender.SendMessageAsync(new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage)) { CorrelationId = Guid.NewGuid().ToString() });
        await client.DisposeAsync();
    }
}