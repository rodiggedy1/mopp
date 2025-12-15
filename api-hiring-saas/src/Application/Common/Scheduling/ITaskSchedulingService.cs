namespace Application.Common.Scheduling
{
    public interface ITaskSchedulingService
    {
        Task ScheduleCommentJob(int commentHistoryId, CancellationToken cancellationToken);
        Task ScheduleCommentGeneratingJob(int commentConfigId, CancellationToken cancellationToken);
    }
}
