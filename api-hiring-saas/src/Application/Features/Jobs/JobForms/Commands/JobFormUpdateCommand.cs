using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository.Base;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using Application.Features.Jobs.JobFormSections.JobFormSectionSections.Commands;
using Application.Features.Validators;
using AutoMapper;
using Domain.Entities.JobForms;
using Domain.Entities.JobFormSectionIcons;
using Domain.Entities.JobFormSectionProperties;
using Domain.Entities.JobFormSections;
using DTO.Job.JobForm;
using DTO.Job.JobForm.JobFormSection;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Jobs.JobForms.Commands;

public sealed record JobFormUpdateCommand(
    int Id,
    string Title,
    string Description,
    List<JobFormSectionCreateRequest>? JobFormSections) : IJobFormUpdateData, ICommand<JobFormResponse>;

public sealed class JobFormUpdateCommandHandler : ICommandHandler<JobFormUpdateCommand, JobFormResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ISender _mediatr;

    public JobFormUpdateCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ISender mediatr)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mediatr = mediatr;
    }

    public async Task<JobFormResponse> Handle(JobFormUpdateCommand command, CancellationToken cancellationToken)
    {
        var existingJobForm = await _dbContext.JobForm
            .Include(s => s.JobFormSections)
            .FirstAsync(s => s.Id == command.Id, cancellationToken);

        _dbContext.JobFormSection.RemoveRange(existingJobForm.JobFormSections);
        existingJobForm.SetSections(new List<JobFormSection>());

        existingJobForm.Update(command);

        var sectionsToAdd = new List<JobFormSection>();

        if (command.JobFormSections != null)
        {
            foreach (var section in command.JobFormSections)
            {
                var commandToCreate = _mapper.Map<JobFormSectionCreateCommand>(section);

                var sectionIcon = _mapper.Map<JobFormSectionIcon>(section.Icon);
                var sectionProperties = _mapper.Map<List<JobFormSectionProperty>>(section.JobFormSectionProperties);

                var UpdatedSection = await _mediatr.Send(commandToCreate with { Icon = sectionIcon, JobFormSectionProperties = sectionProperties }, cancellationToken);

                sectionsToAdd.Add(UpdatedSection);
            }
        }

        existingJobForm.SetSections(sectionsToAdd);

        _dbContext.JobForm.Update(existingJobForm);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<JobFormResponse>(existingJobForm);
    }
}

public sealed class JobFormUpdateCommandValidator : AbstractValidator<JobFormUpdateCommand>
{
    public JobFormUpdateCommandValidator(
        IRepository<JobForm> jobFormRepository,
        ILocalizationService localizationService)
    {
        RuleFor(cmd => cmd.Id)
            .NotNull()
            .DependentRules(
                  () =>
                  {
                      RuleFor(x => new EntityExistsValidatorData(x.Id))
                        .SetValidator(new EntityExistsValidator<JobForm>(jobFormRepository, localizationService))
                        .OverridePropertyName(nameof(JobFormUpdateCommand.Id));
                  });

        RuleFor(cmd => cmd.Title)
            .NotNull();

        RuleFor(cmd => cmd.Description)
            .NotNull();
    }
}
