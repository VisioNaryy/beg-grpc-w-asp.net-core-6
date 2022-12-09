using System.Diagnostics;
using CountryService.Web.ExternalServices;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace CountryService.Web.Services;

public class CountryService : Country.CountryBase
{
    private readonly ICountryManagementService _countryManagementService;
    private readonly ILogger<CountryService> _logger;

    public CountryService(ICountryManagementService countryManagementService, ILogger<CountryService> logger)
    {
        _countryManagementService = countryManagementService;
        _logger = logger;
    }

    public override async Task GetAll(Empty request, IServerStreamWriter<CountryReply> responseStream,
        ServerCallContext context)
    {
        // Streams all found countries to the client
        var countries = await _countryManagementService.GetAllAsync();
        
        foreach (var country in countries)
        {
            await responseStream.WriteAsync(country);
        }
        
        await Task.CompletedTask;
    }

    public override async Task<CountryReply> Get(CountryIdRequest request, ServerCallContext context)
    {
        return await _countryManagementService.GetAsync(request);
    }

    public override async Task<Empty> Delete(IAsyncStreamReader<CountryIdRequest> requestStream,
        ServerCallContext context)
    {
        var countryIdRequestList = new List<CountryIdRequest>();

        await foreach (var countryIdRequest in requestStream.ReadAllAsync())
        {
            countryIdRequestList.Add(countryIdRequest);
        }

        // Delete in one shot all streamed countries
        await _countryManagementService.DeleteAsync(countryIdRequestList);

        return new Empty();
    }

    public override async Task<Empty> Update(CountryUpdateRequest request, ServerCallContext context)
    {
        await _countryManagementService.UpdateAsync(request);

        return new Empty();
    }

    public override async Task Create(IAsyncStreamReader<CountryCreationRequest> requestStream,
        IServerStreamWriter<CountryCreationReply> responseStream, ServerCallContext context)
    {
        var countryCreationRequestList = new List<CountryCreationRequest>();

        await foreach (var countryCreationRequest in requestStream.ReadAllAsync())
        {
            countryCreationRequestList.Add(countryCreationRequest);
        }

        var createdCountries = await _countryManagementService.CreateAsync(countryCreationRequestList);

        foreach (var createdCountry in createdCountries)
        {
            await responseStream.WriteAsync(createdCountry);
        }
    }
}