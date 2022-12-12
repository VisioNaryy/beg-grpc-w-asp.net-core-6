using CountryWiki.Domain.DTO;

namespace CountryWiki.Web.Channels;

public interface ISyncCountriesChannel
{
    IAsyncEnumerable<IEnumerable<CountryCreateDto>> ReadAllAsync(CancellationToken cancellationToken);
    Task<bool> SyncAsync(IEnumerable<CountryCreateDto> countriesToCreate, CancellationToken cancellationToken);
}