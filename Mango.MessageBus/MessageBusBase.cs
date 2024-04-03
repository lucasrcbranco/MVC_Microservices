using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace Mango.MessageBus;

public class MessageBus : IMessageBus
{
    private const string ConnectionString = "Endpoint=sb://lucasbus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=7m+m6qupw0ydzjnJm7KM1yxGSUHYAJDpF+ASbLcfz50=";
    public async Task PublishAsync(string topicOrQeueName, object message)
    {
        await using var client = new ServiceBusClient(ConnectionString);
        ServiceBusSender sender = client.CreateSender(topicOrQeueName);

        string jsonMessage = JsonConvert.SerializeObject(message);
        await sender.SendMessageAsync(new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage)) { CorrelationId = Guid.NewGuid().ToString() });
        await client.DisposeAsync();
    }
}