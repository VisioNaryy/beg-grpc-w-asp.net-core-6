using CountryWiki.Domain.DTO;
using CountryWiki.Domain.Services;
using CountryWiki.Web.Channels;
using CountryWiki.Web.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CountryWiki.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ICountryService _countryService;
    private readonly ICountryFileUploadValidatorService _countryFileUploadValidatorService;
    private readonly ISyncCountriesChannel _syncCountriesChannel;
    
    public GlobalOptions GlobalOptions;
    public IEnumerable<CountryReadDto> Countries { get; set; } = new List<CountryReadDto>();
    public string UploadErrorMessage { get; set; } = string.Empty;
    [BindProperty] public IFormFile Upload { get; set; }

    public IndexModel(ILogger<IndexModel> logger, ICountryService countryService,
        ICountryFileUploadValidatorService countryFileUploadValidatorService,
        ISyncCountriesChannel syncCountriesChannel,
        GlobalOptions globalOptions)
    {
        _logger = logger;
        _countryService = countryService;
        _countryFileUploadValidatorService = countryFileUploadValidatorService;
        _syncCountriesChannel = syncCountriesChannel;
        GlobalOptions = globalOptions;
    }

    public async Task OnGetAsync()
    {
        Countries = await _countryService.GetAllAsync();
    }

    public async Task<IActionResult> OnPostUploadAsync(CancellationToken cancellationToken)
    {
        if (Upload == null)
        {
            return await HandleFileValidation("File is missing");
        }

        var uploadedFile = new CountryUploadedFileDto
        {
            FileName = Upload.FileName,
            ContentType = Upload.ContentType
        };

        if (!_countryFileUploadValidatorService.ValidateFile(uploadedFile))
        {
            return await HandleFileValidation("Only JSON files are allowed");
        }

        var parsedCountries = await _countryFileUploadValidatorService.ParseFile(Upload.OpenReadStream());

        if (parsedCountries == null || !parsedCountries.Any())
        {
            return await HandleFileValidation("Cannot parse the file or the file is empty");
        }

        await _syncCountriesChannel.SyncAsync(parsedCountries, cancellationToken);

        return RedirectToPage("./Index");
    }
    
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _countryService.DeleteAsync(id);
        
        return RedirectToPage("./Index");
    }

    private async Task<PageResult> HandleFileValidation(string errorMessage)
    {
        UploadErrorMessage = errorMessage;
        Countries = await _countryService.GetAllAsync();
        
        return Page();
    }
}