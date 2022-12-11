using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CountryService.DAL.EF.Config;

public class CountryContextConfiguration : IEntityTypeConfiguration<Language>, IEntityTypeConfiguration<CountryLanguage>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.HasData(
            new Language {Id = 1, Name = "English"},
            new Language {Id = 2, Name = "French"},
            new Language {Id = 3, Name = "Spanish"}
            );
    }

    public void Configure(EntityTypeBuilder<CountryLanguage> builder)
    {
        builder
            .HasKey(t => new { t.CountryId, t.LanguageId });

        builder
            .HasOne(cl => cl.Country)
            .WithMany(c => c.CountryLanguages)
            .HasForeignKey(cl => cl.CountryId);

        builder
            .HasOne(cl => cl.Language)
            .WithMany(c => c.CountryLanguages)
            .HasForeignKey(cl => cl.LanguageId);
    }
}