using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository.Base;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using Application.Features.Validators;
using Domain.Entities.JobForms;
using Domain.Entities.JobsDetails;
using Domain.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Jobs.JobsDetails.Commands;

public sealed record JobDetailsCreateCommand(
    string Title,
    string Description,
    string? Location,
    string EmploymentType,
    int Price,
    string UniqueHash,
    int JobFormId) : IJobDetailsInsertData, ICommand;

public sealed class JobDetailsCreateCommandHandler : ICommandHandler<JobDetailsCreateCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEncryption _encryption;

    public JobDetailsCreateCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IEncryption encryption)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _encryption = encryption;
    }

    public async Task Handle(JobDetailsCreateCommand command, CancellationToken cancellationToken)
    {
        var uniqueHash = command.UniqueHash;
        if (string.IsNullOrEmpty(command.UniqueHash))
        {
            do
            {
                uniqueHash = _encryption.GenerateUniqueHashAsync();
            }
            while(await _dbContext.JobDetails.AnyAsync(j => j.UniqueHash == uniqueHash, cancellationToken));
        }
        
        JobDetails newJobDetails = JobDetails.Create(
            command with { UniqueHash = uniqueHash });

        await _dbContext.JobDetails.AddAsync(newJobDetails, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class JobDetailsCreateCommandValidator : AbstractValidator<JobDetailsCreateCommand>
{
    public JobDetailsCreateCommandValidator(
        IRepository<JobForm> jobFormRepository,
        ILocalizationService localizationService)
    {
        RuleFor(cmd => cmd.Title)
            .NotNull();

        RuleFor(cmd => cmd.Description)
            .NotNull();

        RuleFor(cmd => cmd.JobFormId)
            .NotNull()
            .DependentRules(
                  () =>
                  {
                      RuleFor(x => new EntityExistsValidatorData(x.JobFormId))
                        .SetValidator(new EntityExistsValidator<JobForm>(jobFormRepository, localizationService))
                        .OverridePropertyName(nameof(JobDetailsCreateCommand.JobFormId));
                  });
    }
}