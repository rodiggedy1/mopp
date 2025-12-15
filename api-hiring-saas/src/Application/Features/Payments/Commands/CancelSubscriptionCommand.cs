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

public sealed record CancelSubscriptionCommand() : ICommand<PaymentsBaseResponse>;

public sealed class CancelSubscriptionCommandHandler : ICommandHandler<CancelSubscriptionCommand, PaymentsBaseResponse>
{
    private readonly IDateTime _dateTimeProvider;
    private readonly IApplicationDbContext _context;
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILocalizationService _localizationService;

    public CancelSubscriptionCommandHandler(
        IDateTime dateTimeProvider,
        IApplicationDbContext context,
        IPaymentProcessor paymentProcessor,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILocalizationService localizationService)
    {
        _dateTimeProvider = dateTimeProvider;
        _context = context;
        _paymentProcessor = paymentProcessor;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _localizationService = localizationService;
    }

    public async Task<PaymentsBaseResponse> Handle(CancelSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;
        
        var user = await _context.User.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        var stripeResult = await _paymentProcessor.CancelSubscriptionAsync(user!.StripeSubscriptionId!);
        if (!stripeResult)
            throw new InvalidOperationException(_localizationService.GetValue("cancelSubscription.failedStripeCancel.message"));

        user.EndSubscription(_dateTimeProvider.Now);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new PaymentsBaseResponse(
            true,
            _localizationService.GetValue("cancelSubscription.success.message")
        );
    }
}

public sealed class CancelSubscriptionCommandValidator : AbstractValidator<CancelSubscriptionCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly ILocalizationService _localizationService;

    public CancelSubscriptionCommandValidator(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        ILocalizationService localizationService)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _localizationService = localizationService;

        RuleFor(x => _currentUserService.UserId)
            .NotNull()
            .WithMessage(_ => _localizationService.GetValue("cancelSubscription.currentUserNotFound.message"))
            .DependentRules(() => {
                RuleFor(x => x)
                    .MustAsync(UserMustExist)
                    .WithMessage(_ => _localizationService.GetValue("cancelSubscription.userNotFound.message"))
                    .DependentRules(() => {
                        RuleFor(x => x)
                            .MustAsync(UserHasActiveSubscription)
                            .WithMessage(_ => _localizationService.GetValue("cancelSubscription.noActiveSubscription.message"));
                    });
            });
    }

    private async Task<bool> UserMustExist(CancelSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;

        return await _dbContext.User.AnyAsync(u => u.Id == userId, cancellationToken);
    }

    private async Task<bool> UserHasActiveSubscription(CancelSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;

        var user = await _dbContext.User.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return user != null && !string.IsNullOrEmpty(user.StripeSubscriptionId);
    }
}
