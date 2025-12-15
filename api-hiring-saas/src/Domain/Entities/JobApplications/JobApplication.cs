using Domain.Entities.Base;
using Domain.Entities.JobApplicationSections;
using Domain.Entities.JobFormSections;
using Domain.Entities.JobsDetails;
using Domain.Entities.Medias;
using Domain.Events;
using Domain.Events.JobApplications;
using Domain.Interfaces;
using DTO.Enums.Job.JobApplication;
using DTO.Enums.Media;

namespace Domain.Entities.JobApplications;

public class JobApplication : BaseEntity, IHasDomainEvents, IWithMedia
{
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? EmailAddress { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? StreetAddress { get; private set; }
    public string? Apartment { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? ZipCode { get; private set; }
    public bool? HasCleaningExperience { get; private set; }
    public bool? HasBankAccountForDirectDeposit { get; private set; }
    public bool? IsLegallyAuthorizedToWorkInUS { get; private set; }
    public bool? AllowsConsentToFreeBackgroundCheck { get; private set; }
    public string? CleaningExperienceDescription { get; private set; }
    public Media Media { get; private set; } = null!;
    public DateTime Created { get; private set; }
    public int? KanbanSortOrder { get; private set; }

    public JobApplicationStatus Status { get; private set; }
    
    public DateTime? ScreeningStatusDate { get; private set; }
    public DateTime? StatusUpdatedDate { get; private set; }

    public int JobDetailsId { get; private set; }

    public JobDetails? JobDetails { get; private set; }

    public virtual IReadOnlyCollection<JobApplicationSection> JobApplicationSections { get; private set; } = new List<JobApplicationSection>();

    private JobApplication() { }
    private JobApplication(
        IJobApplicationInsertData data,
        IDateTime dateTimeProvider)
    {
        FirstName = data.FirstName;
        LastName = data.LastName;
        EmailAddress = data.EmailAddress;
        PhoneNumber = data.PhoneNumber;
        StreetAddress = data.StreetAddress;
        Apartment = data.Apartment;
        City = data.City;
        State = data.State;
        ZipCode = data.ZipCode;
        HasCleaningExperience = data.HasCleaningExperience;
        HasBankAccountForDirectDeposit = data.HasBankAccountForDirectDeposit;
        IsLegallyAuthorizedToWorkInUS = data.IsLegallyAuthorizedToWorkInUS;
        AllowsConsentToFreeBackgroundCheck = data.AllowsConsentToFreeBackgroundCheck;
        CleaningExperienceDescription = data.CleaningExperienceDescription;
        KanbanSortOrder = data.KanbanSortOrder;
        Status = JobApplicationStatus.Applied;
        JobDetailsId = data.JobDetailsId;

        Created = dateTimeProvider.Now;

        Media = new Media(MediaEntityType.JobApplication);

        AddDomainEvent(new JobApplicationCreatedEvent(this, data.ProfilePicture, data.ApplicationVideo));
    }

    public static JobApplication Create(
        IJobApplicationInsertData data,
        IDateTime dateTimeProvider)
    {
        return new JobApplication(data, dateTimeProvider);
    }

    public void ChangeStatus(
        JobApplicationStatus status,
        int? kanbanSortOrder,
        IDateTime dateTimeProvider)
    {
        StatusUpdatedDate = dateTimeProvider.Now;
        Status = status;

        if(status == JobApplicationStatus.Screening)
            ScreeningStatusDate = dateTimeProvider.Now;

        if (kanbanSortOrder != null)
            KanbanSortOrder = kanbanSortOrder;

        AddDomainEvent(new JobApplicationUpdatedEvent(this));
    }

    public void ChangeKanbanSortOrder(
        int kanbanSortOrder)
    {
        KanbanSortOrder = kanbanSortOrder;

        AddDomainEvent(new JobApplicationUpdatedEvent(this));
    }

    public async Task UploadFile(IMediaUpsertData data, IMediaStorage mediaStorage, bool addDomainEvent = true)
    {
        await Media.Save(data, Id, mediaStorage);

        if (addDomainEvent)
            AddDomainEvent(new JobApplicationUpdatedEvent(this));
    }

    public async Task RemoveFile(Guid fileId, IMediaStorage mediaStorage, bool addDomainEvent = true)
    {
        await Media.Delete(fileId, Id, mediaStorage);

        if (addDomainEvent)
            AddDomainEvent(new JobApplicationUpdatedEvent(this));
    }
    public void SetSections(List<JobApplicationSection> sections)
    {
        JobApplicationSections = sections;
    }

}
