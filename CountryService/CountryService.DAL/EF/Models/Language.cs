using System.ComponentModel.DataAnnotations.Schema;

namespace CountryService.DAL.EF.Models;

public class Language
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<CountryLanguage> CountryLanguages { get; }
}