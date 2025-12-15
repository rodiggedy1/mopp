using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using Application.Common.Payments;
using Domain.Interfaces;
using DTO.Payments;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payments.Commands;

public sealed record RenewSubscriptionCommand() : ICommand<PaymentsBaseResponse>;

public sealed class RenewSubscriptionCommandHandler : ICommandHandler<RenewSubscriptionCommand, PaymentsBaseResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILocalizationService _localizationService;
    private readonly IDateTime _dateTimeProvider;

    public RenewSubscriptionCommandHandler(
        IApplicationDbContext context,
        IPaymentProcessor paymentProcessor,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILocalizationService localizationService,
        IDateTime dateTimeProvider)
    {
        _context = context;
        _paymentProcessor = paymentProcessor;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _localizationService = localizationService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<PaymentsBaseResponse> Handle(RenewSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;
        
        var user = await _context.User.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        var result = await _paymentProcessor.RenewSubscriptionAsync(user!.StripeSubscriptionId!);
        if (!result)
            return new PaymentsBaseResponse(false, _localizationService.GetValue("renewSubscription.failedStripe.message"));

        user.RenewSubscription(_dateTimeProvider.Now);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new PaymentsBaseResponse(
            true,
            _localizationService.GetValue("renewSubscription.success.message")
        );
    }
}

public sealed class RenewSubscriptionCommandValidator : AbstractValidator<RenewSubscriptionCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _context;
    private readonly ILocalizationService _localizationService;

    public RenewSubscriptionCommandValidator(
        ICurrentUserService currentUserService,
        IApplicationDbContext context,
        ILocalizationService localizationService)
    {
        _currentUserService = currentUserService;
        _context = context;
        _localizationService = localizationService;

        RuleFor(x => _currentUserService.UserId)
            .NotNull()
            .WithMessage(_ => _localizationService.GetValue("renewSubscription.currentUserNotFound.message"))
            .DependentRules(() => {
                RuleFor(x => x)
                    .MustAsync(UserMustExist)
                    .WithMessage(_ => _localizationService.GetValue("renewSubscription.userNotFound.message"))
                    .DependentRules(() => {
                        RuleFor(x => x)
                            .MustAsync(UserHasSubscription)
                            .WithMessage(_ => _localizationService.GetValue("renewSubscription.noSubscription.message"));
                    });
            });
    }

    private async Task<bool> UserMustExist(RenewSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;

        return await _context.User.AnyAsync(u => u.Id == userId, cancellationToken);
    }

    private async Task<bool> UserHasSubscription(RenewSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;
        
        var user = await _context.User.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        
        return user != null && !string.IsNullOrEmpty(user.StripeSubscriptionId);
    }
}