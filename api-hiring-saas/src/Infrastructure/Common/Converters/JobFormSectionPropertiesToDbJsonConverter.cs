using Application.Common.Extensions;
using Domain.Entities.JobFormSectionProperties;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Common.Converters;

public class JobFormSectionPropertiesToDbJsonConverter : ValueConverter<List<JobFormSectionProperty>, string>
{
    private static readonly JsonSerializerOptions Settings = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly Expression<Func<List<JobFormSectionProperty>, string>> ConvertToExpr = x => ConvertTo(x);
    private static readonly Expression<Func<string, List<JobFormSectionProperty>>> ConvertFromExpr = x => ConvertFrom(x);

    public JobFormSectionPropertiesToDbJsonConverter()
        : base(ConvertToExpr, ConvertFromExpr)
    {
    }

    private static string ConvertTo(List<JobFormSectionProperty> properties) => properties.ToJson(Settings);

    private static List<JobFormSectionProperty> ConvertFrom(string json) => json.Deserialize<List<JobFormSectionProperty>>()!;
}
