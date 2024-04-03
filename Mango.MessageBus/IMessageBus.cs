namespace Mango.MessageBus;

public interface IMessageBus
{
    Task PublishAsync(string topicOrQeueName, object message);
}