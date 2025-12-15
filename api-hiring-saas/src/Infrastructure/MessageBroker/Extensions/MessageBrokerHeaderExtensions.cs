using Application.Features.Authentication.Core;
using MassTransit;

namespace Infrastructure.MessageBroker.Extensions;

public static class MessageBrokerHeaderExtensions
{
    public static void SetAccessToken(this SendContext ctx, string token) => ctx.Headers.Set(AuthConstants.AccessTokenName, token);
    public static string? GetAccessToken(this MessageContext ctx) =>
        ctx.Headers.TryGetHeader(AuthConstants.AccessTokenName, out var token) && token != null ? (string)token : default;
}