using Application.Common.Search;
using DTO.Job.JobForm;

namespace Application.Features.Jobs.JobForms.Search;

public sealed record JobFormSearchable : JobFormResponse, ISearchable
{
}
