using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Search;
using Application.Features.Jobs.JobApplications.Search;
using DTO.Job.JobApplication.Search;
using DTO.Pagination;
using DTO.Sorting;
using DTO.User;

namespace Application.Features.Jobs.JobApplications.Queries;

public sealed record JobApplicationFullSearchQuery(
    string? Query,
    int? StatusId,
    int? JobDetailsCreatedById,
    int? JobDetailsId,
    List<int>? Status,
    DateTime? DateFrom,
    DateTime? DateTo,
    PaginationOptions Paging,
    SortOptions<JobApplicationFullSearchSortField>? Sorting) : IJobApplicationFullSearchCriteria, IQuery<PaginatedList<JobApplicationSearchable>>;

public sealed class JobApplicationFullSearchQueryHandler : IQueryHandler<JobApplicationFullSearchQuery, PaginatedList<JobApplicationSearchable>>
{
    private readonly ISearchClient<JobApplicationSearchable> _searchClient;
    private readonly ICurrentUserService _currentUserService;
    public JobApplicationFullSearchQueryHandler(
        ISearchClient<JobApplicationSearchable> searchClient,
        ICurrentUserService currentUserService)
    {
        _searchClient = searchClient;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedList<JobApplicationSearchable>> Handle(JobApplicationFullSearchQuery query, CancellationToken cancellationToken)
    {
        return await _searchClient.SearchJobApplicationsAsync(query with { JobDetailsCreatedById = _currentUserService.UserId});
    }
}