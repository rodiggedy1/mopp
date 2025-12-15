namespace DTO.Job.JobApplication;

public sealed record JobApplicationHiredStatisticsResponse
{
    public int HiredCount { get; set; }
    public int HiredThisMonthCount { get; set; }
    public int SuccessRate { get; set; }
}
