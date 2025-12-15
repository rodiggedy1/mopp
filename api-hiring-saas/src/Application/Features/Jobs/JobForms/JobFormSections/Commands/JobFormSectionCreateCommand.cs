using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.JobFormSectionIcons;
using Domain.Entities.JobFormSectionProperties;
using Domain.Entities.JobFormSections;

namespace Application.Features.Jobs.JobFormSections.JobFormSectionSections.Commands;

public sealed record JobFormSectionCreateCommand(
    string? Name,
    string? Description,
    string? Code,
    int? Position,
    JobFormSectionIcon? Icon,
    List<JobFormSectionProperty>? JobFormSectionProperties) : IJobFormSectionInsertData, ICommand<JobFormSection>;

public sealed class JobFormSectionCreateCommandHandler : ICommandHandler<JobFormSectionCreateCommand, JobFormSection>
{

    public async Task<JobFormSection> Handle(JobFormSectionCreateCommand command, CancellationToken cancellationToken)
    {
        JobFormSection newJobFormSection = JobFormSection.Create(
            command);

        return newJobFormSection;
    }
}
