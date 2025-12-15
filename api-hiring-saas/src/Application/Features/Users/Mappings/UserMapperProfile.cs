using Application.Features.Users.Commands;
using Application.Features.Users.Search;
using AutoMapper;
using Domain.Entities.User;
using DTO.User;

namespace Application.Features.Users.Mappings;

public sealed class UserMapperProfile : Profile
{
    public UserMapperProfile()
    {
        CreateMap<ApplicationUser, UserResponse>()
            .ForMember(d => d.DateCreated, opt => opt.MapFrom(s => s.Created))
            .ForMember(d => d.Picture, opt => opt.MapFrom(s => s.Media.GetMainImageUrl()));

        CreateMap<ApplicationUser, UserBaseResponse>()
            .ForMember(d => d.Picture, opt => opt.MapFrom(s => s.Media.GetMainImageUrl()));

        CreateMap<ApplicationUser, UserInfoResponse>()
            .ForPath(d => d.CalendlyDetails.ClientId, opt => opt.MapFrom(s => s.CalendlyClientId))
            .ForPath(d => d.CalendlyDetails.ClientSecret, opt => opt.MapFrom(s => s.CalendlyClientSecret))
            .ForPath(d => d.CalendlyDetails.RedirectUri, opt => opt.MapFrom(s => s.CalendlyRedirectUri))
            .ForPath(d => d.CalendlyDetails.AccessToken, opt => opt.MapFrom(s => s.CalendlyAccessToken))
            .ForPath(d => d.CalendlyDetails.Code, opt => opt.MapFrom(s => s.CalendlyCode))
            .ForMember(d => d.DateCreated, opt => opt.MapFrom(s => s.Created))
            .ForMember(d => d.Picture, opt => opt.MapFrom(s => s.Media.GetMainImageUrl()))
            .ForMember(d => d.Type, opt => opt.Ignore());

        CreateMap<ApplicationUser, MeResponse>()
            .ForPath(d => d.CalendlyDetails.ClientId, opt => opt.MapFrom(s => s.CalendlyClientId))
            .ForPath(d => d.CalendlyDetails.ClientSecret, opt => opt.MapFrom(s => s.CalendlyClientSecret))
            .ForPath(d => d.CalendlyDetails.RedirectUri, opt => opt.MapFrom(s => s.CalendlyRedirectUri))
            .ForPath(d => d.CalendlyDetails.AccessToken, opt => opt.MapFrom(s => s.CalendlyAccessToken))
            .ForPath(d => d.CalendlyDetails.Code, opt => opt.MapFrom(s => s.CalendlyCode))
            .ForPath(d => d.CalendlyDetails.RefreshToken, opt => opt.MapFrom(s => s.CalendlyRefreshToken))
            .ForPath(d => d.CalendlyDetails.TokenExpiresAt, opt => opt.MapFrom(s => s.CalendlyTokenExpiresAt))
            .ForMember(d => d.ProfilePicture, opt => opt.MapFrom(s => s.Media.GetMainImageUrl()))
            .ForMember(d => d.Picture, opt => opt.MapFrom(s => s.Media.GetMainImageUrl()))
            .ForMember(d => d.DateCreated, opt => opt.MapFrom(s => s.Created));

        CreateMap<UserUpdateRequest, UserUpdateCommand>();

        CreateMap<UserResponse, UserSearchable>();

        CreateMap<UserInfoResponse, UserSearchable>();
    }
}
