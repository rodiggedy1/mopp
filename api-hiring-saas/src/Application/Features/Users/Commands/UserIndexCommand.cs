using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Search;
using Application.Features.Users.Queries;
using Application.Features.Users.Search;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands;

public sealed record UserIndexCommand(int UserId) : ICommand;

public sealed record UserIndexCommandHandler : ICommandHandler<UserIndexCommand>
{
    private readonly ILogger<UserIndexCommandHandler> _logger;
    private readonly ISearchClient<UserSearchable> _searchClient;
    private readonly ISender _mediatr;
    private readonly IMapper _mapper;

    public UserIndexCommandHandler(
        ILogger<UserIndexCommandHandler> logger,
        ISearchClient<UserSearchable> searchClient,
        ISender mediatr, 
        IMapper mapper)
    {
        _logger = logger;
        _mediatr = mediatr;
        _searchClient = searchClient;
        _mapper = mapper;
    }

    public async Task Handle(UserIndexCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to index data for user with ID: {0}", command.UserId);
        var user = await _mediatr.Send(new UserGetQuery(command.UserId));

        if (user != null)
        {
            try
            {
                await _searchClient.IndexAndRefreshAsync(_mapper.Map<UserSearchable>(user), cancellationToken);
                _logger.LogInformation("Indexing finished for user with ID: {0}", command.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while reindexing data for user with ID: {0}", command.UserId);
            }
        }
        else
        {
            _logger.LogInformation("User does not exist: {0}", command.UserId);
        }
    }
}
