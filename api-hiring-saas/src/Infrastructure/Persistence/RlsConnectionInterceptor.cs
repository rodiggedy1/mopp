using Application.Common.Interfaces;
using Domain.Entities.User;
using DTO.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using StackExchange.Redis;
using System.Data.Common;

namespace Infrastructure.Persistence;

public class RlsConnectionInterceptor : DbConnectionInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    public RlsConnectionInterceptor(
        ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        SetRlsVariablesAsync(connection).GetAwaiter().GetResult();
    }

    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData,
                                                     CancellationToken cancellationToken = default)
    {
            await SetRlsVariablesAsync(connection);
    }

    private async Task SetRlsVariablesAsync(DbConnection connection)
    {
        using var cmd = connection.CreateCommand();

        var userId = _currentUserService?.UserId ?? 0;
        bool isAdmin = _currentUserService?.IsAdmin ?? userId <= 1 ? true : false;

        cmd.CommandText = $@"
                SET app.current_user_id = {userId};
                SET app.is_admin = {(isAdmin ? "true" : "false")};";

        await cmd.ExecuteNonQueryAsync();
    }
}
