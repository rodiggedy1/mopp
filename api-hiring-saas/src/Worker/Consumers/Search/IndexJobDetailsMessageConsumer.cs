using Application.Features.Jobs.JobsDetails.Commands;
using DTO.MessageBroker.Messages.Search;
using MassTransit;
using MediatR;

namespace Worker.Consumers.Search;

public sealed class IndexJobDetailsMessageConsumer : IConsumer<IndexJobDetailsMessage>
{
    private readonly ISender _mediatr;

    public IndexJobDetailsMessageConsumer(ISender mediatr)
    {
        _mediatr = mediatr;
    }

    public async Task Consume(ConsumeContext<IndexJobDetailsMessage> context)
    {
        await _mediatr.Send(new JobDetailsIndexCommand(context.Message.JobDetailsId));
    }
}
