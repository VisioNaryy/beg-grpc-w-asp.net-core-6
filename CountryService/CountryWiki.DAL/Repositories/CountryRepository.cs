using AutoMapper;
using CountryWiki.DAL.v1;
using CountryWiki.Domain.DTO;
using CountryWiki.Domain.Repositories;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace CountryWiki.DAL.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly CountryGrpc.CountryGrpcClient _countryGrpcClient;
    private readonly IMapper _mapper;

    public CountryRepository(CountryGrpc.CountryGrpcClient countryGrpcClient, IMapper mapper)
    {
        _countryGrpcClient = countryGrpcClient;
        _mapper = mapper;
    }


    public async IAsyncEnumerable<CountryCreatedDto> CreateAsync(IEnumerable<CountryCreateDto> countriesToCreate)
    {
        using (var bidirectionalStreamingCall = _countryGrpcClient.Create())
        {
            foreach (var countryToCreate in countriesToCreate)
            {
                var countryCreationRequest = _mapper.Map<CountryCreationRequest>(countryToCreate);

                await bidirectionalStreamingCall.RequestStream.WriteAsync(countryCreationRequest);
            }
            
            // Tells server that request streaming is done
            await bidirectionalStreamingCall.RequestStream.CompleteAsync();

            while (await bidirectionalStreamingCall.ResponseStream.MoveNext())
            {
                var country = bidirectionalStreamingCall.ResponseStream.Current;

                yield return new CountryCreatedDto
                {
                    Id = country.Id,
                    Name = country.Name
                };
            }
        }
    }

    public async Task UpdateAsync(CountryUpdateDto countryToUpdate)
    {
        var countryUpdateRequest = _mapper.Map<CountryUpdateRequest>(countryToUpdate);

        await _countryGrpcClient.UpdateAsync(countryUpdateRequest);
    }

    public async Task DeleteAsync(int id)
    {
        var request = new CountryIdRequest {Id = id};

        await _countryGrpcClient.DeleteAsync(request);
    }

    public async Task<CountryReadDto> GetAsync(int id)
    {
        var request = new CountryIdRequest {Id = id};

        var country = await _countryGrpcClient.GetAsync(request);
        
        return _mapper.Map<CountryReadDto>(country);
    }

    public async IAsyncEnumerable<CountryReadDto> GetAllAsync()
    {
        using (var streamingCall = _countryGrpcClient.GetAll(new Empty()))
        {
            while (await streamingCall.ResponseStream.MoveNext())
            {
                var country = streamingCall.ResponseStream.Current;

                yield return _mapper.Map<CountryReadDto>(country);
            }
        }
    }
}