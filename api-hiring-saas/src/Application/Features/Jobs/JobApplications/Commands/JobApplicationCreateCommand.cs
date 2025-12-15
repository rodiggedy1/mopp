using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository.Base;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using Application.Features.Jobs.JobApplications.JobApplicationSections.Commands;
using Application.Features.Validators;
using AutoMapper;
using Domain.Entities.JobApplications;
using Domain.Entities.JobApplicationSectionIcons;
using Domain.Entities.JobApplicationSectionProperties;
using Domain.Entities.JobApplicationSections;
using Domain.Entities.JobsDetails;
using Domain.Interfaces;
using DTO.Enums.Job.JobApplication;
using DTO.Job.JobApplication.JobApplicationSection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Application.Features.Jobs.JobApplications.Commands;

public sealed record JobApplicationCreateCommand(
    string? FirstName,
    string? LastName,
    string? EmailAddress,
    string? PhoneNumber,
    string? StreetAddress,
    string? Apartment,
    string? City,
    string? State,
    string? ZipCode,
    bool? HasCleaningExperience,
    bool? HasBankAccountForDirectDeposit,
    bool? IsLegallyAuthorizedToWorkInUS,
    bool? AllowsConsentToFreeBackgroundCheck,
    string? CleaningExperienceDescription,
    int? KanbanSortOrder,
    int JobDetailsId,
    string? JobApplicationSectionsJson,
    IFormFile? ProfilePicture,
    IFormFile? ApplicationVideo) : IJobApplicationInsertData, ICommand;

public sealed class JobApplicationCreateCommandHandler : ICommandHandler<JobApplicationCreateCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTimeProvider;
    private readonly IMapper _mapper;
    private readonly ISender _mediatr;
    private readonly ILogger<JobApplicationCreateCommand> _logger;

    public JobApplicationCreateCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IDateTime dateTimeProvider,
        IMapper mapper,
        ISender mediatr,
        ILogger<JobApplicationCreateCommand> logger)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
        _mediatr = mediatr;
        _logger = logger;
    }

    public async Task Handle(JobApplicationCreateCommand command, CancellationToken cancellationToken)
    {
        JobApplication newJobApplication = JobApplication.Create(
            command with { KanbanSortOrder = await GetLatestSortOrder(cancellationToken) },
            _dateTimeProvider);

        if (!string.IsNullOrEmpty(command.JobApplicationSectionsJson))
        {
            var jobApplicationSectionsJson = ConvertSectionsFromJson(command.JobApplicationSectionsJson!);

            var sectionsToAdd = new List<JobApplicationSection>();

            if (jobApplicationSectionsJson != null)
            {
                foreach (var section in jobApplicationSectionsJson)
                {
                    var commandToCreate = _mapper.Map<JobApplicationSectionCreateCommand>(section);

                    var sectionIcon = _mapper.Map<JobApplicationSectionIcon>(section.Icon);
                    var sectionProperties = _mapper.Map<List<JobApplicationSectionProperty>>(section.JobApplicationSectionProperties);

                    var createdSection = await _mediatr.Send(commandToCreate with { Icon = sectionIcon, JobApplicationSectionProperties = sectionProperties }, cancellationToken);

                    sectionsToAdd.Add(createdSection);
                }
            }

            newJobApplication.SetSections(sectionsToAdd);
        }


        await _dbContext.JobApplication.AddAsync(newJobApplication, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    private async Task<int> GetLatestSortOrder(CancellationToken cancellationToken = default)
    {
        var kanbanNext = await _dbContext.JobApplication
            .Where(s => s.Status == JobApplicationStatus.Applied)
            .DefaultIfEmpty()
            .MaxAsync(s => s != null ? s.KanbanSortOrder : 0, cancellationToken) + 1;

        return (int)kanbanNext;
    }

    private List<JobApplicationSectionCreateRequest>? ConvertSectionsFromJson(string jsonString)
    {
        var sections = new List<JobApplicationSectionCreateRequest>();

        try
        {
            sections = JsonSerializer.Deserialize<List<JobApplicationSectionCreateRequest>>(
                jsonString,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error for JobApplicationSectionsJson: {Message}", ex.Message);

            throw new FormatException("Invalid JSON format for JobApplicationSectionsJson.");
        }

        return sections;
    }
}

public sealed class JobApplicationCreateCommandValidator : AbstractValidator<JobApplicationCreateCommand>
{
    public JobApplicationCreateCommandValidator(
        IRepository<JobDetails> jobDetailsRepository,
        ILocalizationService localizationService)
    {
        RuleFor(cmd => cmd.JobDetailsId)
            .NotNull()
            .DependentRules(
                  () =>
                  {
                      RuleFor(x => new EntityExistsValidatorData(x.JobDetailsId))
                        .SetValidator(new EntityExistsValidator<JobDetails>(jobDetailsRepository, localizationService))
                        .OverridePropertyName(nameof(JobApplicationCreateCommand.JobDetailsId));
                  });
    }
}