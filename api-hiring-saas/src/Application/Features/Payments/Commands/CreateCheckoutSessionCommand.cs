using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using Application.Common.Payments;

namespace Application.Features.Payments.Commands;

public sealed record CreateCheckoutSessionCommand() : ICommand<string>;

public sealed class CreateCheckoutSessionCommandHandler : ICommandHandler<CreateCheckoutSessionCommand, string>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _context;
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly ILocalizationService _localizationService;

    public CreateCheckoutSessionCommandHandler(
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

    public async Task<string> Handle(CreateCheckoutSessionCommand command, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;
        
        var user = await _context.User.FindAsync(userId, cancellationToken);
        if (user == null)
            throw new NotFoundException(_localizationService.GetValue("user.notFound.error.message"));

        bool includeTrial = !user.HasUsedTrial();

        var checkoutUrl = await _paymentProcessor.CreateCheckoutSessionAsync(
            userId, 
            user.Email!,
            includeTrial);

        return checkoutUrl;
    }
}
