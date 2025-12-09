using Application.Features.Jobs.JobApplications.Search;
using Application.Features.Jobs.JobForms.Search;
using Application.Features.Notifications.Search;
using Application.Features.Users.Search;
using DTO.Pagination;

namespace Application.Common.Search;

public interface ISearchClient<T> where T: class, ISearchable
{
    Task IndexAndRefreshAsync(T document, CancellationToken cancellationToken = default);
    Task IndexAsync(T document, CancellationToken cancellationToken = default);
    Task IndexManyAsync(IEnumerable<T> data, CancellationToken cancellationToken = default);
    Task IndexAndRefreshManyAsync(IEnumerable<T> data, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task DeleteAndRefreshAsync(int id, CancellationToken cancellationToken = default);
    Task DeleteAllAsync(CancellationToken cancellationToken = default);
    Task DeleteManyAsync(IEnumerable<T> data, CancellationToken cancellationToken = default);
    Task<bool> IndexExist(string index);
    Task<bool> CreateIndexIfNotExist(string index);
    Task<PaginatedList<UserSearchable>> SearchUsersAsync(IUserFullSearchCriteria criteria);
    Task<PaginatedList<NotificationSearchable>> SearchNotificationsForUserAsync(INotificationForUserFullSearchCriteria criteria);
    Task<PaginatedList<JobApplicationSearchable>> SearchJobApplicationsAsync(IJobApplicationFullSearchCriteria criteria);
    Task<PaginatedList<JobFormSearchable>> SearchJobFormsAsync(IJobFormFullSearchCriteria criteria);
}
