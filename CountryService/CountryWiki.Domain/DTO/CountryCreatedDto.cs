namespace CountryWiki.Domain.DTO;

public record CountryCreatedDto
{
    public int Id { get; init; }
    public string Name { get; init; }
}