namespace CountryService.DAL.EF.Contexts;

public class CountryContext : DbContext
{
    public DbSet<Country> Countries { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<CountryLanguage> CountryLanguage { get; set; }
    
    public CountryContext(DbContextOptions<CountryContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var countryContextConfiguration = new CountryContextConfiguration();

        modelBuilder.ApplyConfiguration<Language>(countryContextConfiguration);
        modelBuilder.ApplyConfiguration<CountryLanguage>(countryContextConfiguration);
    }
}