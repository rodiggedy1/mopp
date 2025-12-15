using Application.Common.Interfaces.Repository.Base;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using Domain.Entities.Notifications;
using DTO.Notification;

namespace Application.Features.Notifications.Queries;

public sealed record NotificationGetAllQuery : IQuery<IReadOnlyCollection<NotificationResponse>>;

public sealed record NotificationGetAllQueryHandler : IQueryHandler<NotificationGetAllQuery, IReadOnlyCollection<NotificationResponse>>
{
    private readonly IRepository<Notification> _repository;
    private readonly IMapper _mapper;

    public NotificationGetAllQueryHandler(
        IRepository<Notification> repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<NotificationResponse>> Handle(NotificationGetAllQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _repository.GetAllAsync(cancellationToken);

        return _mapper.Map<IReadOnlyCollection<NotificationResponse>>(notifications);
    }
}
