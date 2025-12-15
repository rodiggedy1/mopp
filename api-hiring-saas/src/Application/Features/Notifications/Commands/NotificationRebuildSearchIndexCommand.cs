using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Search;
using Application.Features.Notifications.Queries;
using Application.Features.Notifications.Search;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Notifications.Commands;

public sealed record NotificationRebuildSearchIndexCommand : ICommand;

public sealed class NotificationRebuildSearchIndexCommandHandler : ICommandHandler<NotificationRebuildSearchIndexCommand>
{
    private readonly ILogger<NotificationRebuildSearchIndexCommandHandler> _logger;
    private readonly ISearchClient<NotificationSearchable> _searchClient;
    private readonly ISearchIndexProvider _searchIndexProvider;
    private readonly ISender _mediatr;
    private readonly IMapper _mapper;

    public NotificationRebuildSearchIndexCommandHandler(
        ILogger<NotificationRebuildSearchIndexCommandHandler> logger,
        ISearchClient<NotificationSearchable> searchClient,
        ISearchIndexProvider searchIndexProvider,
        ISender mediatr,
        IMapper mapper)
    {
        _logger = logger;
        _searchClient = searchClient;
        _searchIndexProvider = searchIndexProvider;
        _mediatr = mediatr;
        _mapper = mapper;
    }
    public async Task Handle(NotificationRebuildSearchIndexCommand request, CancellationToken cancellationToken)
    {
        var index = _searchIndexProvider.GetIndex<NotificationSearchable>();

        try
        {
            await _searchClient.CreateIndexIfNotExist(index);
            _logger.LogInformation("Attempting to delete data for index: {0}", index);

            await _searchClient.DeleteAllAsync(cancellationToken);

            _logger.LogInformation("Delete finished for index: {0}", index);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting Elastic index {0}", index);
        }

        try
        {
            _logger.LogInformation("Attempting to index data for index: {0}", index);

            var notifications = await _mediatr.Send(new NotificationGetAllQuery());

            if (notifications.Any())
            {
                var searchableAccounts = _mapper.Map<IReadOnlyCollection<NotificationSearchable>>(notifications);
                await _searchClient.IndexAndRefreshManyAsync(searchableAccounts, cancellationToken);
                _logger.LogInformation("Indexing data finished for index: {0}", index);
            }
            else
            {
                _logger.LogInformation("No notifications to index...");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while reindexing Elastic index {0}", index);
        }
    }
}
