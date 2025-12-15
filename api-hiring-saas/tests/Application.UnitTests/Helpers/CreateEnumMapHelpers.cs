using AutoMapper;
using DTO.Response;

namespace Application.UnitTests.Helpers;

public class CreateEnumMapHelpers
{
    public static void CreateEnumMap<TEnum>(IMapperConfigurationExpression cfg) where TEnum : Enum
    {
        cfg.CreateMap<TEnum, ListItemBaseResponse>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => Convert.ToInt32(s)))
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.ToString()));
    }
}
