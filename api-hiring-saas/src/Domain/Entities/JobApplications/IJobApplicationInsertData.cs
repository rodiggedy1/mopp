using DTO.Enums.Job.JobApplication;
using Microsoft.AspNetCore.Http;

namespace Domain.Entities.JobApplications;

public interface IJobApplicationInsertData
{
    string? FirstName { get; }
    string? LastName { get; }
    string? EmailAddress { get; }
    string? PhoneNumber { get; }
    string? StreetAddress { get; }
    string? Apartment { get; }
    string? City { get; }
    string? State { get; }
    string? ZipCode { get; }
    bool? HasCleaningExperience { get; }
    bool? HasBankAccountForDirectDeposit { get; }
    bool? IsLegallyAuthorizedToWorkInUS { get; }
    bool? AllowsConsentToFreeBackgroundCheck { get; }
    string? CleaningExperienceDescription { get; }
    int? KanbanSortOrder { get; }
    int JobDetailsId { get; }
    IFormFile? ProfilePicture { get; }
    IFormFile? ApplicationVideo { get; }
}
