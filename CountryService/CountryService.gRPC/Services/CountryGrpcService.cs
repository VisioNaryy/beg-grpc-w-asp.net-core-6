using AutoMapper;
using CountryService.Domain.DTO;
using CountryService.Domain.Services;
using CountryService.gRPC.v1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace CountryService.gRPC.Services;

public class CountryGrpcService : CountryGrpc.CountryGrpcBase
{
    private readonly ICountryService _countryService;
    private readonly IMapper _mapper;

    public CountryGrpcService(ICountryService countryService, IMapper mapper)
    {
        _countryService = countryService;
        _mapper = mapper;
    }

    public override async Task GetAll(Empty request, IServerStreamWriter<CountryReply> responseStream,
        ServerCallContext context)
    {
        var countries = await _countryService.GetAllAsync();

        foreach (var country in countries)
        {
            await responseStream.WriteAsync(_mapper.Map<CountryReply>(country));
        }

        await Task.CompletedTask;
    }

    public override async Task<CountryReply> Get(CountryIdRequest request, ServerCallContext context)
    {
        var country = await _countryService.GetAsync(request.Id);

        if (country == null)
            throw new RpcException(new Status(StatusCode.NotFound, $"Country with Id {request.Id} hasn't been found"));

        return _mapper.Map<CountryReply>(country);
    }

    public override async Task<Empty> Update(CountryUpdateRequest request, ServerCallContext context)
    {
        var countryToUpdate = _mapper.Map<CountryUpdateDto>(request);

        var updateSucceded = await _countryService.UpdateAsync(countryToUpdate);

        if (!updateSucceded)
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Country with Id {request.Id} hasn't been updated, it have probably been deleted"));

        return new Empty();
    }

    public override async Task<Empty> Delete(CountryIdRequest request, ServerCallContext context)
    {
        var deleteSucceed = await _countryService.DeleteAsync(request.Id);

        if (!deleteSucceed)
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Country with Id {request.Id} hasn't been deleted"));

        return new Empty();
    }

    public override async Task Create(IAsyncStreamReader<CountryCreationRequest> requestStream,
        IServerStreamWriter<CountryCreationReply> responseStream, ServerCallContext context)
    {
        await foreach (var countryToCreate in requestStream.ReadAllAsync())
        {
            var createdCountryId = await _countryService.CreateAsync(_mapper.Map<CountryCreateDto>(countryToCreate));

            var countryCreationReply = new CountryCreationReply
            {
                Id = createdCountryId,
                Name = countryToCreate.Name
            };
            
            await responseStream.WriteAsync(countryCreationReply);
        }
    }
}