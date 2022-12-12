using System.Threading.Channels;
using CountryWiki.Domain.DTO;

namespace CountryWiki.Web.Channels;

public class SyncCountriesChannel : ISyncCountriesChannel
{
    private readonly Channel<IEnumerable<CountryCreateDto>> _channel;
    private readonly ILogger<SyncCountriesChannel> _logger;

    public SyncCountriesChannel(ILogger<SyncCountriesChannel> logger)
    {
        var options = new UnboundedChannelOptions
        {
            SingleWriter = false,
            SingleReader = true
        };

        _channel = Channel.CreateUnbounded<IEnumerable<CountryCreateDto>>(options);
        _logger = logger;
    }

    public IAsyncEnumerable<IEnumerable<CountryCreateDto>> ReadAllAsync(CancellationToken cancellationToken)
        => _channel.Reader.ReadAllAsync(cancellationToken);
    
    public async Task<bool> SyncAsync(IEnumerable<CountryCreateDto> countriesToCreate,
        CancellationToken cancellationToken)
    {
        while (await _channel.Writer.WaitToWriteAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
        {
            if (_channel.Writer.TryWrite(countriesToCreate))
            {
                _logger.LogDebug("Sending parsed countries to the background task");
                return true;
            }
        }

        return false;
    }
}