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
    
    public async ValueTask<int> CreateAsync(CountryCreateDto toCountryCreate)
    {
        var country = _mapper.Map<Country>(toCountryCreate);
        
        await _countryContext.Countries.AddAsync(country);
        await _countryContext.SaveChangesAsync();
        
        return country.Id;
    }

    public async Task<int> UpdateAsync(CountryUpdateDto toCountryUpdate)
    {
        var country = await _countryContext.Countries.SingleOrDefaultAsync(x => x.Id == toCountryUpdate.Id);
        
        if (country == null) { throw new Exception("Unable to find the country"); }
        
        _mapper.Map(toCountryUpdate, country);
        
        return await _countryContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(int id)
    {
        var country = await _countryContext.Countries.SingleOrDefaultAsync(x => x.Id == id);

        if (country == null) { throw new Exception("Unable to find the country"); }

        _countryContext.Countries.Remove(country);
        
        return await _countryContext.SaveChangesAsync(); 
    }

    public async Task<CountryReadDto> GetAsync(int id)
    {
        var country = await _countryContext.Countries.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);

        if (country == null) { throw new Exception("Unable to find the country"); }

        return _mapper.Map<CountryReadDto>(country);
    }

    public async Task<IEnumerable<CountryReadDto>> GetAllAsync()
    {
        var countries = await _countryContext.Countries.AsNoTracking().Select(x => _mapper.Map<CountryReadDto>(x)).ToListAsync();

        return countries;
    }
}