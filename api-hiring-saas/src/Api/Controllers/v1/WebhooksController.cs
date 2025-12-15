using Application.Common.MessageBroker;
using Application.Features.Payments.Commands;
using DTO.MessageBroker.Messages.System;
using Infrastructure.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Api.Controllers.v1
{
    public class WebhooksController : ApiControllerBase
    {
        private readonly ILogger<WebhooksController> _logger;
        private readonly StripeConfig _stripeConfig;
        private readonly IMessagePublisher _messagePublisher;

        public WebhooksController(
            ILogger<WebhooksController> logger,
            IOptions<StripeConfig> stripeConfig,
            IMessagePublisher messagePublisher)
        {
            _logger = logger;
            _stripeConfig = stripeConfig.Value;
            _messagePublisher = messagePublisher;
        }

        [AllowAnonymous]
        [HttpPost("redis-error-notification")]
        public async Task<IActionResult> TestSendingRedisErrorWebhook([FromForm] string errorMessage)
        {
            await _messagePublisher.PublishAsync(new RestartRedisMessage(errorMessage));
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("stripe")]
        public async Task<IActionResult> HandleStripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _stripeConfig.WebhookSecret,
                    throwOnApiVersionMismatch: false
                );

                _logger.LogInformation("Processing Stripe webhook: {EventType}", stripeEvent.Type);

                switch (stripeEvent.Type)
                {
                    case Events.CheckoutSessionCompleted:
                        var session = stripeEvent.Data.Object as Session;
                        if (session != null)
                        {
                            _logger.LogInformation("Checkout session completed: {SessionId}", session.Id);
                            // The verification will be handled by the verify-payment endpoint
                        }
                        break;

                    case Events.CustomerSubscriptionCreated:
                    case Events.CustomerSubscriptionUpdated:
                        var subscription = stripeEvent.Data.Object as Subscription;
                        if (subscription != null)
                        {
                            await Mediator.Send(new HandleSubscriptionUpdatedCommand(
                                subscription.Id,
                                subscription.CustomerId,
                                subscription.Status,
                                subscription.TrialEnd.HasValue 
                                    ? subscription.TrialEnd.Value 
                                    : null,
                                subscription.CurrentPeriodEnd
                            ));
                        }
                        break;

                    case Events.CustomerSubscriptionDeleted:
                        var deletedSubscription = stripeEvent.Data.Object as Subscription;
                        if (deletedSubscription != null)
                        {
                            await Mediator.Send(new HandleSubscriptionUpdatedCommand(
                                deletedSubscription.Id,
                                deletedSubscription.CustomerId,
                                "canceled",
                                null,
                                deletedSubscription.CurrentPeriodEnd
                            ));
                        }
                        break;

                    case Events.CustomerSubscriptionPaused:
                        var pausedSubscription = stripeEvent.Data.Object as Subscription;
                        if (pausedSubscription != null)
                        {
                            await Mediator.Send(new HandleSubscriptionUpdatedCommand(
                                pausedSubscription.Id,
                                pausedSubscription.CustomerId,
                                pausedSubscription.Status,
                                pausedSubscription.TrialEnd.HasValue
                                    ? pausedSubscription.TrialEnd.Value
                                    : null,
                                pausedSubscription.CurrentPeriodEnd
                            ));
                        }
                        break;

                    case Events.CustomerSubscriptionResumed:
                        var resumedSubscription = stripeEvent.Data.Object as Subscription;
                        if (resumedSubscription != null)
                        {
                            await Mediator.Send(new HandleSubscriptionUpdatedCommand(
                                resumedSubscription.Id,
                                resumedSubscription.CustomerId,
                                resumedSubscription.Status,
                                resumedSubscription.TrialEnd.HasValue
                                    ? resumedSubscription.TrialEnd.Value
                                    : null,
                                resumedSubscription.CurrentPeriodEnd
                            ));
                        }
                        break;

                    case Events.InvoicePaymentSucceeded:
                        var invoice = stripeEvent.Data.Object as Invoice;
                        if (invoice != null && invoice.SubscriptionId != null)
                        {
                            // Handle successful subscription payment
                            _logger.LogInformation("Invoice payment succeeded for subscription: {SubscriptionId}", invoice.SubscriptionId);
                        }
                        break;

                    case Events.InvoicePaymentFailed:
                        var failedInvoice = stripeEvent.Data.Object as Invoice;
                        if (failedInvoice != null && failedInvoice.SubscriptionId != null)
                        {
                            // Handle failed subscription payment
                            _logger.LogWarning("Invoice payment failed for subscription: {SubscriptionId}", failedInvoice.SubscriptionId);
                        }
                        break;

                    default:
                        _logger.LogInformation("Unhandled event type: {EventType}", stripeEvent.Type);
                        break;
                }

                return Ok();
            }
            catch (StripeException e)
            {
                _logger.LogError(e, "Stripe webhook error: {Message}", e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Webhook processing error: {Message}", e.Message);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
