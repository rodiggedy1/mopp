using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Interfaces;
using DTO.Enums.Job.JobApplication;
using DTO.Job.JobApplication;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Jobs.JobApplications.Queries;

public sealed record JobApplicationDashboardStatisticsQuery() : IQuery<JobApplicationDashboardStatisticsResponse>;
public sealed class JobApplicationDashboardStatisticsQueryHandler : IQueryHandler<JobApplicationDashboardStatisticsQuery, JobApplicationDashboardStatisticsResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IDateTime _dateTime;

    public JobApplicationDashboardStatisticsQueryHandler(
        IApplicationDbContext dbContext,
        IDateTime dateTime,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _dateTime = dateTime;
        _currentUserService = currentUserService;
    }
    public async Task<JobApplicationDashboardStatisticsResponse> Handle(JobApplicationDashboardStatisticsQuery request, CancellationToken cancellationToken)
    {
        var response = new JobApplicationDashboardStatisticsResponse();

        response.ApplicationsThisMonthCount = await GetThisMonthApplicationsCount(cancellationToken);
        var lastMonthApplications = await GetLastMonthApplicationsCount(cancellationToken);
        response.ApplicationsLastMonthCompared = GetLastMonthApplicationsCompared(response.ApplicationsThisMonthCount, lastMonthApplications);

        response.HiredThisMonthCount = await GetThisMonthHiredCount(cancellationToken);
        var lastMonthHired = await GetLastMonthHiredCount(cancellationToken);
        response.HiredLastMonthCompared = GetLastMonthHiredCompared(response.HiredThisMonthCount, lastMonthHired);

        response.InScreeningThisMonthCount= await GetThisMonthInScreeningCount(cancellationToken);
        response.InScreeningLastMonthCompared = await GetLastMonthInScreeningCompared(response.InScreeningThisMonthCount, cancellationToken);

        response.SuccessRateThisMonth = GetThisMonthSuccessRate(response.ApplicationsThisMonthCount, response.HiredThisMonthCount);
        response.SuccessRateLastMonthCompared = GetLastMonthSuccessRateCompared(lastMonthApplications, lastMonthHired, response.SuccessRateThisMonth);

        return response;
    }

    #region Total Applications This Month & Last Month Compared

    private async Task<int> GetThisMonthApplicationsCount(CancellationToken cancellationToken)
    {
        return await _dbContext.JobApplication
            .AsNoTracking()
            .Include(jd => jd.JobDetails)
            .Where(ja => ja.JobDetails != null &&
                         ja.JobDetails.CreatedBy == _currentUserService.UserId && 
                         ja.Created.Date >= _dateTime.Now.AddDays(-30).Date)
            .CountAsync(cancellationToken);
    }
    private async Task<int> GetLastMonthApplicationsCount(CancellationToken cancellationToken)
    {
        return await _dbContext.JobApplication
            .AsNoTracking()
            .Include(jd => jd.JobDetails)
            .Where(ja => ja.JobDetails != null &&
                         ja.JobDetails.CreatedBy == _currentUserService.UserId &&
                         ja.Created.Date >= _dateTime.Now.AddDays(-60).Date &&
                         ja.Created.Date < _dateTime.Now.AddDays(-30).Date)
            .CountAsync(cancellationToken);
    }
    private int GetLastMonthApplicationsCompared(int thisMonthApplicationsCount, int lastMonthApplicationsCount)
    {
        if(lastMonthApplicationsCount == 0)
        {
            if (thisMonthApplicationsCount == 0)
                return 0;
            else
                return 100;
        }

        var percentageCompared = ((double)thisMonthApplicationsCount - lastMonthApplicationsCount) / lastMonthApplicationsCount * 100;

        return (int)percentageCompared;
    }

    #endregion

    #region Total Hired Applications This Month & Last Month Compared

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
    private async Task<int> GetLastMonthHiredCount(CancellationToken cancellationToken)
    {
        return await _dbContext.JobApplication
            .AsNoTracking()
            .Include(jd => jd.JobDetails)
            .Where(ja => ja.JobDetails != null &&
                         ja.JobDetails.CreatedBy == _currentUserService.UserId &&
                         ja.Status == JobApplicationStatus.Hired &&
                         ja.StatusUpdatedDate != null &&
                         ja.StatusUpdatedDate.Value.Date >= _dateTime.Now.AddDays(-60).Date &&
                         ja.StatusUpdatedDate.Value.Date < _dateTime.Now.AddDays(-30).Date)
            .CountAsync(cancellationToken);
    }
    private int GetLastMonthHiredCompared(int thisMonthHiredCount, int lastMonthHiredCount)
    {
        if (lastMonthHiredCount == 0)
        {
            if (thisMonthHiredCount == 0)
                return 0;
            else
                return 100;
        }
        var percentageCompared = ((double)thisMonthHiredCount - lastMonthHiredCount) / lastMonthHiredCount * 100;

        return (int)percentageCompared;
    }

    #endregion

    #region Total In Screening Applications This Month & Last Month Compared

    private async Task<int> GetThisMonthInScreeningCount(CancellationToken cancellationToken)
    {
        return await _dbContext.JobApplication
            .AsNoTracking()
            .Include(jd => jd.JobDetails)
            .Where(ja => ja.JobDetails != null &&
                         ja.JobDetails.CreatedBy == _currentUserService.UserId &&
                         ja.Status == JobApplicationStatus.Screening &&
                         ja.StatusUpdatedDate != null &&
                         ja.StatusUpdatedDate.Value.Date >= _dateTime.Now.AddDays(-30).Date)
            .CountAsync(cancellationToken);
    }
    private async Task<int> GetLastMonthInScreeningCompared(int thisMonthInScreeningCount, CancellationToken cancellationToken)
    {
        var lastMonthInScreeningCount = await _dbContext.JobApplication
            .AsNoTracking()
            .Include(jd => jd.JobDetails)
            .Where(ja => ja.JobDetails != null &&
                         ja.JobDetails.CreatedBy == _currentUserService.UserId &&
                         ja.Status == JobApplicationStatus.Screening &&
                         ja.StatusUpdatedDate != null &&
                         ja.StatusUpdatedDate.Value.Date >= _dateTime.Now.AddDays(-60).Date &&
                         ja.StatusUpdatedDate.Value.Date < _dateTime.Now.AddDays(-30).Date)
            .CountAsync(cancellationToken);

        if (lastMonthInScreeningCount == 0)
        {
            if (thisMonthInScreeningCount == 0)
                return 0;
            else
                return 100;
        }

        var percentageCompared = ((double)thisMonthInScreeningCount - lastMonthInScreeningCount) / lastMonthInScreeningCount * 100;

        return (int)percentageCompared;
    }

    #endregion

    #region Get Total Success Rate This Month & Last Month Compared

    private int GetThisMonthSuccessRate(int totalApplicationsThisMonth, int totalHiredThisMonth)
    {
        if (totalApplicationsThisMonth == 0) return 0;
        var successRate = (double)totalHiredThisMonth / totalApplicationsThisMonth * 100;
        return (int)successRate;
    }
    private int GetLastMonthSuccessRateCompared(int totalApplicationsLastMonth, int totalHiredLastMonth, int thisMonthSuccessRate)
    {
        if (totalApplicationsLastMonth == 0)
        {
            return thisMonthSuccessRate > 0 ? 100 : 0;
        }

        var lastMonthSuccessRate = (double)totalHiredLastMonth / totalApplicationsLastMonth * 100;

        var successRate = thisMonthSuccessRate - lastMonthSuccessRate;

        return (int)successRate;
    }

    #endregion
}