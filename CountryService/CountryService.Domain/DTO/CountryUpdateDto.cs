namespace CountryService.Domain.DTO;

public record CountryUpdateDto
{
    public int Id { get; set; }
    public string Description { get; set; }
    public DateTime UpdateDate { get; set; }
}