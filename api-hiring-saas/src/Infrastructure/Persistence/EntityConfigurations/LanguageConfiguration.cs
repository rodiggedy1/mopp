using Domain.Entities.Languages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.Property(e => e.Name)
            .HasMaxLength(50);

        builder.Property(e => e.Code)
            .HasMaxLength(2);

        builder.Property(e => e.CultureCode)
            .HasMaxLength(5);
    }
}
