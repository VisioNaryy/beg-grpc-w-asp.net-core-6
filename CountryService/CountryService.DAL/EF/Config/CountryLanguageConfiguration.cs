using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CountryService.DAL.EF.Config;

public class CountryContextConfiguration : IEntityTypeConfiguration<CountryLanguage>, IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<CountryLanguage> builder)
    {
        builder.HasKey(x => new {x.CountryId, x.LanguageId});
    }

    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.HasData(
            new Language {Id = 1, Name = "English"},
            new Language {Id = 2, Name = "French"},
            new Language {Id = 3, Name = "Spanish"}
            );
    }
}