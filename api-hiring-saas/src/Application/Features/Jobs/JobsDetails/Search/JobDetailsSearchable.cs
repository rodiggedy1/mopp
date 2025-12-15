using Application.Common.Search;
using DTO.Job.JobDetails;

namespace Application.Features.Jobs.JobsDetails.Search;

public sealed record JobDetailsSearchable : JobDetailsResponse, ISearchable
{
}
