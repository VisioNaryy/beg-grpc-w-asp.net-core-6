using AutoMapper;

namespace CountryService.DAL.EF.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly CountryContext _countryContext;
    private readonly IMapper _mapper;

    public CountryRepository(CountryContext countryContext, IMapper mapper)
    {
        _countryContext = countryContext;
        _mapper = mapper;
    }

    public async ValueTask<int> CreateAsync(CountryCreateDto countryCreateDto)
    {
        var country = _mapper.Map<Country>(countryCreateDto);

        await _countryContext.Countries.AddAsync(country);
        await _countryContext.SaveChangesAsync();

        return country.Id;
    }

    public async Task<int> UpdateAsync(CountryUpdateDto toCountryUpdate)
    {
        var country = await _countryContext.Countries.SingleOrDefaultAsync(x => x.Id == toCountryUpdate.Id);

        _mapper.Map(toCountryUpdate, country);

        return await _countryContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(int id)
    {
        var country = await _countryContext.Countries.SingleOrDefaultAsync(x => x.Id == id);

        if (country != null) _countryContext.Countries.Remove(country);

        return await _countryContext.SaveChangesAsync();
    }

    public async Task<CountryReadDto> GetAsync(int id)
    {
        var country = await _countryContext.Countries
            .AsNoTracking()
            .Include(c => c.CountryLanguages)
            .ThenInclude(cl => cl.Language)
            .SingleOrDefaultAsync(x => x.Id == id);

        return _mapper.Map<CountryReadDto>(country);
    }

    public async Task<IEnumerable<CountryReadDto>> GetAllAsync()
    {
        var countries = await _countryContext.Countries
            .AsNoTracking()
            .Include(c => c.CountryLanguages)
            .ThenInclude(cl => cl.Language)
            .Select(x => _mapper.Map<CountryReadDto>(x))
            .ToListAsync();

        return countries;
    }
}