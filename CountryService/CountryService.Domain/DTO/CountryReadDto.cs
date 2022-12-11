namespace CountryService.Domain.DTO;

public record CountryReadDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string FlagUri { get; init; }
    public string CapitalCity { get; init; }
    public string Anthem { get; init; }
    public ICollection<string> Languages { get; init; }
}