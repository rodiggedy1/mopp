using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using DTO.Job.JobForm;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Jobs.JobForms.Queries;

public sealed record JobFormGetAllQuery() : IQuery<IReadOnlyCollection<JobFormResponse>>;

public sealed class JobFormGetAllQueryHandler : IQueryHandler<JobFormGetAllQuery, IReadOnlyCollection<JobFormResponse>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public JobFormGetAllQueryHandler(
        IApplicationDbContext dbContext,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<JobFormResponse>> Handle(JobFormGetAllQuery request, CancellationToken cancellationToken)
    {
        var jobForms = await _dbContext.JobForm
            .AsNoTracking()
            .Include(jf => jf.JobFormSections)
            .ToListAsync(cancellationToken);

        var response = _mapper.Map<IReadOnlyCollection<JobFormResponse>>(jobForms);

        return response;
    }
}