using CountryService.Domain.DTO;

namespace CountryService.Domain.Services;

public interface ICountryService
{
    ValueTask<int> CreateAsync(CountryCreateDto countryToCreate);
    Task<bool> UpdateAsync(CountryUpdateDto countryToUpdate);
    Task<bool> DeleteAsync(int id);
    Task<CountryReadDto> GetAsync(int id);
    Task<IEnumerable<CountryReadDto>> GetAllAsync();
}