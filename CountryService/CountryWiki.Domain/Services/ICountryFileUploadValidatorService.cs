namespace CountryWiki.Domain.Services;

public interface ICountryFileUploadValidatorService
{
    bool ValidateFile(CountryUploadedFileDto countryUploadedFile);
    Task<IEnumerable<CountryCreateDto>> ParseFile(Stream content);
}