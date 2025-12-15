using Application.Common.Search;
using Application.Features.Jobs.JobApplications.Search;
using Application.Features.Jobs.JobForms.Search;
using Application.Features.Jobs.JobsDetails.Search;
using Application.Features.Notifications.Search;
using Application.Features.Users.Search;

namespace Infrastructure.Search;

public class SearchIndexProvider : ISearchIndexProvider
{
    public string GetIndex<T>() where T : ISearchable
    {
        return typeof(T) switch
        {
            _ when typeof(T) == typeof(UserSearchable) => SearchIndex.User,
            _ when typeof(T) == typeof(NotificationSearchable) => SearchIndex.Notification,
            _ when typeof(T) == typeof(JobDetailsSearchable) => SearchIndex.JobDetails,
            _ when typeof(T) == typeof(JobFormSearchable) => SearchIndex.JobForm,
            _ when typeof(T) == typeof(JobApplicationSearchable) => SearchIndex.JobApplication,
            _ => SearchIndex.Default
        };
    }
}
