using Application.Common.Extensions;
using Domain.Entities.JobApplicationSectionIcons;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Common.Converters;

public class JobApplicationSectionIconToDbJsonConverter : ValueConverter<JobApplicationSectionIcon, string>
{
    private static readonly JsonSerializerOptions Settings = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly Expression<Func<JobApplicationSectionIcon, string>> ConvertToExpr = x => ConvertTo(x);
    private static readonly Expression<Func<string, JobApplicationSectionIcon>> ConvertFromExpr = x => ConvertFrom(x);

    public JobApplicationSectionIconToDbJsonConverter()
        : base(ConvertToExpr, ConvertFromExpr)
    {
    }

    private static string ConvertTo(JobApplicationSectionIcon icon) => icon.ToJson(Settings);

    private static JobApplicationSectionIcon ConvertFrom(string json) => json.Deserialize<JobApplicationSectionIcon>()!;
}
