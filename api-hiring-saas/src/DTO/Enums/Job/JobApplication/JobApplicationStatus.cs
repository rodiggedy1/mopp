using DTO.Attributes;

namespace DTO.Enums.Job.JobApplication;

public enum JobApplicationStatus
{
    [LocalizationKey("enum.applicationJob.status.applied")]
    Applied = 1,
    [LocalizationKey("enum.applicationJob.status.screening")]
    Screening,
    [LocalizationKey("enum.applicationJob.status.interview")]
    Interview,
    [LocalizationKey("enum.applicationJob.status.referenceCheck")]
    ReferenceCheck,
    [LocalizationKey("enum.applicationJob.status.hired")]
    Hired,
    [LocalizationKey("enum.applicationJob.status.rejected")]
    Rejected
}
