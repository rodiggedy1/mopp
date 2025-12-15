using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Features.Jobs.JobFormSections.JobFormSectionSections.Commands;
using Application.Features.Jobs.JobsDetails.Commands;
using AutoMapper;
using Domain.Entities.JobForms;
using Domain.Entities.JobFormSectionIcons;
using Domain.Entities.JobFormSectionProperties;
using Domain.Entities.JobFormSections;
using Domain.Interfaces;
using DTO.Job.JobForm;
using DTO.Job.JobForm.JobFormSection;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Jobs.JobForms.Commands;

public sealed record JobFormCreateCommand(
    string Title,
    string Description,
    string? UniqueHash,
    List<JobFormSectionCreateRequest>? JobFormSections,
    bool CreateJobDetails = true) : IJobFormInsertData, ICommand<JobFormResponse>;

public sealed class JobFormCreateCommandHandler : ICommandHandler<JobFormCreateCommand, JobFormResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ISender _mediatr;
    private readonly IEncryption _encryption;

    public JobFormCreateCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ISender mediatr,
        IEncryption encryption)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mediatr = mediatr;
        _encryption = encryption;
    }

    public async Task<JobFormResponse> Handle(JobFormCreateCommand command, CancellationToken cancellationToken)
    {
        var uniqueHash = command.UniqueHash;
        do
        {
            uniqueHash = _encryption.GenerateUniqueHashAsync();
        }
        while (await _dbContext.JobForm.AnyAsync(j => j.UniqueHash == uniqueHash, cancellationToken));

        JobForm newJobForm = JobForm.Create(
            command with { UniqueHash = uniqueHash });

        var sectionsToAdd = new List<JobFormSection>();

        if (command.JobFormSections != null)
        {
            foreach (var section in command.JobFormSections)
            {
                var commandToCreate = _mapper.Map<JobFormSectionCreateCommand>(section);

                var sectionIcon = _mapper.Map<JobFormSectionIcon>(section.Icon);
                var sectionProperties = _mapper.Map<List<JobFormSectionProperty>>(section.JobFormSectionProperties);

                var createdSection = await _mediatr.Send(commandToCreate with { Icon = sectionIcon, JobFormSectionProperties = sectionProperties }, cancellationToken);

                sectionsToAdd.Add(createdSection);
            }
        }

        newJobForm.SetSections(sectionsToAdd);
        await _dbContext.JobForm.AddAsync(newJobForm, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if(command.CreateJobDetails)
        {
            await _mediatr.Send(new JobDetailsCreateCommand(
                command.Title,
                command.Description,
                default,
                "Full Time",
                0,
                uniqueHash,
                newJobForm.Id), cancellationToken);
        }

        return _mapper.Map<JobFormResponse>(newJobForm);
    }
}

public sealed class JobFormCreateCommandValidator : AbstractValidator<JobFormCreateCommand>
{
    public JobFormCreateCommandValidator()
    {
        RuleFor(cmd => cmd.Title)
            .NotNull();

        RuleFor(cmd => cmd.Description)
            .NotNull();
    }
}