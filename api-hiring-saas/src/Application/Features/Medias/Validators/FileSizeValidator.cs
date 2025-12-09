using Application.Common.Extensions;
using Application.Common.Localization.Extensions;
using Application.Common.Validation;
using ByteSizeLib;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Application.Features.Medias.Validators;

public sealed record FileSizeValidatorData(ByteSize FileSize, ByteSize MaxFileSize)
{
    public static FileSizeValidatorData FromFile(IFormFile file, ByteSize maxFileSize) => new(file.GetSize(), maxFileSize);
}

public sealed class FileSizeValidator : AbstractValidator<FileSizeValidatorData>
{
    public FileSizeValidator()
    {
        RuleFor(d => d)
            .Must(d => d.FileSize <= d.MaxFileSize)
            .WithLocalizationKey("fileSizeValidator.message", d => new object[] { d.FileSize.ToString("KB"), d.MaxFileSize.ToString("KB") });
    }
}

public class FileSizeExceedError : FluentValidationError
{
    public override HttpStatusCode? StatusCode => HttpStatusCode.BadRequest;

    public FileSizeExceedError(ByteSize fileSize, ByteSize maxFileSize)
        : base($"File size: {fileSize.ToString("KB")} exceed limit of {maxFileSize.ToString("KB")}")
    {
    }
}
