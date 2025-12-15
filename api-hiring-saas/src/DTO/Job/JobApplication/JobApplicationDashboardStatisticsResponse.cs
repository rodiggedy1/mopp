namespace DTO.Job.JobApplication;

public sealed record JobApplicationDashboardStatisticsResponse
{
    public int ApplicationsThisMonthCount { get; set; }
    public int ApplicationsLastMonthCompared { get; set; }
    public int InScreeningThisMonthCount { get; set; }
    public int InScreeningLastMonthCompared { get; set; }
    public int HiredThisMonthCount { get; set; }
    public int HiredLastMonthCompared { get; set; }
    public int SuccessRateThisMonth { get; set; }
    public int SuccessRateLastMonthCompared { get; set; }
}
