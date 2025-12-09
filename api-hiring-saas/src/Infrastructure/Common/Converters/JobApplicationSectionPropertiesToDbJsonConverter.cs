using Application.Common.Extensions;
using Domain.Entities.JobApplicationSectionProperties;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Common.Converters;

public class JobApplicationSectionPropertiesToDbJsonConverter : ValueConverter<List<JobApplicationSectionProperty>, string>
{
    private static readonly JsonSerializerOptions Settings = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly Expression<Func<List<JobApplicationSectionProperty>, string>> ConvertToExpr = x => ConvertTo(x);
    private static readonly Expression<Func<string, List<JobApplicationSectionProperty>>> ConvertFromExpr = x => ConvertFrom(x);

    public JobApplicationSectionPropertiesToDbJsonConverter()
        : base(ConvertToExpr, ConvertFromExpr)
    {
    }

    private static string ConvertTo(List<JobApplicationSectionProperty> properties) => properties.ToJson(Settings);

    private static List<JobApplicationSectionProperty> ConvertFrom(string json) => json.Deserialize<List<JobApplicationSectionProperty>>()!;
}
