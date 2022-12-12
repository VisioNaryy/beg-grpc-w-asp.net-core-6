namespace CountryWiki.Domain.Repositories;

public interface ICountryRepository
{
    IAsyncEnumerable<CountryCreatedDto> CreateAsync(IEnumerable<CountryCreateDto> countriesToCreate);
    Task UpdateAsync(CountryUpdateDto countryToUpdate);
    Task DeleteAsync(int id);
    Task<CountryReadDto> GetAsync(int id);
    IAsyncEnumerable<CountryReadDto> GetAllAsync();
}