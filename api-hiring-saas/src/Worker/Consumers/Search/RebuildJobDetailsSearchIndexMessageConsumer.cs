using Application.Features.Jobs.JobsDetails.Commands;
using DTO.MessageBroker.Messages.Search;
using MassTransit;
using MediatR;

namespace Worker.Consumers.Search;

public sealed class RebuildJobDetailsSearchIndexMessageConsumer : IConsumer<RebuildJobDetailsSearchIndexMessage>
{
    private readonly ISender _mediatr;

    public RebuildJobDetailsSearchIndexMessageConsumer(ISender mediatr)
    {
       _mediatr = mediatr;
    }

    public async Task Consume(ConsumeContext<RebuildJobDetailsSearchIndexMessage> context)
    {
        await _mediatr.Send(new JobDetailsRebuildSearchIndexCommand());
    }
}
