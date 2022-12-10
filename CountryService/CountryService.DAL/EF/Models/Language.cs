namespace CountryService.DAL.EF.Models;

public class Language
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Country> Countries { get; }
}