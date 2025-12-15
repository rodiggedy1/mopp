using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using DTO.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Queries;

public sealed record UserGetRolesQuery() : IQuery<IReadOnlyCollection<ListItemBaseResponse>>;

public sealed class UserGetRolesQueryHandler : IQueryHandler<UserGetRolesQuery, IReadOnlyCollection<ListItemBaseResponse>>
{
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public UserGetRolesQueryHandler(
        RoleManager<IdentityRole<int>> roleManager)
    {
        _roleManager = roleManager;
    }
    public async Task<IReadOnlyCollection<ListItemBaseResponse>> Handle(UserGetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = _roleManager.Roles;

        var roleListItems = await roles
            .Select(role => new ListItemBaseResponse
            {
                Id = role.Id,
                Name = role.Name
            })
            .ToListAsync(cancellationToken);

        return roleListItems;
    }
}
