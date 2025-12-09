using Application.Features.Jobs.JobForms.Commands;
using DTO.MessageBroker.Messages.Search;
using MassTransit;
using MediatR;

namespace Worker.Consumers.Search;

public sealed class IndexJobFormMessageConsumer : IConsumer<IndexJobFormMessage>
{
    private readonly ISender _mediatr;

    public IndexJobFormMessageConsumer(ISender mediatr)
    {
        _mediatr = mediatr;
    }

    public async Task Consume(ConsumeContext<IndexJobFormMessage> context)
    {
        await _mediatr.Send(new JobFormIndexCommand(context.Message.JobFormId));
    }
}