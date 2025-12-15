using Domain.Entities.JobFormSections;
using Domain.Entities.JobFormSectionProperties;
using Infrastructure.Common.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Infrastructure.Persistence.EntityConfigurations;
public class JobFormSectionConfiguration : IEntityTypeConfiguration<JobFormSection>
{
    public void Configure(EntityTypeBuilder<JobFormSection> builder)
    {
        builder.Property(e => e.Icon)
            .HasConversion<JobFormSectionIconToDbJsonConverter>();

        var valueComparer = new ValueComparer<List<JobFormSectionProperty>>(
            (c1, c2) => CompareCollections(c1, c2),
            c => GetCollectionHashCode(c),
            c => CloneCollection(c));

        builder.Property(e => e.JobFormSectionProperties)
            .HasConversion<JobFormSectionPropertiesToDbJsonConverter>()
            .Metadata.SetValueComparer(valueComparer);
    }

    private static bool CompareCollections(List<JobFormSectionProperty>? c1, List<JobFormSectionProperty>? c2)
    {
        if (c1 == null && c2 == null) return true;
        if (c1 == null || c2 == null) return false;
        
        var json1 = JsonSerializer.Serialize(c1, (JsonSerializerOptions?)null);
        var json2 = JsonSerializer.Serialize(c2, (JsonSerializerOptions?)null);
        return json1 == json2;
    }

    private static int GetCollectionHashCode(List<JobFormSectionProperty>? c)
    {
        if (c == null) return 0;
        var json = JsonSerializer.Serialize(c, (JsonSerializerOptions?)null);
        return json.GetHashCode();
    }

    private static List<JobFormSectionProperty> CloneCollection(List<JobFormSectionProperty>? c)
    {
        if (c == null) return new List<JobFormSectionProperty>();
        var json = JsonSerializer.Serialize(c, (JsonSerializerOptions?)null);
        return JsonSerializer.Deserialize<List<JobFormSectionProperty>>(json, (JsonSerializerOptions?)null) ?? new List<JobFormSectionProperty>();
    }
}