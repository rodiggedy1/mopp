using Application.Common.Payments;
using Domain.Interfaces;
using DTO.Payments;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;

namespace Infrastructure.Payments
{
    public class StripePaymentService : IPaymentProcessor
    {
        private readonly StripeConfig _config;
        private readonly ILogger<StripePaymentService> _logger;
        private readonly IDateTime _dateTimeProvider;

        public StripePaymentService(
            StripeConfig config,
            ILogger<StripePaymentService> logger,
            IDateTime dateTimeProvider)
        {
            if (config == null)
                throw new ArgumentNullException("Stripe config not found");

            StripeConfiguration.ApiKey = config.Key;
            _config = config;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<string> CreatePaymentIntentAsync(decimal amount, string currency, string customerId)
        {
            var service = new PaymentIntentService();
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Convert to cents
                Currency = currency,
                Customer = customerId,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };

            var paymentIntent = await service.CreateAsync(options);
            return paymentIntent.Id;
        }

        public async Task<string> CreateCustomerAsync(string email, string name)
        {
            var service = new CustomerService();
            var options = new CustomerCreateOptions
            {
                Email = email,
                Name = name,
            };

            var customer = await service.CreateAsync(options);
            return customer.Id;
        }

        public async Task<bool> VerifyPaymentIntentAsync(string paymentIntentId)
        {
            try
            {
                var service = new PaymentIntentService();

                var paymentIntent = await service.GetAsync(paymentIntentId);
                return paymentIntent?.Status == "succeeded";
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> CreateCheckoutSessionAsync(int userId, string userEmail, bool includeTrial = true)
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    ClientReferenceId = userId.ToString(),
                    CustomerEmail = userEmail,
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            Price = _config.SubscriptionPriceId,
                            Quantity = 1,
                        },
                    },
                    Mode = "subscription",
                    SuccessUrl = $"{_config.SuccessUrl}?session_id={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = _config.CancelUrl,
                };

                if (includeTrial)
                {
                    options.SubscriptionData = new SessionSubscriptionDataOptions
                    {
                        TrialPeriodDays = 7, // 7-day trial
                        TrialSettings = new SessionSubscriptionDataTrialSettingsOptions
                        {
                            EndBehavior = new SessionSubscriptionDataTrialSettingsEndBehaviorOptions
                            {
                                MissingPaymentMethod = "cancel" // Cancel if no payment method
                            }
                        }
                    };
                }

