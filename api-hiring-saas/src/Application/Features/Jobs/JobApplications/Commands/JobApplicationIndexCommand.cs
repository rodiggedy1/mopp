using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Search;
using Application.Features.Jobs.JobApplications.Queries;
using Application.Features.Jobs.JobApplications.Search;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Jobs.JobApplications.Commands;

public sealed record JobApplicationIndexCommand(int JobApplicationId) : ICommand;

public sealed record JobApplicationIndexCommandHandler : ICommandHandler<JobApplicationIndexCommand>
{
    private readonly ILogger<JobApplicationIndexCommandHandler> _logger;
    private readonly ISearchClient<JobApplicationSearchable> _searchClient;
    private readonly ISender _mediatr;
    private readonly IMapper _mapper;

    public JobApplicationIndexCommandHandler(
        ILogger<JobApplicationIndexCommandHandler> logger,
        ISearchClient<JobApplicationSearchable> searchClient,
        ISender mediatr,
        IMapper mapper)
    {
        _logger = logger;
        _searchClient = searchClient;
        _mediatr = mediatr;
        _mapper = mapper;
    }

    public async Task Handle(JobApplicationIndexCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to index data for job application with ID: {0}", command.JobApplicationId);
        var jobApplication = await _mediatr.Send(new JobApplicationGetQuery(command.JobApplicationId));

        if (jobApplication != null)
        {
            try
            {
                await _searchClient.IndexAndRefreshAsync(_mapper.Map<JobApplicationSearchable>(jobApplication), cancellationToken);
                _logger.LogInformation("Indexing finished for job application with ID: {0}", command.JobApplicationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while reindexing data for job application with ID: {0}", command.JobApplicationId);
            }
        }
        else
        {
            _logger.LogInformation("Job application does not exist: {0}", command.JobApplicationId);
        }
    }
}