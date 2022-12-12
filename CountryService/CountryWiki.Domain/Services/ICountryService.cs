namespace CountryWiki.Domain.Services;

public interface ICountryService
{
    Task CreateAsync(IEnumerable<CountryCreateDto> countryToCreate);
    Task UpdateAsync(CountryUpdateDto countryToUpdate);
    Task DeleteAsync(int id);
    Task<CountryReadDto> GetAsync(int id);
    Task<IEnumerable<CountryReadDto>> GetAllAsync();
}