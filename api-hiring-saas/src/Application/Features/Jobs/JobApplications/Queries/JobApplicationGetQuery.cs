using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using AutoMapper;
using DTO.Job.JobApplication;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Jobs.JobApplications.Queries;

public sealed record JobApplicationGetQuery(int JobApplicationId) : IQuery<JobApplicationResponse>;

public sealed class JobApplicationGetQueryHandler : IQueryHandler<JobApplicationGetQuery, JobApplicationResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILocalizationService _localizationService;

    public JobApplicationGetQueryHandler(
        IApplicationDbContext dbContext,
        IMapper mapper,
        ILocalizationService localizationService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _localizationService = localizationService;
    }
    public async Task<JobApplicationResponse> Handle(JobApplicationGetQuery query, CancellationToken cancellationToken)
    {
        var jobApplication = await _dbContext.JobApplication
                            .AsNoTracking()
                            .Include(jd => jd.JobDetails)
                                .ThenInclude(jf => jf!.JobForm)
                            .Include(s => s.JobApplicationSections)
                            .FirstOrDefaultAsync(s => s.Id == query.JobApplicationId, cancellationToken);

        if (jobApplication == null)
            throw new NotFoundException(_localizationService.GetValue("jobApplication.notFound.error.message"));

        var response = _mapper.Map<JobApplicationResponse>(jobApplication);

        return response;
    }
}
