using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using DTO.Job.JobDetails;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Jobs.JobsDetails.Queries;

public sealed record JobDetailsGetAllQuery() : IQuery<IReadOnlyCollection<JobDetailsResponse>>;
public sealed class JobDetailsGetAllQueryHandler : IQueryHandler<JobDetailsGetAllQuery, IReadOnlyCollection<JobDetailsResponse>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
 
    public JobDetailsGetAllQueryHandler(
        IApplicationDbContext dbContext, 
        IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public async Task<IReadOnlyCollection<JobDetailsResponse>> Handle(JobDetailsGetAllQuery request, CancellationToken cancellationToken)
    {
        var jobDetails = await _dbContext.JobDetails
            .AsNoTracking()
            .Include(jf => jf.JobForm)
            .ThenInclude(s => s.JobFormSections)
            .Include(ja => ja.JobApplications)
            .ToListAsync(cancellationToken);

        var response = _mapper.Map<IReadOnlyCollection<JobDetailsResponse>>(jobDetails);

        return response;
    }
}