using DTO.MessageBroker;

namespace Application.Common.MessageBroker;

public interface IMessagePublisher
{
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellation = default)
        where TMessage : IMessage;

    Task PublishAsync(object message, Type type, CancellationToken cancellation = default);
}
