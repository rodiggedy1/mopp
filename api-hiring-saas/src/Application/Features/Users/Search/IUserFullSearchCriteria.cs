using Application.Common.Search;
using DTO.User.Search;

namespace Application.Features.Users.Search;

public interface IUserFullSearchCriteria : IFullSearchCriteria<UserFullSearchSortField>
{
    public int? Status { get; }
}
