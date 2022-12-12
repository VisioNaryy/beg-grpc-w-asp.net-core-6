namespace CountryWiki.BLL.Services;

public class CountryService : ICountryService
{
    private readonly ICountryRepository _countryRepository;
    private readonly ILogger<CountryService> _logger;

    public CountryService(ICountryRepository countryRepository, ILogger<CountryService> logger)
    {
        _countryRepository = countryRepository;
        _logger = logger;
    }
    
    public async Task CreateAsync(IEnumerable<CountryCreateDto> countriesToCreate)
    {
        await foreach (var countryToCreate in _countryRepository.CreateAsync(countriesToCreate)) 
        {
            _logger.LogDebug($"Country {countryToCreate.Name} has been created successfully with Id {countryToCreate.Id}");
        }
    }

    public async Task UpdateAsync(CountryUpdateDto countryToUpdate)
    {
        await _countryRepository.UpdateAsync(countryToUpdate);
        
        _logger.LogDebug($"Country with Id {countryToUpdate.Id} has been successfully updated");
    }

    public async Task DeleteAsync(int id)
    {
        await _countryRepository.DeleteAsync(id);
        
        _logger.LogDebug($"Country with Id {id} has been successfully deleted");
    }

    public async Task<CountryReadDto> GetAsync(int id)
    {
        return await _countryRepository.GetAsync(id);
    }

    public async Task<IEnumerable<CountryReadDto>> GetAllAsync()
    {
        var countries = new List<CountryReadDto>();

        await foreach (var country in _countryRepository.GetAllAsync())
        {
            countries.Add(country);
        }

        return countries;
    }
}