namespace CountryWiki.Domain.DTO;

public record CountryUploadedFileDto
{
    public string FileName { get; init; }
    public string ContentType { get; init; }
}