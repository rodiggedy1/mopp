using Application.Common.Search;
using DTO.Job.JobApplication;

namespace Application.Features.Jobs.JobApplications.Search;

public sealed record JobApplicationSearchable : JobApplicationResponse, ISearchable
{
}
