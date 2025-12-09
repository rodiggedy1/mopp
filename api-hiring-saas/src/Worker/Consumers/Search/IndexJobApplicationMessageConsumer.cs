using Application.Features.Jobs.JobApplications.Commands;
using DTO.MessageBroker.Messages.Search;
using MassTransit;
using MediatR;

namespace Worker.Consumers.Search;

public sealed class IndexJobApplicationMessageConsumer : IConsumer<IndexJobApplicationMessage>
{
    private readonly ISender _mediatr;

    public IndexJobApplicationMessageConsumer(ISender mediatr)
    {
        _mediatr = mediatr;
    }

    public async Task Consume(ConsumeContext<IndexJobApplicationMessage> context)
    {
        await _mediatr.Send(new JobApplicationIndexCommand(context.Message.JobApplicationId));
    }
}
