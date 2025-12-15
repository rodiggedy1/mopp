using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Search;
using Application.Features.Jobs.JobForms.Queries;
using Application.Features.Jobs.JobForms.Search;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Jobs.JobForms.Commands;

public sealed record JobFormRebuildSearchIndexCommand : ICommand;

public sealed class JobFormRebuildSearchIndexCommandHandler : ICommandHandler<JobFormRebuildSearchIndexCommand>
{
    private readonly ILogger<JobFormRebuildSearchIndexCommandHandler> _logger;
    private readonly ISearchClient<JobFormSearchable> _searchClient;
    private readonly ISearchIndexProvider _searchIndexProvider;
    private readonly ISender _mediatr;
    private readonly IMapper _mapper;

    public JobFormRebuildSearchIndexCommandHandler(
        ILogger<JobFormRebuildSearchIndexCommandHandler> logger,
        ISearchClient<JobFormSearchable> searchClient,
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

    public async Task Handle(JobFormRebuildSearchIndexCommand request, CancellationToken cancellationToken)
    {
        var index = _searchIndexProvider.GetIndex<JobFormSearchable>();
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

            var jobForms = await _mediatr.Send(new JobFormGetAllQuery());

            _logger.LogInformation("Attempting to index data for index: {0} with job forms count {1}", index, jobForms.Count);
            if (jobForms.Any())
            {
                var searchableJobForms = _mapper.Map<IReadOnlyCollection<JobFormSearchable>>(jobForms);
                await _searchClient.IndexAndRefreshManyAsync(searchableJobForms, cancellationToken);
                _logger.LogInformation("Indexing data finished for index: {0}", index);
            }
            else
            {
                _logger.LogInformation("No job forms to index...");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while reindexing Elastic index {0}", index);
        }
    }
}