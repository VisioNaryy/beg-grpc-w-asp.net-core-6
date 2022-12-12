namespace CountryWiki.BLL.Services;

public class CountryFileUploadValidatorService : ICountryFileUploadValidatorService
{
    public CountryFileUploadValidatorService()
    {
    }

    public bool ValidateFile(CountryUploadedFileDto countryUploadedFile)
    {
        return countryUploadedFile.FileName.ToLower().EndsWith(".json") &&
               countryUploadedFile.ContentType == "application/json";
    }

    public async Task<IEnumerable<CountryCreateDto>> ParseFile(Stream content)
    {
        try
        {
            var parsedCountries = await JsonSerializer.DeserializeAsync<IEnumerable<CountryCreateDto>>(content, new
                JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return parsedCountries.Any(x => string.IsNullOrEmpty(x.Name) ||
                                            string.IsNullOrEmpty(x.Anthem) ||
                                            string.IsNullOrEmpty(x.Description) ||
                                            string.IsNullOrEmpty(x.FlagUri) ||
                                            string.IsNullOrEmpty(x.CapitalCity) ||
                                            x.Languages == null ||
                                            !x.Languages.Any())
                ? null
                : parsedCountries;
        }
        catch
        {
            return null;
        }
    }
}