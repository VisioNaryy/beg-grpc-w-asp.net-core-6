using CountryWiki.Domain.Services;
using CountryWiki.Web.Channels;
using CountryWiki.Web.Options;
using Grpc.Core;

namespace CountryWiki.Web.BackgroundServices;

public class SyncUploadedCountriesBackgroundService : BackgroundService
{
    private readonly ILogger<SyncUploadedCountriesBackgroundService> _logger;
    private readonly ISyncCountriesChannel _channel;
    private readonly IServiceProvider _serviceProvider;
    private readonly GlobalOptions _globalOptions;

    public SyncUploadedCountriesBackgroundService(ILogger<SyncUploadedCountriesBackgroundService> logger, 
        ISyncCountriesChannel channel, IServiceProvider serviceProvider, GlobalOptions globalOptions)
    {
        _logger = logger;
        _channel = channel;
        _serviceProvider = serviceProvider;
        _globalOptions = globalOptions;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await foreach (var uploadedCountries in _channel.ReadAllAsync(cancellationToken))
        {
            try
            {
                _logger.LogInformation("Received uploaded countries from the channel for sync");

                // Because countryService has a scoped lifecycle and background service is a singleton
                using var scope = _serviceProvider.CreateScope();
                var countryService = scope.ServiceProvider.GetRequiredService<ICountryService>();

                try
                {
                    // Processing sync
                    _globalOptions.ProcessingUpload = true;
                    await countryService.CreateAsync(uploadedCountries);
                }
                catch (RpcException e)
                {
                    var correlationId = e.Trailers.GetValue("correlationId");

                    _logger.LogError(e, "Background synchronization has failed. CorrelationId {correlationId}",
                        correlationId);
                }
                finally
                {
                    _globalOptions.ProcessingUpload = false;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to manage uploaded countries");
            }
            finally
            {
                _globalOptions.ProcessingUpload = false;
            }
        }
    }
}