using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository.Base;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using Application.Features.Validators;
using Domain.Entities.JobApplications;
using Domain.Interfaces;
using DTO.Enums.Job.JobApplication;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Jobs.JobApplications.Commands;

public sealed record JobApplicationStatusChangeCommand(
    int JobApplicationId,
    JobApplicationStatus Status,
    int? KibanaSortOrder) : ICommand;

public sealed class JobApplicationStatusChangeCommandHandler : ICommandHandler<JobApplicationStatusChangeCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTime;
    public JobApplicationStatusChangeCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IDateTime dateTime)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
    }

    public async Task Handle(JobApplicationStatusChangeCommand command, CancellationToken cancellationToken)
    {
        var jobApplication = await _dbContext.JobApplication
            .FirstAsync(s => s.Id == command.JobApplicationId, cancellationToken);

        jobApplication.ChangeStatus(
            command.Status,
            command.KibanaSortOrder ?? await GetLatestSortOrderForStatus(command.Status, cancellationToken),
            _dateTime);

        await UpdateOtherApplicationsSortOrders(command.Status, jobApplication, cancellationToken);

        _dbContext.JobApplication.Update(jobApplication);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<int> GetLatestSortOrderForStatus(
        JobApplicationStatus status, 
        CancellationToken cancellationToken = default)
    {
        var kanbanNext = await _dbContext.JobApplication
            .Where(s => s.Status == status)
            .DefaultIfEmpty()
            .MaxAsync(s => s != null ? s.KanbanSortOrder : 0, cancellationToken) + 1;

        return (int)kanbanNext;
    }

    private async Task UpdateOtherApplicationsSortOrders(
        JobApplicationStatus status, 
        JobApplication jobApplication,
        CancellationToken cancellationToken = default)
    {
        var needOfUpdate = await _dbContext.JobApplication.AnyAsync(s => s.Id != jobApplication.Id &&
                                                                    s.KanbanSortOrder == jobApplication.KanbanSortOrder,
                                                                    cancellationToken);

        if (needOfUpdate)
        {
            var jobApplications = await _dbContext.JobApplication
                .Where(s => s.Id != jobApplication.Id &&
                            s.Status == status &&
                            s.KanbanSortOrder >= jobApplication.KanbanSortOrder)
                .ToListAsync(cancellationToken);

            foreach(var jobApplicationToUpdate in jobApplications)
            {
                jobApplicationToUpdate.ChangeKanbanSortOrder((int)jobApplicationToUpdate.KanbanSortOrder! + 1);
            }

            _dbContext.JobApplication.UpdateRange(jobApplications);
        }
    }
}

public sealed class JobApplicationStatusChangeCommandValidator : AbstractValidator<JobApplicationStatusChangeCommand>
{
    public JobApplicationStatusChangeCommandValidator(
        IRepository<JobApplication> jobApplicationRepository,
        ILocalizationService localizationService)
    {
        RuleFor(cmd => cmd.JobApplicationId)
            .NotNull()
            .DependentRules(
                  () =>
                  {
                      RuleFor(x => new EntityExistsValidatorData(x.JobApplicationId))
                        .SetValidator(new EntityExistsValidator<JobApplication>(jobApplicationRepository, localizationService))
                        .OverridePropertyName(nameof(JobApplicationStatusChangeCommand.JobApplicationId));
                  });

        RuleFor(cmd => cmd.Status)
            .IsInEnum();
    }
}