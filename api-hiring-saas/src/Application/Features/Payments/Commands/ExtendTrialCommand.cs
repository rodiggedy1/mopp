using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using Application.Common.Payments;
using DTO.Payments;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payments.Commands;

public sealed record ExtendTrialCommand() : ICommand<PaymentsBaseResponse>;

public sealed class ExtendTrialCommandHandler : ICommandHandler<ExtendTrialCommand, PaymentsBaseResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILocalizationService _localizationService;

    public ExtendTrialCommandHandler(
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

    public async Task<PaymentsBaseResponse> Handle(ExtendTrialCommand command, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;
        
        var user = await _context.User.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        var result = await _paymentProcessor.ExtendTrialAsync(user!.StripeSubscriptionId!, 30);
        if (!result)
            throw new InvalidOperationException(_localizationService.GetValue("extendTrial.failedStripe.message"));

        user.MarkTrialExtensionUsed();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new PaymentsBaseResponse(
            true,
            _localizationService.GetValue("extendTrial.success.message")
        );
    }
}

public sealed class ExtendTrialCommandValidator : AbstractValidator<ExtendTrialCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly ILocalizationService _localizationService;

    public ExtendTrialCommandValidator(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        ILocalizationService localizationService)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _localizationService = localizationService;

        RuleFor(x => _currentUserService.UserId)
            .NotNull()
            .WithMessage(_ => _localizationService.GetValue("extendTrial.currentUserNotFound.message"))
            .DependentRules(() => {
                RuleFor(x => x)
                    .MustAsync(UserMustExist)
                    .WithMessage(_ => _localizationService.GetValue("extendTrial.userNotFound.message"))
                    .DependentRules(() => {
                        RuleFor(x => x)
                            .MustAsync(UserHasNotUsedTrialExtension)
                            .WithMessage(_ => _localizationService.GetValue("extendTrial.alreadyUsed.message"));
                        
                        RuleFor(x => x)
                            .MustAsync(UserHasActiveSubscription)
                            .WithMessage(_ => _localizationService.GetValue("extendTrial.noSubscription.message"));
                    });
            });
    }

    private async Task<bool> UserMustExist(ExtendTrialCommand command, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;

        return await _dbContext.User.AnyAsync(u => u.Id == userId, cancellationToken);
    }

    private async Task<bool> UserHasNotUsedTrialExtension(ExtendTrialCommand command, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;
        
        var user = await _dbContext.User.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        
        return user != null && !user.HasUsedTrialExtension;
    }

    private async Task<bool> UserHasActiveSubscription(ExtendTrialCommand command, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;
        
        var user = await _dbContext.User.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        
        return user != null && !string.IsNullOrEmpty(user.StripeSubscriptionId);
    }
}