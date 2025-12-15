using DTO.Payments;

namespace Application.Common.Payments;

public interface IPaymentProcessor
{
    Task<string> CreateCheckoutSessionAsync(int userId, string userEmail, bool includeTrial = true);
    Task<string> CreateBillingPortalSessionAsync(string stripeCustomerId, int userId);
    Task<CheckoutSessionResult> VerifyCheckoutSessionAsync(string sessionId);
    Task<SubscriptionStatusResult> GetSubscriptionStatusAsync(string subscriptionId);
    Task<bool> CancelSubscriptionAsync(string subscriptionId);
    Task<bool> ExtendTrialAsync(string subscriptionId, int days);
    Task<bool> PauseSubscriptionAsync(string subscriptionId, int days);
    Task<bool> ApplyDiscountAsync(string subscriptionId, decimal percent, int billingCycles);
    Task<bool> RenewSubscriptionAsync(string subscriptionId);

}
