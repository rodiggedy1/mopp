using Application.Features.Jobs.JobApplications.Commands;
using Application.Features.Jobs.JobForms.Commands;
using Application.Features.Jobs.JobsDetails.Commands;
using Application.Features.Notifications.Commands;
using Application.Features.Users.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Infrastructure.Search;

public static class SearchIndexInitializer
{
    public static async Task InitializeIndexes(ISender mediatr, ILogger<ApplicationDbContextInitialiser> logger)
    {
        await InitializeUserIndex(mediatr, logger);
        await InitializeNotificationIndex(mediatr, logger);
        await InitializeJobDetailsIndex(mediatr, logger);
        await InitializeJobFormIndex(mediatr, logger);
        await InitializeJobApplicationIndex(mediatr, logger);
    }

    private static async Task InitializeUserIndex(ISender mediatr, ILogger<ApplicationDbContextInitialiser> logger)
    {
        try
        {
            logger.LogDebug("STARTED BUILDING SEARCH INDEX FOR USER");
            await mediatr.Send(new UserRebuildSearchIndexCommand());
            logger.LogDebug("FINISHED BUILDING SEARCH INDEX FOR USER");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ERROR WHILE BUILDING SEARCH INDEX FOR USER");
        }
    }

    private static async Task InitializeNotificationIndex(ISender mediatr, ILogger<ApplicationDbContextInitialiser> logger)
    {
        try
        {
            logger.LogDebug("STARTED BUILDING SEARCH INDEX FOR NOTIFICATION");
            await mediatr.Send(new NotificationRebuildSearchIndexCommand());
            logger.LogDebug("FINISHED BUILDING SEARCH INDEX FOR NOTIFICATION");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ERROR WHILE BUILDING SEARCH INDEX FOR NOTIFICATION");
        }
    }

    private static async Task InitializeJobDetailsIndex(ISender mediatr, ILogger<ApplicationDbContextInitialiser> logger)
    {
        try
        {
            logger.LogDebug("STARTED BUILDING SEARCH INDEX FOR JOB DETAILS");
            await mediatr.Send(new JobDetailsRebuildSearchIndexCommand());
            logger.LogDebug("FINISHED BUILDING SEARCH INDEX FOR JOB DETAILS");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ERROR WHILE BUILDING SEARCH INDEX FOR JOB DETAILS");
        }
    }

    private static async Task InitializeJobFormIndex(ISender mediatr, ILogger<ApplicationDbContextInitialiser> logger)
    {
        try
        {
            logger.LogDebug("STARTED BUILDING SEARCH INDEX FOR JOB FORM");
            await mediatr.Send(new JobFormRebuildSearchIndexCommand());
            logger.LogDebug("FINISHED BUILDING SEARCH INDEX FOR JOB FORM");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ERROR WHILE BUILDING SEARCH INDEX FOR JOB FORM");
        }
    }

    private static async Task InitializeJobApplicationIndex(ISender mediatr, ILogger<ApplicationDbContextInitialiser> logger)
    {
        try
        {
            logger.LogDebug("STARTED BUILDING SEARCH INDEX FOR JOB APPLICATION");
            await mediatr.Send(new JobApplicationRebuildSearchIndexCommand());
            logger.LogDebug("FINISHED BUILDING SEARCH INDEX FOR JOB APPLICATION");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ERROR WHILE BUILDING SEARCH INDEX FOR JOB APPLICATION");
        }
    }
}
