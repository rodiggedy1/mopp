using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Search;
using Application.Features.Jobs.JobForms.Search;
using DTO.Job.JobForm.Search;
using DTO.Pagination;
using DTO.Sorting;

namespace Application.Features.Jobs.JobForms.Queries;

public sealed record JobFormFullSearchQuery(
    string? Query,
    int? JobDetailsCreatedById,
    PaginationOptions Paging,
    SortOptions<JobFormFullSearchSortField>? Sorting) : IJobFormFullSearchCriteria, IQuery<PaginatedList<JobFormSearchable>>;

public sealed class JobFormFullSearchQueryHandler : IQueryHandler<JobFormFullSearchQuery, PaginatedList<JobFormSearchable>>
{
    private readonly ISearchClient<JobFormSearchable> _searchClient;
    private readonly ICurrentUserService _currentUserService;

    public JobFormFullSearchQueryHandler(
        ISearchClient<JobFormSearchable> searchClient,
        ICurrentUserService currentUserService)
    {
        _searchClient = searchClient;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedList<JobFormSearchable>> Handle(JobFormFullSearchQuery query, CancellationToken cancellationToken)
    {
        var result = await _searchClient.SearchJobFormsAsync(query with { JobDetailsCreatedById = _currentUserService.UserId });

        var sortedItems = result.Items
            .Select(item => new JobFormSearchable
            {
                Id = item.Id,
                Title = item.Title,
                UniqueHash = item.UniqueHash,
                Description = item.Description,
                CreatedBy = item.CreatedBy,
                JobFormSections = item.JobFormSections?.OrderBy(s => s.Position).ToList()
            })
            .ToList();

        return new PaginatedList<JobFormSearchable>(
            sortedItems,
            result.TotalCount,
            result.PageNumber,
            query.Paging.PageSize
        );
    }
}