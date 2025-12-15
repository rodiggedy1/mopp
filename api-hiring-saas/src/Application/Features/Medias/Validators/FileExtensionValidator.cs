using Application.Common.Extensions;
using Application.Common.Localization.Extensions;
using Application.Common.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Application.Features.Medias.Validators;

public sealed record FileExtensionValidatorData(string? Extension, IReadOnlyCollection<string> AllowedExtensions)
{
    public static FileExtensionValidatorData FromFile(IFormFile file, IEnumerable<string> allowedExtensions) =>
        new(file.GetExtension(), allowedExtensions.AsArray());
}

public sealed class FileExtensionValidator : AbstractValidator<FileExtensionValidatorData>
{
    public FileExtensionValidator()
    {
        RuleFor(d => d)
            .Must(d => d.AllowedExtensions.Contains(d.Extension))
            .When(d => d.AllowedExtensions.HasValue())
            .WithLocalizationKey("disallowedFileExtensionValidator.message", d => new object[] { d.Extension });
    }
}

public class FileExtensionDisallowedError : FluentValidationError
{
    public override HttpStatusCode? StatusCode => HttpStatusCode.BadRequest;

    public FileExtensionDisallowedError(string? extension)
        : base($"File extension: {extension} is disallowed")
    {
    }
}
