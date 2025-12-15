using Application.Common.MessageBroker;
using Application.Features.Enums.Queries;
using Application.Features.Notifications.Commands;
using Application.Features.Notifications.Queries;
using Application.Features.Notifications.Search;
using DTO.Enums.Notification;
using DTO.MessageBroker.Messages.Notification;
using DTO.Notification;
using DTO.Pagination;
using DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class NotificationController : ApiControllerBase
{
    private readonly IMessagePublisher _messagePublisher;

    public NotificationController(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    [HttpPut("{id}/change-status")]
    public async Task<IActionResult> ChangeStatus([FromRoute] int id, [FromBody] NotificationStatus status)
    {
        await Mediator.Send(new NotificationChangeStatusCommand(id, status));
        return Ok();
    }

    [HttpPut("user/mark-all-as-read")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        await Mediator.Send(new NotificationMarkAllAsReadCommand());
        return Ok();
    }

    [HttpGet("count/unread")]
    public async Task<NotificationUnreadCountResponse> GetUnreadCountForUser()
    {
        return await Mediator.Send(new NotificationGetUnreadCountQuery());
    }

    [HttpPost("search/user")]
    public async Task<PaginatedList<NotificationSearchable>> FullSearchForUser([FromBody] NotificationFullSearchForUserQuery request)
    {
        return await Mediator.Send(request);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("search/rebuild")]
    public async Task<IActionResult> RebuildSearchIndex()
    {
        await Mediator.Send(new NotificationInitiateSearchIndexRebuildCommand());
        return Ok();
    }

    [HttpGet("lookup/status")]
    public async Task<IReadOnlyCollection<ListItemBaseResponse>> GetStatuses()
    {
        return await Mediator.Send(new GetEnumValuesQuery(typeof(NotificationStatus)));
    }

    [AllowAnonymous]
    [HttpPost("{userId}/test")]
    public async Task<IActionResult> SendTestNotificationToUser([FromRoute] int userId, [FromQuery] string text)
    {
        await _messagePublisher.PublishAsync(new TestNotificationMessage(userId, text));
        return Ok();
    }
}
