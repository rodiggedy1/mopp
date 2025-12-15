using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using Application.Common.Payments;

namespace Application.Features.Payments.Commands;

public sealed record CreateBillingPortalSessionCommand() : ICommand<string>;

public sealed class CreateBillingPortalSessionCommandHandler : ICommandHandler<CreateBillingPortalSessionCommand, string>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _context;
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly ILocalizationService _localizationService;

    public CreateBillingPortalSessionCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext context,
        IPaymentProcessor paymentProcessor,
        ILocalizationService localizationService)
    {
        _currentUserService = currentUserService;
        _context = context;
        _paymentProcessor = paymentProcessor;
        _localizationService = localizationService;
    }

    public async Task<string> Handle(CreateBillingPortalSessionCommand command, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;

        var user = await _context.User.FindAsync(userId, cancellationToken);
        if (user == null)
            throw new NotFoundException(_localizationService.GetValue("user.notFound.error.message"));

        if(string.IsNullOrEmpty(user.StripeCustomerId))
            throw new ApplicationException(_localizationService.GetValue("user.subscription.missing.error.message"));
        
        var checkoutUrl = await _paymentProcessor.CreateBillingPortalSessionAsync(
            user.StripeCustomerId,
            userId);

        return checkoutUrl;
    }
}
