using AutoMapper;
using CountryWiki.DAL.v1;
using CountryWiki.Domain.DTO;

namespace CountryWiki.Web.Profiles;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<CountryCreateDto, CountryCreationRequest>();
        CreateMap<CountryReply, CountryReadDto>();
        CreateMap<CountryUpdateDto, CountryUpdateRequest>();
    }
}