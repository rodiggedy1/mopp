using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using AutoMapper;
using DTO.Job.JobForm;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Jobs.JobForms.Queries;

public sealed record JobFormGetQuery(int JobFormId) : IQuery<JobFormResponse>;

public sealed class JobFormGetQueryHandler : IQueryHandler<JobFormGetQuery, JobFormResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILocalizationService _localizationService;

    public JobFormGetQueryHandler(
        IApplicationDbContext dbContext,
        IMapper mapper,
        ILocalizationService localizationService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _localizationService = localizationService;
    }

    public async Task<JobFormResponse> Handle(JobFormGetQuery query, CancellationToken cancellationToken)
    {
        var jobForm = await _dbContext.JobForm
                            .AsNoTracking()
                            .Include(jf => jf.JobFormSections)
                            .FirstOrDefaultAsync(s => s.Id == query.JobFormId, cancellationToken);

        if (jobForm == null)
            throw new NotFoundException(_localizationService.GetValue("jobForm.notFound.error.message"));

        var response = _mapper.Map<JobFormResponse>(jobForm);

        return response;
    }
}