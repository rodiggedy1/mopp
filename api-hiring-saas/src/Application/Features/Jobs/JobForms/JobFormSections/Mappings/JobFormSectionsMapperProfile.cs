using Application.Features.Jobs.JobFormSections.JobFormSectionSections.Commands;
using AutoMapper;
using Domain.Entities.JobFormSectionIcons;
using Domain.Entities.JobFormSectionProperties;
using Domain.Entities.JobFormSections;
using DTO.Job.JobForm.JobFormSection;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Jobs.JobForms.JobFormSections.Mappings;

public sealed class JobFormSectionsMapperProfile : Profile
{
    public JobFormSectionsMapperProfile()
    {
        CreateMap<JobFormSectionCreateRequest, JobFormSectionCreateCommand>()
            .ConstructUsing((src, context) => new JobFormSectionCreateCommand(
                src.Name,
                src.Description,
                src.Code,
                src.Position,
                default,
                default));

        CreateMap<JobFormSection, JobFormSectionResponse>();

        CreateMap<JobFormSectionIconCreateRequest, JobFormSectionIcon>();
        CreateMap<JobFormSectionIcon, JobFormSectionIconResponse>();

        CreateMap<JobFormSectionPropertyCreateRequest, JobFormSectionProperty>();
        CreateMap<JobFormSectionProperty, JobFormSectionPropertyResponse>();

        CreateMap<JobFormSectionPropertyValidationCreateRequest, JobFormSectionPropertyValidation>();
        CreateMap<JobFormSectionPropertyValidation, JobFormSectionPropertyValidationResponse>();
    }
}
