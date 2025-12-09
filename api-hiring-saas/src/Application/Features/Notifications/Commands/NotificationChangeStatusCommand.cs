using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository.Base;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Notifications;
using DTO.Enums.Notification;
using FluentValidation;

namespace Application.Features.Notifications.Commands;

public sealed record NotificationChangeStatusCommand(
    int Id,
    NotificationStatus Status) : ICommand;

public sealed class NotificationChangeStatusCommandHandler : ICommandHandler<NotificationChangeStatusCommand>
{
    private readonly IRepository<Notification> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationChangeStatusCommandHandler(
        IRepository<Notification> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(NotificationChangeStatusCommand command, CancellationToken cancellationToken)
    {
        var notification = await _repository.GetSafeAsync(command.Id);

        notification.UpdateStatus(command.Status);
        _repository.Update(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class NotificationChangeStatusCommandValidator : AbstractValidator<NotificationChangeStatusCommand>
{
    public NotificationChangeStatusCommandValidator()
    {
        RuleFor(cmd => cmd.Id)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(cmd => cmd.Status)
            .NotEmpty()
            .IsInEnum();
    }
}
