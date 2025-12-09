using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Interfaces;
using DTO.Enums.Job.JobApplication;
using DTO.Job.JobApplication;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Jobs.JobApplications.Queries;

public sealed record JobApplicationHiredStatisticsQuery() : IQuery<JobApplicationHiredStatisticsResponse>;
public sealed class JobApplicationHiredStatisticsQueryHandler : IQueryHandler<JobApplicationHiredStatisticsQuery, JobApplicationHiredStatisticsResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IDateTime _dateTime;
    private readonly ICurrentUserService _currentUserService;

    public JobApplicationHiredStatisticsQueryHandler(
        IApplicationDbContext dbContext,
        IDateTime dateTime,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _dateTime = dateTime;
        _currentUserService = currentUserService;
    }
    public async Task<JobApplicationHiredStatisticsResponse> Handle(JobApplicationHiredStatisticsQuery request, CancellationToken cancellationToken)
    {
        var response = new JobApplicationHiredStatisticsResponse();

        response.HiredCount = await GetHiredCount(cancellationToken);
        response.HiredThisMonthCount = await GetThisMonthHiredCount(cancellationToken);
        response.SuccessRate = await GetSuccessRate(response.HiredCount, cancellationToken);

        return response;
    }

    private async Task<int> GetJobApplicationsCount(CancellationToken cancellationToken)
    {
        return await _dbContext.JobApplication
            .AsNoTracking()
            .Include(jd => jd.JobDetails)
            .Where(ja => ja.JobDetails != null &&
                         ja.JobDetails.CreatedBy == _currentUserService.UserId)
            .CountAsync(cancellationToken);
    }
    private async Task<int> GetHiredCount(CancellationToken cancellationToken)
    {
        return await _dbContext.JobApplication
            .AsNoTracking()
            .Include(jd => jd.JobDetails)
            .Where(ja => ja.JobDetails != null &&
                         ja.JobDetails.CreatedBy == _currentUserService.UserId &&
                         ja.Status == JobApplicationStatus.Hired)
            .CountAsync(cancellationToken);
    }
    private async Task<int> GetThisMonthHiredCount(CancellationToken cancellationToken)
    {
        return await _dbContext.JobApplication
            .AsNoTracking()
            .Include(jd => jd.JobDetails)
            .Where(ja => ja.JobDetails != null &&
                         ja.JobDetails.CreatedBy == _currentUserService.UserId &&
                         ja.Status == JobApplicationStatus.Hired &&
                         ja.StatusUpdatedDate != null &&
                         ja.StatusUpdatedDate.Value.Date >= _dateTime.Now.AddDays(-30).Date)
            .CountAsync(cancellationToken);
    }
    private async Task<int> GetSuccessRate(int hiredCount, CancellationToken cancellationToken)
    {
        var jobApplicationsCount = await GetJobApplicationsCount(cancellationToken);

        if (hiredCount == 0 || jobApplicationsCount == 0) return 0;
        var successRate = (double)hiredCount / jobApplicationsCount * 100;
        return (int)successRate;
    }

}