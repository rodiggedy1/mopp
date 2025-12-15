using Application.Features.Jobs.JobsDetails.Search;
using AutoMapper;
using Domain.Entities.JobsDetails;
using DTO.Job.JobDetails;

namespace Application.Features.Jobs.JobsDetails.Mappings;

public sealed class JobDetailsMapperProfile : Profile
{
    public JobDetailsMapperProfile()
    {
        CreateMap<JobDetails, JobDetailsResponse>();
        CreateMap<JobDetails, JobDetailsBaseResponse>();
        CreateMap<JobDetails, JobDetailsSearchable>();

        CreateMap<JobDetailsSearchable, JobDetailsBaseResponse>();

        CreateMap<JobDetailsResponse, JobDetailsSearchable>();
    }
}
