using DTO.Job.JobApplication.JobApplicationSection;
using DTO.Job.JobDetails;
using DTO.Medias;
using DTO.Response;

namespace DTO.Job.JobApplication;

public record JobApplicationResponse
{
    public int Id { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? EmailAddress { get; init; }
    public string? PhoneNumber { get; init; }
    public string? StreetAddress { get; init; }
    public string? Apartment { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? ZipCode { get; init; }
    public bool? HasCleaningExperience { get; init; }
    public bool? HasBankAccountForDirectDeposit { get; init; }
    public bool? IsLegallyAuthorizedToWorkInUS { get; init; }
    public bool? AllowsConsentToFreeBackgroundCheck { get; init; }
    public string? CleaningExperienceDescription { get; init; }
    public MediaResponse Media { get; init; } = null!;
    public MediaItemResponse? ProfilePicture { get; init; }
    public MediaItemResponse? ApplicationVideo { get; init; }
    public DateTime Created { get; init; }
    public DateTime? StatusUpdatedDate { get; init; }
    public int? KanbanSortOrder { get; init; }

    public int JobDetailsId { get; init; }

    public ListItemBaseResponse Status { get; init; } = null!;
    public JobDetailsBaseResponse JobDetails { get; init; } = null!;

    public IReadOnlyCollection<JobApplicationSectionResponse>? JobApplicationSections { get; init; }
}