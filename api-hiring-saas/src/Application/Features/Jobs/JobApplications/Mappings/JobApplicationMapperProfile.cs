using Application.Features.Jobs.JobApplications.Search;
using AutoMapper;
using Domain.Entities.JobApplications;
using DTO.Enums.Media;
using DTO.Job.JobApplication;

namespace Application.Features.Jobs.JobApplications.Mappings;

public sealed class JobApplicationMapperProfile : Profile
{
    public JobApplicationMapperProfile()
    {
        CreateMap<JobApplication, JobApplicationResponse>()
            .ForMember(s => s.ProfilePicture, opt => opt.MapFrom(s => s.Media.Items.Where(o => o.Type == MediaItemType.Image).FirstOrDefault()))
            .ForMember(s => s.ApplicationVideo, opt => opt.MapFrom(s => s.Media.Items.Where(o => o.Type == MediaItemType.Video).FirstOrDefault()));
        CreateMap<JobApplication, JobApplicationSearchable>()
            .ForMember(s => s.ProfilePicture, opt => opt.MapFrom(s => s.Media.Items.Where(o => o.Type == MediaItemType.Image).FirstOrDefault()))
            .ForMember(s => s.ApplicationVideo, opt => opt.MapFrom(s => s.Media.Items.Where(o => o.Type == MediaItemType.Video).FirstOrDefault()));

        CreateMap<JobApplicationResponse, JobApplicationSearchable>();
    }
}
