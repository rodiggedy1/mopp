using Application.Common.Search;
using DTO.Job.JobApplication.Search;

namespace Application.Features.Jobs.JobApplications.Search;

public interface IJobApplicationFullSearchCriteria : IFullSearchCriteria<JobApplicationFullSearchSortField>
{
    public int? JobDetailsCreatedById { get; }
    public int? StatusId { get; }
    public int? JobDetailsId { get; }
    public List<int>? Status { get; }
    public DateTime? DateFrom { get; }
    public DateTime? DateTo { get; }
}
