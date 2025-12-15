namespace DTO.MessageBroker;

public interface IMessage
{
}

public abstract record MessageBase : IMessage
{
    public Guid Id => Guid.NewGuid();
}