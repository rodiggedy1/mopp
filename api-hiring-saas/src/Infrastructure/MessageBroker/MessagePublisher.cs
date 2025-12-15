using Application.Common.MessageBroker;
using DTO.MessageBroker;
using MassTransit;

namespace Infrastructure.MessageBroker;

public class MessagePublisher : IMessagePublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MessagePublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellation = default)
        where TMessage : IMessage
        => _publishEndpoint.Publish(message, cancellation);

    public Task PublishAsync(object message, Type type, CancellationToken cancellation = default)
        => _publishEndpoint.Publish(message, type, cancellation);
}
