using Application.Common.Search;
using DTO.Job.JobForm.Search;

namespace Application.Features.Jobs.JobForms.Search;

public interface IJobFormFullSearchCriteria : IFullSearchCriteria<JobFormFullSearchSortField>
{
    public int? JobDetailsCreatedById { get; }
}