using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Request;
using Application.Common.Search;
using Application.Features.Users.Search;
using DTO.Pagination;
using DTO.Sorting;
using DTO.User.Search;

namespace Application.Features.Users.Queries;

public sealed record UserFullSearchQuery(
    string? Query,
    int? Status,
    PaginationOptions Paging,
    SortOptions<UserFullSearchSortField>? Sorting) : IUserFullSearchCriteria, IQuery<PaginatedList<UserSearchable>>;

public sealed class UserFullSearchQueryHandler : IQueryHandler<UserFullSearchQuery, PaginatedList<UserSearchable>>
{
    private readonly ISearchClient<UserSearchable> _searchClient;

    public UserFullSearchQueryHandler(ISearchClient<UserSearchable> searchClient)
    {
        _searchClient = searchClient;
    }
    public async Task<PaginatedList<UserSearchable>> Handle(UserFullSearchQuery query, CancellationToken cancellationToken)
    {
        return await _searchClient.SearchUsersAsync(query);
    }
}
