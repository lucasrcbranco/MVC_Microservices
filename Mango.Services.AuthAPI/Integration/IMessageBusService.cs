namespace AuthAPI.Integration;

public interface IMessageBusService
{
    Task PublishAsync(string topicOrQueueName, object message);
}