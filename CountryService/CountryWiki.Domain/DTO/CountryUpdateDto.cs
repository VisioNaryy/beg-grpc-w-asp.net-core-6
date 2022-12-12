using System.ComponentModel.DataAnnotations;

namespace CountryWiki.Domain.DTO;

public record CountryUpdateDto
{
    public int Id { get; init; }
    
    [Required, StringLength(200, MinimumLength = 10)]
    public string Description { get; init; }
}