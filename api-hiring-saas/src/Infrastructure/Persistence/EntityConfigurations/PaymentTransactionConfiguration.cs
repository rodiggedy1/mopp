using Domain.Entities.Payments;
using Infrastructure.Persistence.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public class PaymentTransactionConfiguration : EntityTypeConfiguration<PaymentTransaction>
{
    protected override void OnConfigure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("PaymentTransactions");

        builder.Property(e => e.StripePaymentIntentId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.StripeCustomerId)
            .HasMaxLength(255);

        builder.Property(e => e.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(e => e.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("usd");

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(e => e.FailureReason)
            .HasMaxLength(500);

        builder.Property(e => e.StripeInvoiceId)
            .HasMaxLength(255);

        builder.HasOne(e => e.User)
            .WithMany(u => u.PaymentTransactions)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.StripePaymentIntentId)
            .IsUnique();

        builder.HasIndex(e => e.UserId);
    }
}