using Application.Features.Jobs.JobApplications.Commands;
using DTO.MessageBroker.Messages.Search;
using MassTransit;
using MediatR;

namespace Worker.Consumers.Search;

public sealed class RebuildJobApplicationSearchIndexMessageConsumer : IConsumer<RebuildJobApplicationSearchIndexMessage>
{
    private readonly ISender _mediatr;

    public RebuildJobApplicationSearchIndexMessageConsumer(ISender mediatr)
    {
        _mediatr = mediatr;
    }

    public async Task Consume(ConsumeContext<RebuildJobApplicationSearchIndexMessage> context)
    {
        await _mediatr.Send(new JobApplicationRebuildSearchIndexCommand());
    }
}
