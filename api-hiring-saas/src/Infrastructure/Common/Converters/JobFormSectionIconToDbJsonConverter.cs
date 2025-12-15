using Application.Common.Extensions;
using Domain.Entities.JobFormSectionIcons;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Common.Converters;

public class JobFormSectionIconToDbJsonConverter : ValueConverter<JobFormSectionIcon, string>
{
    private static readonly JsonSerializerOptions Settings = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly Expression<Func<JobFormSectionIcon, string>> ConvertToExpr = x => ConvertTo(x);
    private static readonly Expression<Func<string, JobFormSectionIcon>> ConvertFromExpr = x => ConvertFrom(x);

    public JobFormSectionIconToDbJsonConverter()
        : base(ConvertToExpr, ConvertFromExpr)
    {
    }

    private static string ConvertTo(JobFormSectionIcon icon) => icon.ToJson(Settings);

    private static JobFormSectionIcon ConvertFrom(string json) => json.Deserialize<JobFormSectionIcon>()!;
}
