using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Search;
using Application.Features.Notifications.Queries;
using Application.Features.Notifications.Search;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Notifications.Commands;

public sealed record NotificationIndexCommand(int NotificationId) : ICommand;

public sealed class NotificationIndexCommandHandler : ICommandHandler<NotificationIndexCommand>
{
    private readonly ILogger<NotificationIndexCommandHandler> _logger;
    private readonly ISearchClient<NotificationSearchable> _searchClient;
    private readonly ISender _mediatr;
    private readonly IMapper _mapper;

    public NotificationIndexCommandHandler(
        ILogger<NotificationIndexCommandHandler> logger,
        ISearchClient<NotificationSearchable> searchClient,
        ISender mediatr,
        IMapper mapper)
    {
        _logger = logger;
        _searchClient = searchClient;
        _mediatr = mediatr;
        _mapper = mapper;
    }

    public async Task Handle(NotificationIndexCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to index data for notification with ID: {0}", command.NotificationId);
        var notificaiton = await _mediatr.Send(new NotificationGetByIdQuery(command.NotificationId));

        if (notificaiton != null)
        {
            try
            {
                await _searchClient.IndexAndRefreshAsync(_mapper.Map<NotificationSearchable>(notificaiton), cancellationToken);
                _logger.LogInformation("Indexing finished for notification with ID: {0}", command.NotificationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while reindexing data for notification with ID: {0}", command.NotificationId);
            }
        }
        else
        {
            _logger.LogInformation("Notification does not exist: {0}", command.NotificationId);
        }
    }
}
