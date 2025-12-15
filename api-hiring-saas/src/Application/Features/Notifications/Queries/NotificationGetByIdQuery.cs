using Application.Common.Caching;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Repository.Base;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using Domain.Entities.Notifications;
using DTO.Notification;
using FluentValidation;

namespace Application.Features.Notifications.Queries;

public sealed record NotificationGetByIdQuery(int NotificationId) : IQuery<NotificationResponse>;

public sealed class NotificationGetByIdQueryHandler : IQueryHandler<NotificationGetByIdQuery, NotificationResponse>
{
    private readonly IRepository<Notification> _repository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public NotificationGetByIdQueryHandler(
        IRepository<Notification> repository,
        IMapper mapper,
        ICacheService cacheService)
    {
        _repository = repository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<NotificationResponse> Handle(NotificationGetByIdQuery query, CancellationToken cancellationToken)
    {
        var notification = await _repository.GetAsync(query.NotificationId, cancellationToken);

        if (notification == null) 
             throw new NotFoundException("Notification not found");

        return _mapper.Map<NotificationResponse>(notification);
    }
}

public sealed class NotificationGetByIdQueryValidator : AbstractValidator<NotificationGetByIdQuery>
{
    public NotificationGetByIdQueryValidator()
    {
        RuleFor(qry => qry.NotificationId)
            .NotEmpty();
    }
}
