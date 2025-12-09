using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository.Base;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using Application.Common.MediaStorage;
using Application.Common.Validation;
using Application.Features.Medias.Validators;
using Application.Features.Validators;
using Domain.Entities.JobApplications;
using Domain.Entities.Medias;
using Domain.Interfaces;
using DTO.Enums.Job.JobApplication;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Application.Features.Jobs.JobApplications.Commands;

public sealed record JobApplicationFileUploadCommand(
    int JobApplicationId,
    IFormFile File,
    JobApplicationFileType FileType) : ICommand;

public sealed record JobApplicationFileUploadCommandHandler : ICommandHandler<JobApplicationFileUploadCommand>
{
    private readonly IMediaStorage _mediaStorage;
    private readonly IRepository<JobApplication> _jobApplicationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public JobApplicationFileUploadCommandHandler(
        IMediaStorage mediaStorage,
        IRepository<JobApplication> jobApplicationRepository,
        IUnitOfWork unitOfWork)
    {
        _mediaStorage = mediaStorage;
        _jobApplicationRepository = jobApplicationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(JobApplicationFileUploadCommand command, CancellationToken cancellationToken)
    {
        var jobApplication = await _jobApplicationRepository.GetSafeAsync(command.JobApplicationId, cancellationToken);


        await jobApplication.UploadFile(new MediaCreateData(command.File, false, GetSortOrder(command.FileType)), _mediaStorage, false);

        _jobApplicationRepository.Update(jobApplication);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private int GetSortOrder(JobApplicationFileType fileType)
    {
        var sortOrder = fileType switch
        {
            JobApplicationFileType.ProfilePicture => 1,
            JobApplicationFileType.ApplicationVideo => 2,
            _ => 0
        };

        return sortOrder;
    }
}

public sealed class JobApplicationFileUploadCommandValidator : BaseAbstractValidator<JobApplicationFileUploadCommand>
{
    public JobApplicationFileUploadCommandValidator(
        IOptions<MediaConfig> mediaConfigOpt,
        IRepository<JobApplication> jobApplicationRepository,
        ILocalizationService localizationService,
        FileExtensionValidator fileExtensionValidator,
        FileSizeValidator fileSizeValidator)
    {
        var mediaConfig = mediaConfigOpt.Value;

        RuleFor(x => x.JobApplicationId)
            .NotNull()
            .DependentRules(
                  () =>
                  {
                      RuleFor(x => new EntityExistsValidatorData(x.JobApplicationId))
                        .SetValidator(new EntityExistsValidator<JobApplication>(jobApplicationRepository, localizationService))
                        .OverridePropertyName(nameof(JobApplicationFileUploadCommand.JobApplicationId));
                  });

        RuleFor(cmd => FileSizeValidatorData.FromFile(cmd.File, mediaConfig.MaxFileSize))
            .SetValidator(fileSizeValidator)
            .OverridePropertyName(nameof(JobApplicationFileUploadCommand.File));


        RuleFor(cmd => FileExtensionValidatorData.FromFile(cmd.File, mediaConfig.AllowedExtensions))
            .SetValidator(fileExtensionValidator)
            .OverridePropertyName(nameof(JobApplicationFileUploadCommand.File));
    }
}