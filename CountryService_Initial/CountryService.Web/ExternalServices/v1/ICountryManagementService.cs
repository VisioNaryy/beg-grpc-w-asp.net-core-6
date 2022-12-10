using CountryServices.v1;

namespace CountryService.Web.ExternalServices.v1;

public interface ICountryManagementService
{
    Task<IEnumerable<CountryReply>> GetAllAsync();
    Task<CountryReply> GetAsync(CountryIdRequest country);
    Task DeleteAsync(IEnumerable<CountryIdRequest> countries);
    Task UpdateAsync(CountryUpdateRequest country);
    Task<IEnumerable<CountryCreationReply>> CreateAsync(List<CountryCreationRequest> countries);
}