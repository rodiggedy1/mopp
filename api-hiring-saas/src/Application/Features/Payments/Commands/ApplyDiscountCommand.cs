using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using Application.Common.Payments;
using DTO.Payments;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payments.Commands;

public sealed record ApplyDiscountCommand() : ICommand<PaymentsBaseResponse>;

public sealed class ApplyDiscountCommandHandler : ICommandHandler<ApplyDiscountCommand, PaymentsBaseResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILocalizationService _localizationService;

    public ApplyDiscountCommandHandler(
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

    public async Task<PaymentsBaseResponse> Handle(ApplyDiscountCommand command, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;
        
        var user = await _context.User.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        var result = await _paymentProcessor.ApplyDiscountAsync(user!.StripeSubscriptionId!, 0.5m, 3);
        if (!result)
            throw new InvalidOperationException(_localizationService.GetValue("applyDiscount.failedStripe.message"));

        user.MarkDiscountUsed();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new PaymentsBaseResponse(
            true,
            _localizationService.GetValue("applyDiscount.success.message")
        );
    }
}

public sealed class ApplyDiscountCommandValidator : AbstractValidator<ApplyDiscountCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _context;
    private readonly ILocalizationService _localizationService;

    public ApplyDiscountCommandValidator(
        ICurrentUserService currentUserService,
        IApplicationDbContext context,
        ILocalizationService localizationService)
    {
        _currentUserService = currentUserService;
        _context = context;
        _localizationService = localizationService;

        RuleFor(x => _currentUserService.UserId)
            .NotNull()
            .WithMessage(_ => _localizationService.GetValue("applyDiscount.currentUserNotFound.message"))
            .DependentRules(() => {
                RuleFor(x => x)
                    .MustAsync(UserMustExist)
                    .WithMessage(_ => _localizationService.GetValue("applyDiscount.userNotFound.message"))
                    .DependentRules(() => {
                        RuleFor(x => x)
                            .MustAsync(UserHasNotUsedDiscount)
                            .WithMessage(_ => _localizationService.GetValue("applyDiscount.alreadyUsed.message"));
                        
                        RuleFor(x => x)
                            .MustAsync(UserHasActiveSubscription)
                            .WithMessage(_ => _localizationService.GetValue("applyDiscount.noSubscription.message"));
                    });
            });
    }

    private async Task<bool> UserMustExist(ApplyDiscountCommand command, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;

        return await _context.User.AnyAsync(u => u.Id == userId, cancellationToken);
    }

    private async Task<bool> UserHasNotUsedDiscount(ApplyDiscountCommand command, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;

        var user = await _context.User.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return user != null && !user.HasUsedDiscount;
    }

    private async Task<bool> UserHasActiveSubscription(ApplyDiscountCommand command, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;

        var user = await _context.User.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return user != null && !string.IsNullOrEmpty(user.StripeSubscriptionId);
    }
}