using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Search;
using Application.Features.Jobs.JobApplications.Queries;
using Application.Features.Jobs.JobApplications.Search;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Jobs.JobApplications.Commands;

public sealed record JobApplicationRebuildSearchIndexCommand : ICommand;

public sealed class JobApplicationRebuildSearchIndexCommandHandler : ICommandHandler<JobApplicationRebuildSearchIndexCommand>
{
    private readonly ILogger<JobApplicationRebuildSearchIndexCommandHandler> _logger;
    private readonly ISearchClient<JobApplicationSearchable> _searchClient;
    private readonly ISearchIndexProvider _searchIndexProvider;
    private readonly ISender _mediatr;
    private readonly IMapper _mapper;

    public JobApplicationRebuildSearchIndexCommandHandler(
        ILogger<JobApplicationRebuildSearchIndexCommandHandler> logger,
        ISearchClient<JobApplicationSearchable> searchClient,
        ISearchIndexProvider searchIndexProvider,
        ISender mediatr,
        IMapper mapper)
    {
        _logger = logger;
        _searchClient = searchClient;
        _searchIndexProvider = searchIndexProvider;
        _mediatr = mediatr;
        _mapper = mapper;
    }
    public async Task Handle(JobApplicationRebuildSearchIndexCommand request, CancellationToken cancellationToken)
    {
        var index = _searchIndexProvider.GetIndex<JobApplicationSearchable>();
        try
        {
            await _searchClient.CreateIndexIfNotExist(index);
            _logger.LogInformation("Attempting to delete data for index: {0}", index);
            await _searchClient.DeleteAllAsync(cancellationToken);
            _logger.LogInformation("Delete finished for index: {0}", index);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting Elastic index {0}", index);
        }

        try
        {
            _logger.LogInformation("Attempting to index data for index: {0}", index);

            var jobApplications = await _mediatr.Send(new JobApplicationGetAllQuery());

            _logger.LogInformation("Attempting to index data for index: {0} with job applications count {1}", index, jobApplications.Count);
            if (jobApplications.Any())
            {
                var searchableJobApplication = _mapper.Map<IReadOnlyCollection<JobApplicationSearchable>>(jobApplications);
                await _searchClient.IndexAndRefreshManyAsync(searchableJobApplication, cancellationToken);
                _logger.LogInformation("Indexing data finished for index: {0}", index);
            }
            else
            {
                _logger.LogInformation("No job applications to index...");
            }
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while reindexing Elastic index {0}", index);
        }
    }
}