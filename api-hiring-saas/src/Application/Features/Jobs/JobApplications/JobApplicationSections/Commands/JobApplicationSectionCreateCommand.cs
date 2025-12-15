using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.JobApplicationSectionIcons;
using Domain.Entities.JobApplicationSectionProperties;
using Domain.Entities.JobApplicationSections;

namespace Application.Features.Jobs.JobApplications.JobApplicationSections.Commands;

public sealed record JobApplicationSectionCreateCommand(
    string? Name,
    string? Description,
    string? Code,
    int? Position,
    JobApplicationSectionIcon? Icon,
    List<JobApplicationSectionProperty>? JobApplicationSectionProperties) : IJobApplicationSectionInsertData, ICommand<JobApplicationSection>;

public sealed class JobApplicationSectionCreateCommandHandler : ICommandHandler<JobApplicationSectionCreateCommand, JobApplicationSection>
{

    public async Task<JobApplicationSection> Handle(JobApplicationSectionCreateCommand command, CancellationToken cancellationToken)
    {
        JobApplicationSection newJobApplicationSection = JobApplicationSection.Create(
            command);

        return newJobApplicationSection;
    }
}