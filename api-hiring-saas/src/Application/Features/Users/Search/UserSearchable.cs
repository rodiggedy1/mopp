using Application.Common.Search;
using DTO.User;

namespace Application.Features.Users.Search;

public sealed record UserSearchable : UserInfoResponse, ISearchable
{
}
