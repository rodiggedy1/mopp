using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using AutoMapper;
using DTO.Job.JobDetails;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Jobs.JobsDetails.Queries;

public sealed record JobDetailsGetQuery(int JobDetailsId) : IQuery<JobDetailsResponse>;

public sealed class JobDetailsGetQueryHandler : IQueryHandler<JobDetailsGetQuery, JobDetailsResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILocalizationService _localizationService;
    
    public JobDetailsGetQueryHandler(
        IApplicationDbContext dbContext,
        IMapper mapper,
        ILocalizationService localizationService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _localizationService = localizationService;
    }
    public async Task<JobDetailsResponse> Handle(JobDetailsGetQuery query, CancellationToken cancellationToken)
    {
        var jobDetails = await _dbContext.JobDetails
                             .AsNoTracking()
                             .Include(jf => jf.JobForm)
                             .ThenInclude(s => s.JobFormSections)
                             .Include(ja => ja.JobApplications)
                             .FirstOrDefaultAsync(s => s.Id == query.JobDetailsId, cancellationToken);

        if (jobDetails == null)
            throw new NotFoundException(_localizationService.GetValue("jobDetails.notFound.error.message"));

        var response = _mapper.Map<JobDetailsResponse>(jobDetails);

        return response;
    }
}
