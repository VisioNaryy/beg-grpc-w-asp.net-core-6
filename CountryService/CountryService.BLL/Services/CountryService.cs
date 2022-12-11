using CountryService.DAL.EF.Models;

namespace CountryService.BLL.Services;

public class CountryService : ICountryService
{
    private readonly ICountryRepository _countryRepository;

    public CountryService(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }

    public async ValueTask<int> CreateAsync(CountryCreateDto countryToCreate)
        => await _countryRepository.CreateAsync(countryToCreate);

    public async Task<bool> UpdateAsync(CountryUpdateDto countryToUpdate)
        => await _countryRepository.UpdateAsync(countryToUpdate) > 0;

    public async Task<bool> DeleteAsync(int id)
        => await _countryRepository.DeleteAsync(id) > 0;

    public async Task<CountryReadDto> GetAsync(int id)
        => await _countryRepository.GetAsync(id);

    public async Task<IEnumerable<CountryReadDto>> GetAllAsync()
        => await _countryRepository.GetAllAsync();
}