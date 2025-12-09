using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Search;
using Application.Features.Notifications.Search;
using AutoMapper;
using DTO.Enums.Notification;
using DTO.Notification.Search;
using DTO.Pagination;
using DTO.Sorting;

namespace Application.Features.Notifications.Queries;

public sealed record NotificationFullSearchForUserQuery(
    string? Query,
    NotificationStatus? Status,
    PaginationOptions Paging,
    SortOptions<NotificationFullSearchSortField>? Sorting) : INotificationFullSearchCriteria, IQuery<PaginatedList<NotificationSearchable>>;

public sealed class NotificationFullSearchForUserQueryHandler : IQueryHandler<NotificationFullSearchForUserQuery, PaginatedList<NotificationSearchable>>
{
    private readonly ISearchClient<NotificationSearchable> _searchClient;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public NotificationFullSearchForUserQueryHandler(
        ISearchClient<NotificationSearchable> searchClient,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _searchClient = searchClient;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<PaginatedList<NotificationSearchable>> Handle(NotificationFullSearchForUserQuery query, CancellationToken cancellationToken)
    {
        var request = _mapper.Map<NotificationForUserFullSearchRequest>(query);

        var result = await _searchClient.SearchNotificationsForUserAsync(request with { UserId = (int)_currentUserService.UserId! });

        return new PaginatedList<NotificationSearchable>(
            _mapper.Map<IReadOnlyCollection<NotificationSearchable>>(result.Items),
            result.TotalCount,
            result.PageNumber,
            query.Paging.PageSize);
    }
}
