using MassTransit;

namespace Infrastructure.MessageBroker.Filters;

public class FillConsumeContextAccessorConsumeFilter<T> : IFilter<ConsumeContext<T>>
where T : class
{
    private readonly IConsumeContextAccessor _consumeContextAccessor;

    public FillConsumeContextAccessorConsumeFilter(IConsumeContextAccessor consumeContextAccessor)
    {
        _consumeContextAccessor = consumeContextAccessor;
    }

    public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        _consumeContextAccessor.ConsumeContext = context;
        return next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
    }
}