                var service = new SessionService();
                var session = await service.CreateAsync(options);
                return session.Url;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Error creating checkout session for user {UserId}: {Message}", userId, ex.Message);
                throw;
            }
        }


        public async Task<string> CreateBillingPortalSessionAsync(string stripeCustomerId, int userId)
        {
            try
            {
                var options = new Stripe.BillingPortal.SessionCreateOptions
                {
                    Customer = stripeCustomerId,
                    ReturnUrl = _config.SubscriptionUrl
                };

                var service = new Stripe.BillingPortal.SessionService();
                var session = await service.CreateAsync(options);
                return session.Url;
            }

            catch (StripeException ex)
            {
                _logger.LogError(ex, "Error creating billing portal session for user {UserId}: {Message}", userId, ex.Message);
                throw;
            }
        }

        public async Task<CheckoutSessionResult> VerifyCheckoutSessionAsync(string sessionId)
        {
            try
            {
                // Retrieve the session
                var sessionService = new SessionService();
                var session = await sessionService.GetAsync(sessionId);

                if (session.PaymentStatus == "paid" || session.Status == "complete")
                {
                    var userId = int.Parse(session.ClientReferenceId!);

                    // Get subscription details
                    var subscriptionService = new SubscriptionService();
                    var subscription = await subscriptionService.GetAsync(session.SubscriptionId);

                    // Calculate trial end date
                    DateTime? trialEndsAt = null;
                    if (subscription.TrialEnd.HasValue)
                    {
                        trialEndsAt = subscription.TrialEnd.Value;
                    }

                    return new CheckoutSessionResult(
                        IsSuccess: true,
                        UserId: userId,
                        Email: session.CustomerEmail,
                        SubscriptionStatus: subscription.Status,
                        SubscriptionId: session.SubscriptionId,
                        CustomerId: session.CustomerId,
                        TrialEndsAt: trialEndsAt
                    );
                }
                else
                {
                    return new CheckoutSessionResult(
                        IsSuccess: false,
                        Error: $"Payment not completed. Status: {session.PaymentStatus}"
                    );
                }
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Error verifying checkout session {SessionId}: {Message}", sessionId, ex.Message);
                return new CheckoutSessionResult(
                    IsSuccess: false,
                    Error: ex.Message
                );
            }
        }

        public async Task<SubscriptionStatusResult> GetSubscriptionStatusAsync(string subscriptionId)
        {
            try
            {
                var service = new SubscriptionService();
                var subscription = await service.GetAsync(subscriptionId);

                return new SubscriptionStatusResult(
                    Status: subscription.Status,
                    TrialEnd: subscription.TrialEnd.HasValue
                        ? subscription.TrialEnd.Value
                        : null,
                    CurrentPeriodEnd: subscription.CurrentPeriodEnd
                );
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Error getting subscription status {SubscriptionId}: {Message}", subscriptionId, ex.Message);
                return new SubscriptionStatusResult(
                    Status: "error",
                    Error: ex.Message
                );
            }
        }

        public async Task<bool> CancelSubscriptionAsync(string subscriptionId)
        {
            try
            {
                var service = new SubscriptionService();
                var options = new SubscriptionCancelOptions
                {
                    InvoiceNow = true,
                    Prorate = true
                };
                var canceled = await service.CancelAsync(subscriptionId, options);
                return canceled.Status == "canceled";
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Error canceling subscription {SubscriptionId}: {Message}", subscriptionId, ex.Message);
                return false;
            }
        }

        public async Task<bool> ExtendTrialAsync(string subscriptionId, int days)
        {
            try
            {
                var service = new SubscriptionService();
                var subscription = await service.GetAsync(subscriptionId);
                
                DateTime newTrialEnd;
                
                if (subscription.Status == "trialing" && subscription.TrialEnd.HasValue)
                {
                    newTrialEnd = subscription.TrialEnd.Value.AddDays(days);
                }
                else
                {
                    newTrialEnd = _dateTimeProvider.Now.AddDays(days);
                }
                
                var options = new SubscriptionUpdateOptions
                {
                    TrialEnd = newTrialEnd
                };
                
                var updatedSubscription = await service.UpdateAsync(subscriptionId, options);
                return updatedSubscription.Status == "trialing";
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Error extending trial for subscription {SubscriptionId}: {Message}", subscriptionId, ex.Message);
                return false;
            }
        }

        public async Task<bool> PauseSubscriptionAsync(string subscriptionId, int days)
        {
            try
            {
                var service = new SubscriptionService();
                
                var pauseOptions = new SubscriptionUpdateOptions
                {
                    PauseCollection = new SubscriptionPauseCollectionOptions
                    {
                        Behavior = "void",
                        ResumesAt = _dateTimeProvider.Now.AddDays(days)
                    }
                };
                
                var pausedSubscription = await service.UpdateAsync(subscriptionId, pauseOptions);
                return pausedSubscription.PauseCollection?.ResumesAt != null;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Error pausing subscription {SubscriptionId}: {Message}", subscriptionId, ex.Message);
                return false;
            }
        }

        public async Task<bool> ApplyDiscountAsync(string subscriptionId, decimal percent, int billingCycles)
        {
            try
            {
                var couponService = new CouponService();
                var couponOptions = new CouponCreateOptions
                {
                    PercentOff = (long)(percent * 100),
                    Duration = "repeating",
                    DurationInMonths = billingCycles,
                    Name = $"Discount_{percent*100}percent_{Guid.NewGuid().ToString("N").Substring(0, 8)}"
                };
                
                var coupon = await couponService.CreateAsync(couponOptions);
                
                var subscriptionService = new SubscriptionService();
                var subscriptionOptions = new SubscriptionUpdateOptions
                {
                    Coupon = coupon.Id
                };
                
                var updatedSubscription = await subscriptionService.UpdateAsync(subscriptionId, subscriptionOptions);
                return updatedSubscription.Discount != null;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Error applying discount to subscription {SubscriptionId}: {Message}", subscriptionId, ex.Message);
                return false;
            }
        }

        public async Task<bool> RenewSubscriptionAsync(string subscriptionId)
        {
            try
            {
                var subscriptionService = new SubscriptionService();
                var subscription = await subscriptionService.GetAsync(subscriptionId);

                if (subscription.Status != "past_due" && 
                    subscription.Status != "canceled" &&
                    subscription.Status != "incomplete_expired")
                {
                    _logger.LogInformation("Subscription {SubscriptionId} does not need renewal, current status: {Status}", 
                        subscriptionId, subscription.Status);
                    return false;
                }

                var invoiceService = new InvoiceService();
                var invoices = await invoiceService.ListAsync(new InvoiceListOptions
                {
                    Subscription = subscriptionId,
                    Limit = 1
                });
                
                var latestInvoice = invoices.FirstOrDefault();
                if (latestInvoice == null || latestInvoice.Status == "paid")
                {
                    _logger.LogInformation("No unpaid invoice found for subscription {SubscriptionId}", subscriptionId);
                    
                    if (subscription.Status == "canceled" || subscription.Status == "incomplete_expired")
                    {
                        var options = new SubscriptionCreateOptions
                        {
                            Customer = subscription.CustomerId,
                            Items = new List<SubscriptionItemOptions>
                            {
                                new SubscriptionItemOptions
                                {
                                    Price = _config.SubscriptionPriceId,
                                    Quantity = 1,
                                }
                            },
                            CollectionMethod = "charge_automatically",
                        };

                        var newSubscription = await subscriptionService.CreateAsync(options);
                        return newSubscription.Status == "active";
                    }
                    
                    return false;
                }

                if (latestInvoice.Status == "open" || latestInvoice.Status == "uncollectible")
                {
                    var payResult = await invoiceService.PayAsync(latestInvoice.Id);
                    return payResult.Status == "paid";
                }

                _logger.LogInformation("Could not retry payment for subscription {SubscriptionId}, invoice status: {Status}", 
                    subscriptionId, latestInvoice.Status);
                return false;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Error renewing subscription {SubscriptionId}: {Message}", subscriptionId, ex.Message);
                return false;
            }
        }
    }
}
