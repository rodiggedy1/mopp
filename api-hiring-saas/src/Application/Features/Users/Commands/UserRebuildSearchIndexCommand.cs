using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Search;
using Application.Features.Users.Queries;
using Application.Features.Users.Search;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands;

public sealed record UserRebuildSearchIndexCommand : ICommand;

public sealed class UserRebuildSearchIndexCommandHandler : ICommandHandler<UserRebuildSearchIndexCommand>
{
    private readonly ILogger<UserRebuildSearchIndexCommandHandler> _logger;
    private readonly ISearchClient<UserSearchable> _searchClient;
    private readonly ISearchIndexProvider _searchIndexProvider;
    private readonly ISender _mediatr;
    private readonly IMapper _mapper;

    public UserRebuildSearchIndexCommandHandler(
        ILogger<UserRebuildSearchIndexCommandHandler> logger,
        ISearchClient<UserSearchable> searchClient,
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
    public async Task Handle(UserRebuildSearchIndexCommand request, CancellationToken cancellationToken)
    {
        var index = _searchIndexProvider.GetIndex<UserSearchable>();

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
            var users = await _mediatr.Send(new UserGetAllQuery());

            _logger.LogInformation("Attempting to index data for index: {0} with user count {1}", index, users.Count);
            if (users.Any())
            {
                var searchableUsers = _mapper.Map<IReadOnlyCollection<UserSearchable>>(users);
                await _searchClient.IndexAndRefreshManyAsync(searchableUsers, cancellationToken);
                _logger.LogInformation("Indexing data finished for index: {0}", index);
            }
            else
            {
                _logger.LogInformation("No users to index...");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while reindexing Elastic index {0}", index);
        }
    }
}
