using DTO.Job.JobForm;
using DTO.User;

namespace DTO.Job.JobDetails;

public record JobDetailsResponse : JobDetailsBaseResponse
{
    public UserBaseResponse Creator { get; set; } = new();
}
