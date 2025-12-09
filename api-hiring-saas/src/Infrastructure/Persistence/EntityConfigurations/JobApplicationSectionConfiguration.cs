using Domain.Entities.JobApplicationSectionProperties;
using Domain.Entities.JobApplicationSections;
using Infrastructure.Common.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Infrastructure.Persistence.EntityConfigurations;

public class JobApplicationSectionConfiguration : IEntityTypeConfiguration<JobApplicationSection>
{
    public void Configure(EntityTypeBuilder<JobApplicationSection> builder)
    {
        builder.Property(e => e.Icon)
            .HasConversion<JobApplicationSectionIconToDbJsonConverter>();

        var valueComparer = new ValueComparer<List<JobApplicationSectionProperty>>(
            (c1, c2) => CompareCollections(c1, c2),
            c => GetCollectionHashCode(c),
            c => CloneCollection(c));

        builder.Property(e => e.JobApplicationSectionProperties)
            .HasConversion<JobApplicationSectionPropertiesToDbJsonConverter>()
            .Metadata.SetValueComparer(valueComparer);
    }

    private static bool CompareCollections(List<JobApplicationSectionProperty>? c1, List<JobApplicationSectionProperty>? c2)
    {
        if (c1 == null && c2 == null) return true;
        if (c1 == null || c2 == null) return false;

        var json1 = JsonSerializer.Serialize(c1, (JsonSerializerOptions?)null);
        var json2 = JsonSerializer.Serialize(c2, (JsonSerializerOptions?)null);
        return json1 == json2;
    }

    private static int GetCollectionHashCode(List<JobApplicationSectionProperty>? c)
    {
        if (c == null) return 0;
        var json = JsonSerializer.Serialize(c, (JsonSerializerOptions?)null);
        return json.GetHashCode();
    }

    private static List<JobApplicationSectionProperty> CloneCollection(List<JobApplicationSectionProperty>? c)
    {
        if (c == null) return new List<JobApplicationSectionProperty>();
        var json = JsonSerializer.Serialize(c, (JsonSerializerOptions?)null);
        return JsonSerializer.Deserialize<List<JobApplicationSectionProperty>>(json, (JsonSerializerOptions?)null) ?? new List<JobApplicationSectionProperty>();
    }
}