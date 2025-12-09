using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Search;
using Application.Features.Jobs.JobsDetails.Queries;
using Application.Features.Jobs.JobsDetails.Search;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Jobs.JobsDetails.Commands;

public sealed record JobDetailsIndexCommand(int JobDetailsId) : ICommand;

public sealed record JobDetailsIndexCommandHandler : ICommandHandler<JobDetailsIndexCommand>
{
    private readonly ILogger<JobDetailsIndexCommandHandler> _logger;
    private readonly ISearchClient<JobDetailsSearchable> _searchClient;
    private readonly ISender _mediatr;
    private readonly IMapper _mapper;

    public JobDetailsIndexCommandHandler(
        ILogger<JobDetailsIndexCommandHandler> logger,
        ISearchClient<JobDetailsSearchable> searchClient,
        ISender mediatr,
        IMapper mapper)
    {
        _logger = logger;
        _searchClient = searchClient;
        _mediatr = mediatr;
        _mapper = mapper;
    }

    public async Task Handle(JobDetailsIndexCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to index data for job details with ID: {0}", command.JobDetailsId);
        var jobDetails = await _mediatr.Send(new JobDetailsGetQuery(command.JobDetailsId));

        if (jobDetails != null)
        {
            try
            {
                await _searchClient.IndexAndRefreshAsync(_mapper.Map<JobDetailsSearchable>(jobDetails), cancellationToken);
                _logger.LogInformation("Indexing finished for job details with ID: {0}", command.JobDetailsId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while reindexing data for job details with ID: {0}", command.JobDetailsId);
            }
        }
        else
        {
            _logger.LogInformation("Job details does not exist: {0}", command.JobDetailsId);
        }
    }
}