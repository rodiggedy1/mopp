using DTO.Pagination;
using DTO.Sorting;

namespace Application.Common.Search;

public interface IFullSearchCriteria<TSort> : ISearchCriteria
    where TSort : Enum
{
    PaginationOptions Paging { get; }
    SortOptions<TSort>? Sorting { get; }
}
