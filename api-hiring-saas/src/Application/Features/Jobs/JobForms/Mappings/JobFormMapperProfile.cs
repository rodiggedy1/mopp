using Application.Features.Jobs.JobForms.Search;
using AutoMapper;
using Domain.Entities.JobForms;
using DTO.Job.JobForm;

namespace Application.Features.Jobs.JobForms.Mappings;

public sealed class JobFormMapperProfile : Profile
{
    public JobFormMapperProfile()
    {
        CreateMap<JobForm, JobFormResponse>();

        CreateMap<JobForm, JobFormSearchable>();

        CreateMap<JobFormResponse, JobFormSearchable>();
    }
}
