using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using Application.Common.Payments;
using DTO.Payments;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payments.Commands;

public sealed record PauseSubscriptionCommand() : ICommand<PaymentsBaseResponse>;

public sealed class PauseSubscriptionCommandHandler : ICommandHandler<PauseSubscriptionCommand, PaymentsBaseResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILocalizationService _localizationService;

    public PauseSubscriptionCommandHandler(
        IApplicationDbContext context,
        IPaymentProcessor paymentProcessor,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILocalizationService localizationService)
    {
        _context = context;
        _paymentProcessor = paymentProcessor;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _localizationService = localizationService;
    }

    public async Task<PaymentsBaseResponse> Handle(PauseSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;
        
        var user = await _context.User.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        var result = await _paymentProcessor.PauseSubscriptionAsync(user!.StripeSubscriptionId!, 60);
        if (!result)
            throw new InvalidOperationException(_localizationService.GetValue("pauseSubscription.failedStripe.message"));

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new PaymentsBaseResponse(
            true,
            _localizationService.GetValue("pauseSubscription.success.message")
        );
    }
}

public sealed class PauseSubscriptionCommandValidator : AbstractValidator<PauseSubscriptionCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _context;
    private readonly ILocalizationService _localizationService;

    public PauseSubscriptionCommandValidator(
        ICurrentUserService currentUserService,
        IApplicationDbContext context,
        ILocalizationService localizationService)
    {
        _currentUserService = currentUserService;
        _context = context;
        _localizationService = localizationService;

        RuleFor(x => _currentUserService.UserId)
            .NotNull()
            .WithMessage(_ => _localizationService.GetValue("pauseSubscription.currentUserNotFound.message"))
            .DependentRules(() => {
                RuleFor(x => x)
                    .MustAsync(UserMustExist)
                    .WithMessage(_ => _localizationService.GetValue("pauseSubscription.userNotFound.message"))
                    .DependentRules(() => {
                        RuleFor(x => x)
                            .MustAsync(UserHasActiveSubscription)
                            .WithMessage(_ => _localizationService.GetValue("pauseSubscription.noSubscription.message"));
                    });
            });
    }

    private async Task<bool> UserMustExist(PauseSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;

        return await _context.User.AnyAsync(u => u.Id == userId, cancellationToken);
    }

    private async Task<bool> UserHasActiveSubscription(PauseSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;
        
        var user = await _context.User.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        
        return user != null && !string.IsNullOrEmpty(user.StripeSubscriptionId);
    }
}