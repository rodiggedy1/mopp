using Application.Features.Payments.Commands;
using Application.Features.Payments.Queries;
using DTO.Payments;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class PaymentController : ApiControllerBase
{
    /// <summary>
    /// Creates a Stripe checkout session for subscription with trial.
    /// </summary>
    /// <param name="request">The checkout session request with success and cancel URLs.</param>
    /// <returns>The checkout session URL to redirect the user to.</returns>
    [HttpGet("create-checkout")]
    public async Task<IActionResult> CreateCheckoutSession()
    {
        var command = new CreateCheckoutSessionCommand();
        var checkoutUrl = await Mediator.Send(command);
        
        return Ok(new { url = checkoutUrl });
    }


    /// <summary>
    /// Creates a Stripe billing portal session to update details.
    /// </summary>
    /// <param name="request">The billing portal session request with redirect URL.</param>
    /// <returns>The billing portal session URL to redirect the user to.</returns>
    [HttpGet("billing-portal-session")]
    public async Task<IActionResult> CreateBillingPortalSession()
    {
        var command = new CreateBillingPortalSessionCommand();
        var url = await Mediator.Send(command);

        return Ok(new { url = url });
    }

    /// <summary>
    /// Verifies payment and gets subscription details after checkout completion.
    /// </summary>
    /// <param name="sessionId">The Stripe checkout session ID.</param>
    /// <returns>Payment verification result with subscription details.</returns>
    [HttpGet("verify-payment")]
    public async Task<VerifyPaymentResponse> VerifyPayment([FromQuery] string sessionId)
    {
        var command = new VerifyPaymentCommand(sessionId);
        return await Mediator.Send(command);
    }

    /// <summary>
    /// Gets the current user's subscription status.
    /// </summary>
    /// <returns>Current subscription status including trial information.</returns>
    [HttpGet("subscription-status")]
    public async Task<SubscriptionStatusResponse> GetSubscriptionStatus()
    {
        return await Mediator.Send(new GetSubscriptionStatusQuery());
    }

    /// <summary>
    /// Gets the current user's payment status and recent transactions.
    /// </summary>
    /// <returns>User payment status including subscription details and recent transactions.</returns>
    [HttpGet("status")]
    public async Task<UserPaymentStatusResponse> GetPaymentStatus()
    {
        return await Mediator.Send(new GetUserPaymentStatusQuery());
    }

    /// <summary>
    /// Cancels the current user's active subscription.
    /// </summary>
    /// <returns>Result of the cancellation operation including a success status and message.</returns>
    [HttpPost("cancel-subscription")]
    public async Task<IActionResult> CancelSubscription()
    {
        var result = await Mediator.Send(new CancelSubscriptionCommand());
        return Ok(result);
    }

    /// <summary>
    /// Extends the user's trial period by 30 days. Can only be used once per account.
    /// </summary>
    /// <returns>Result of the trial extension operation including a success status and message.</returns>
    [HttpPost("extend-trial")]
    public async Task<IActionResult> ExtendTrial()
    {
        var result = await Mediator.Send(new ExtendTrialCommand());
        return Ok(result);
    }

    /// <summary>
    /// Temporarily pauses the current user's subscription for 60 days.
    /// </summary>
    /// <returns>Result of the subscription pause operation including a success status and message.</returns>
    [HttpPost("pause-subscription")]
    public async Task<IActionResult> PauseSubscription()
    {
        var result = await Mediator.Send(new PauseSubscriptionCommand());
        return Ok(result);
    }

    /// <summary>
    /// Applies a 50% discount to the user's subscription for 3 billing cycles. Can only be used once per account.
    /// </summary>
    /// <returns>Result of the discount application including a success status and message.</returns>
    [HttpPost("apply-discount")]
    public async Task<IActionResult> ApplyDiscount()
    {
        var result = await Mediator.Send(new ApplyDiscountCommand());
        return Ok(result);
    }

    /// <summary>
    /// Attempts to renew a failed subscription using the customer's saved payment method.
    /// </summary>
    /// <returns>Result of the renewal attempt.</returns>
    [HttpPost("renew-subscription")]
    public async Task<IActionResult> RenewSubscription()
    {
        var result = await Mediator.Send(new RenewSubscriptionCommand());
        return Ok(result);
    }
}