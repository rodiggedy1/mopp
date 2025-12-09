using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository.Base;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Notifications;
using Domain.Interfaces;
using DTO.Enums.Notification;
using FluentValidation;

namespace Application.Features.Notifications.Commands;
public sealed record NotificationCreateCommand(
    int UserId,
    string Title,
    string? Description,
    string Data,
    NotificationType Type) : ICommand;

public sealed class NotificationCreateCommandHandler : ICommandHandler<NotificationCreateCommand>
{
    private readonly IRepository<Notification> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTimeProvider;

    public NotificationCreateCommandHandler(
        IRepository<Notification> repository,
        IUnitOfWork unitOfWork,
        IDateTime dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(NotificationCreateCommand command, CancellationToken cancellationToken)
    {
        Notification newMessageNotification = Notification.Create(
            command.UserId, 
            _dateTimeProvider, 
            command.Type, 
            command.Title, 
            command.Description, 
            command.Data);

        await _repository.AddAsync(newMessageNotification);
        await _unitOfWork.SaveChangesAsync();
    }
}

public sealed class NotificationCreateCommandValidator : AbstractValidator<NotificationCreateCommand>
{
    public NotificationCreateCommandValidator()
    {
        RuleFor(cmd => cmd.UserId)
           .NotEmpty();

        RuleFor(cmd => cmd.Title)
            .NotEmpty();

        RuleFor(cmd => cmd.Data)
            .NotEmpty();

        RuleFor(cmd => cmd.Type)
            .NotEmpty()
            .IsInEnum();
    }
}
