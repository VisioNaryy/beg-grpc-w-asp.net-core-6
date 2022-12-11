namespace CountryService.Domain.DTO;

public record CountryCreateDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string FlagUri { get; set; }
    public string CapitalCity { get; set; }
    public string Anthem { get; set; }
    public DateTime CreatedDate { get; set; }
    public ICollection<int> Languages { get; set; }
}