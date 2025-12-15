using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Search;
using Application.Features.Jobs.JobsDetails.Queries;
using Application.Features.Jobs.JobsDetails.Search;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Jobs.JobsDetails.Commands;

public sealed record JobDetailsRebuildSearchIndexCommand : ICommand;

public sealed class JobDetailsRebuildSearchIndexCommandHandler : ICommandHandler<JobDetailsRebuildSearchIndexCommand>
{
    private readonly ILogger<JobDetailsRebuildSearchIndexCommandHandler> _logger;
    private readonly ISearchClient<JobDetailsSearchable> _searchClient;
    private readonly ISearchIndexProvider _searchIndexProvider;
    private readonly ISender _mediatr;
    private readonly IMapper _mapper;

    public JobDetailsRebuildSearchIndexCommandHandler(
        ILogger<JobDetailsRebuildSearchIndexCommandHandler> logger,
        ISearchClient<JobDetailsSearchable> searchClient,
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
    public async Task Handle(JobDetailsRebuildSearchIndexCommand request, CancellationToken cancellationToken)
    {
        var index = _searchIndexProvider.GetIndex<JobDetailsSearchable>();
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

            var jobsDetails = await _mediatr.Send(new JobDetailsGetAllQuery());

            _logger.LogInformation("Attempting to index data for index: {0} with job details count {1}", index, jobsDetails.Count);
            if (jobsDetails.Any())
            {
                var searchableJobDetails = _mapper.Map<IReadOnlyCollection<JobDetailsSearchable>>(jobsDetails);
                await _searchClient.IndexAndRefreshManyAsync(searchableJobDetails, cancellationToken);
                _logger.LogInformation("Indexing data finished for index: {0}", index);
            }
            else
            {
                _logger.LogInformation("No job details to index...");
            }
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while reindexing Elastic index {0}", index);
        }
    }
}