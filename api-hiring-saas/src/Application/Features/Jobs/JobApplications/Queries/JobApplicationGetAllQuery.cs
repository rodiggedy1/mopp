using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using DTO.Job.JobApplication;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Jobs.JobApplications.Queries;

public sealed record JobApplicationGetAllQuery() : IQuery<IReadOnlyCollection<JobApplicationResponse>>;
public sealed class JobApplicationGetAllQueryHandler : IQueryHandler<JobApplicationGetAllQuery, IReadOnlyCollection<JobApplicationResponse>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public JobApplicationGetAllQueryHandler(
        IApplicationDbContext dbContext,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public async Task<IReadOnlyCollection<JobApplicationResponse>> Handle(JobApplicationGetAllQuery request, CancellationToken cancellationToken)
    {
        var jobApplication = await _dbContext.JobApplication
            .AsNoTracking()
            .Include(jd => jd.JobDetails)
                .ThenInclude(jf => jf!.JobForm)
            .Include(s => s.JobApplicationSections)
            .ToListAsync(cancellationToken);

        var response = _mapper.Map<IReadOnlyCollection<JobApplicationResponse>>(jobApplication);

        return response;
    }
}