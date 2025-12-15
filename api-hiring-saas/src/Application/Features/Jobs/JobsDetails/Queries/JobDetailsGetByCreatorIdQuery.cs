using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using DTO.Job.JobDetails;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Jobs.JobsDetails.Queries;

public sealed record JobDetailsGetByCreatorIdQuery(int CreatorId) : IQuery<IReadOnlyCollection<JobDetailsBaseResponse>>;
public sealed class JobDetailsGetByCreatorIdQueryHandler : IQueryHandler<JobDetailsGetByCreatorIdQuery, IReadOnlyCollection<JobDetailsBaseResponse>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public JobDetailsGetByCreatorIdQueryHandler(
        IApplicationDbContext dbContext,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public async Task<IReadOnlyCollection<JobDetailsBaseResponse>> Handle(JobDetailsGetByCreatorIdQuery request, CancellationToken cancellationToken)
    {
        var jobDetails = await _dbContext.JobDetails
            .AsNoTracking()
            .Where(s => s.CreatedBy == request.CreatorId)
            .Include(jf => jf.JobForm)
                .ThenInclude(s => s.JobFormSections)
            .Include(ja => ja.JobApplications)
            .ToListAsync(cancellationToken);

        var response = _mapper.Map<IReadOnlyCollection<JobDetailsBaseResponse>>(jobDetails);

        return response;
    }
}