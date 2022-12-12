namespace CountryWiki.Domain.DTO;

public record CountryUpdateDto
{
    public int Id { get; init; }
    public string Description { get; init; }
}