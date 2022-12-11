using AutoMapper;
using CountryService.Domain.DTO;
using CountryService.gRPC.v1;

namespace CountryService.gRPC.Profiles;

public class GrpcMapper : Profile
{
    public GrpcMapper()
    {
        CreateMap<CountryReadDto, CountryReply>();

        CreateMap<CountryUpdateRequest, CountryUpdateDto>()
            .ForMember(dest => dest.UpdateDate,
                opt =>
                    opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<CountryCreationRequest, CountryCreateDto>();
    }
}