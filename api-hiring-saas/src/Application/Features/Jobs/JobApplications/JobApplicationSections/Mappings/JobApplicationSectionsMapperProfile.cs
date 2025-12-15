using Application.Features.Jobs.JobApplications.JobApplicationSections.Commands;
using AutoMapper;
using Domain.Entities.JobApplicationSectionIcons;
using Domain.Entities.JobApplicationSectionProperties;
using Domain.Entities.JobApplicationSections;
using DTO.Job.JobApplication.JobApplicationSection;

namespace Application.Features.Jobs.JobApplications.JobApplicationSections.Mappings;

public sealed class JobApplicationSectionsMapperProfile : Profile
{
    public JobApplicationSectionsMapperProfile()
    {
        CreateMap<JobApplicationSectionCreateRequest, JobApplicationSectionCreateCommand>()
            .ConstructUsing((src, context) => new JobApplicationSectionCreateCommand(
                src.Name,
                src.Description,
                src.Code,
                src.Position,
                default,
                default));

        CreateMap<JobApplicationSection, JobApplicationSectionResponse>();

        CreateMap<JobApplicationSectionIconCreateRequest, JobApplicationSectionIcon>();
        CreateMap<JobApplicationSectionIcon, JobApplicationSectionIconResponse>();

        CreateMap<JobApplicationSectionPropertyCreateRequest, JobApplicationSectionProperty>();
        CreateMap<JobApplicationSectionProperty, JobApplicationSectionPropertyResponse>();

        CreateMap<JobApplicationSectionPropertyValidationCreateRequest, JobApplicationSectionPropertyValidation>();
        CreateMap<JobApplicationSectionPropertyValidation, JobApplicationSectionPropertyValidationResponse>();
    }
}

