using MassTransit;
namespace Infrastructure.MessageBroker;

public interface IConsumeContextAccessor
{
    ConsumeContext? ConsumeContext { get; set; }
}

public class ConsumeContextAccessor : IConsumeContextAccessor
{
    public ConsumeContext? ConsumeContext { get; set; }
}