using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Search;
using Application.Features.Jobs.JobForms.Queries;
using Application.Features.Jobs.JobForms.Search;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Jobs.JobForms.Commands;

public sealed record JobFormIndexCommand(int JobFormId) : ICommand;

public sealed record JobFormIndexCommandHandler : ICommandHandler<JobFormIndexCommand>
{
    private readonly ILogger<JobFormIndexCommandHandler> _logger;
    private readonly ISearchClient<JobFormSearchable> _searchClient;
    private readonly ISender _mediatr;
    private readonly IMapper _mapper;

    public JobFormIndexCommandHandler(
        ILogger<JobFormIndexCommandHandler> logger,
        ISearchClient<JobFormSearchable> searchClient,
        ISender mediatr,
        IMapper mapper)
    {
        _logger = logger;
        _searchClient = searchClient;
        _mediatr = mediatr;
        _mapper = mapper;
    }

    public async Task Handle(JobFormIndexCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to index data for job form with ID: {0}", command.JobFormId);
        var jobForm = await _mediatr.Send(new JobFormGetQuery(command.JobFormId));

        if (jobForm != null)
        {
            try
            {
                await _searchClient.IndexAndRefreshAsync(_mapper.Map<JobFormSearchable>(jobForm), cancellationToken);
                _logger.LogInformation("Indexing finished for job form with ID: {0}", command.JobFormId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while reindexing data for job form with ID: {0}", command.JobFormId);
            }
        }
        else
        {
            _logger.LogInformation("Job form does not exist: {0}", command.JobFormId);
        }
    }
}