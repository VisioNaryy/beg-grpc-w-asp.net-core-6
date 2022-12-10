using CountryService.Domain.DTO;

namespace CountryService.Domain.Repositories;

public interface ICountryRepository
{
    ValueTask<int> CreateAsync(CountryCreateDto toCountryCreate);
    Task<int> UpdateAsync(CountryUpdateDto toCountryUpdate);
    Task<int> DeleteAsync(int id);
    Task<CountryReadDto> GetAsync(int id);
    Task<IEnumerable<CountryReadDto>> GetAllAsync();
}
