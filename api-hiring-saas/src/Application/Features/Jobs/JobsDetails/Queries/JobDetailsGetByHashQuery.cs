using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using AutoMapper;
using DTO.Job.JobDetails;
using DTO.User;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Jobs.JobsDetails.Queries;

public sealed record JobDetailsGetByHashQuery(string JobDetailsHash) : IQuery<JobDetailsResponse>;

public sealed class JobDetailsGetByHashQueryHandler : IQueryHandler<JobDetailsGetByHashQuery, JobDetailsResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILocalizationService _localizationService;

    public JobDetailsGetByHashQueryHandler(
        IApplicationDbContext dbContext,
        IMapper mapper,
        ILocalizationService localizationService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _localizationService = localizationService;
    }
    public async Task<JobDetailsResponse> Handle(JobDetailsGetByHashQuery query, CancellationToken cancellationToken)
    {
        var jobDetails = await _dbContext.JobDetails
                             .AsNoTracking()
                             .Include(jf => jf.JobForm)
                             .ThenInclude(s => s.JobFormSections)
                             .Include(ja => ja.JobApplications)
                             .FirstOrDefaultAsync(s => s.UniqueHash == query.JobDetailsHash, cancellationToken);

        if (jobDetails == null)
            throw new NotFoundException(_localizationService.GetValue("jobDetails.notFound.error.message"));

        var response = _mapper.Map<JobDetailsResponse>(jobDetails);

        response.Creator = _mapper.Map<UserBaseResponse>(await _dbContext.User.FirstOrDefaultAsync(s => s.Id == response.CreatedBy));

        return response;
    }
}
