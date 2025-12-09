using Application.Features.Jobs.JobForms.Commands;
using DTO.MessageBroker.Messages.Search;
using MassTransit;
using MediatR;

namespace Worker.Consumers.Search;

public sealed class RebuildJobFormSearchIndexMessageConsumer : IConsumer<RebuildJobFormSearchIndexMessage>
{
    private readonly ISender _mediatr;

    public RebuildJobFormSearchIndexMessageConsumer(ISender mediatr)
    {
        _mediatr = mediatr;
    }

    public async Task Consume(ConsumeContext<RebuildJobFormSearchIndexMessage> context)
    {
        await _mediatr.Send(new JobFormRebuildSearchIndexCommand());
    }
}