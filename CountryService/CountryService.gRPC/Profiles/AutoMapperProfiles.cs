using AutoMapper;
using CountryService.DAL.EF.Models;
using CountryService.Domain.DTO;

namespace CountryService.gRPC.Profiles;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<CountryCreateDto, Country>()
            .ForMember(dest => dest.CountryLanguages,
                opt =>
                    opt.MapFrom(src => src.Languages.Select(x => new CountryLanguage { LanguageId = x}).ToList()));

        CreateMap<CountryUpdateDto, Country>()
            .ForMember(dest => dest.UpdateDate,
                opt =>
                    opt.MapFrom(src => DateTime.UtcNow));
        
        CreateMap<Country, CountryReadDto>()
            .ForMember(dest => dest.Languages,
                opt => 
                    opt.MapFrom(src => src.CountryLanguages.Select(x => x.Language.Name).ToList()));
    }
}