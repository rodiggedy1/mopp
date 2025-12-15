using Domain.Entities.Base;
using Domain.Entities.User;
using DTO.Enums.Payments;

namespace Domain.Entities.Payments;

public class PaymentTransaction : BaseAuditableEntity
{
    public int UserId { get; set; }
    public string StripePaymentIntentId { get; set; } = null!;
    public string? StripeCustomerId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "usd";
    public PaymentStatus Status { get; set; }
    public string? FailureReason { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? StripeInvoiceId { get; set; }
    
    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
}