using Domain.Entities.Medias;
using Domain.Entities.Notifications;
using Domain.Entities.Payments;
using Domain.Entities.RefreshTokens;
using Domain.Entities.Users;
using Domain.Entities.Users.Providers;
using Domain.Events;
using Domain.Events.Users;
using Domain.Interfaces;
using DTO.Enums.Media;
using DTO.Enums.User;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.User
{
    public class ApplicationUser : IdentityUser<int>, IHasDomainEvents, IWithMedia
    {
        #region Domain events

        private readonly List<BaseEvent> _domainEvents = new();
        [NotMapped]
        public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public void AddDomainEvent(BaseEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(BaseEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        #endregion

        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;
        public DateTime? LastLoginDate { get; set; }
        public DateTime Created { get; set; }
        public UserStatus Status { get; set; }
        public Guid Uid { get; private set; }
        public Media Media { get; set; } = null!;
        public string? PasswordResetToken { get; private set; }
        public string? ActualPassword { get; private set; }
        public string? EmailVerificationToken { get; private set; }
        public string? SuspensionReason { get; private set; }
        public string? CalendlyProfileUrl { get; private set; }
        public string? CalendlyClientId { get; private set; }
        public string? CalendlyClientSecret { get; private set; }
        public string? CalendlyAccessToken { get; private set; }
        public string? CalendlyCode { get; private set; }
        public string? CalendlyRedirectUri { get; private set; }
        public string? CalendlyRefreshToken { get; private set; }
        public string? CalendlyTokenExpiresAt { get; private set; }

        public string? ExternalCalendarUrl { get; private set; }

        // Subscription related properties
        public string? StripeCustomerId { get; private set; }
        public string? StripeSubscriptionId { get; private set; }
        public string? SubscriptionStatus { get; private set; } // "trialing", "active", "canceled", etc.
        public DateTime? TrialStartedAt { get; private set; }
        public DateTime? TrialEndsAt { get; private set; }
        public DateTime? SubscriptionStartedAt { get; private set; }
        public DateTime? SubscriptionEndsAt { get; private set; }
        public bool HasUsedTrialExtension { get; private set; }
        public bool HasUsedDiscount { get; private set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

        public static ApplicationUser Create(
            IUserInsertData data,
            IDateTime dateTimeProvider,
            bool autoVerify = false)
        {
            var user = new ApplicationUser
            {
                FirstName = data.FirstName,
                LastName = data.LastName,
                Email = data.Email.ToLower(),
                UserName = data.Email.ToLower(),
                Created = dateTimeProvider.Now,
                Uid = Guid.NewGuid(),
                Media = new Media(MediaEntityType.User),
                PhoneNumber = data.PhoneNumber,
                Status = UserStatus.Active,
                EmailConfirmed = true
            };

            user.AddDomainEvent(new UserCreatedEvent(user));

            if (autoVerify)
            {
                user.SetEmailCofirmed();
            }

            return user;
        }

        public void SetActualPassword(string password)
        {
            ActualPassword = password;
            AddDomainEvent(new UserUpdatedEvent(this));
        }

        public void Update(IUserUpdateData data)
        {
            FirstName = data.FirstName;
            LastName = data.LastName;
            Email = data.Email.ToLower();
            UserName = data.Email.ToLower();
            PhoneNumber = data.PhoneNumber;

            AddDomainEvent(new UserUpdatedEvent(this));
        }

        public void UpdateCustomer(IUserUpdateCustomerData data)
        {
            FirstName = data.FirstName;
            LastName = data.LastName;
            PhoneNumber = data.PhoneNumber;

            AddDomainEvent(new UserUpdatedEvent(this));
        }

        public void SetEmailConfirmed()
        {
            SetEmailCofirmed();
            AddDomainEvent(new UserEmailConfirmedEvent(this));
            AddDomainEvent(new UserUpdatedEvent(this));
        }

        public void UpdateSubscriptionStatus(
            string subscriptionStatus,
            IDateTime dateTimeProvider,
            string? stripeCustomerId = null,
            string? stripeSubscriptionId = null)
        {
            SubscriptionStatus = subscriptionStatus;

            if (!string.IsNullOrEmpty(stripeCustomerId))
                StripeCustomerId = stripeCustomerId;

            if (!string.IsNullOrEmpty(stripeSubscriptionId))
                StripeSubscriptionId = stripeSubscriptionId;

            if (subscriptionStatus == "active")
            {
                SubscriptionEndsAt = null;
            }
            else if (subscriptionStatus == "canceled")
            {
                if (SubscriptionEndsAt == null)
                {
                    SubscriptionEndsAt = dateTimeProvider.Now;
                }
            }

            AddDomainEvent(new UserUpdatedEvent(this));
        }

        public void StartTrial(DateTime trialStartDate, DateTime trialEndDate, string stripeCustomerId, string stripeSubscriptionId)
        {
            SubscriptionStatus = "trialing";
            TrialStartedAt = trialStartDate;
            TrialEndsAt = trialEndDate;
            StripeCustomerId = stripeCustomerId;
            StripeSubscriptionId = stripeSubscriptionId;
            SubscriptionEndsAt = trialEndDate;

            AddDomainEvent(new UserUpdatedEvent(this));
        }

        public void StartSubscription(DateTime subscriptionStartDate, DateTime? subscriptionEndDate = null)
        {
            SubscriptionStatus = "active";
            SubscriptionStartedAt = subscriptionStartDate;
            SubscriptionEndsAt = subscriptionEndDate;
            AddDomainEvent(new UserUpdatedEvent(this));
        }

        public void EndSubscription(DateTime endDate)
        {
            SubscriptionStatus = "canceled";
            SubscriptionEndsAt = endDate;

            AddDomainEvent(new UserUpdatedEvent(this));
        }

        public void RenewSubscription(DateTime renewalDate)
        {
            SubscriptionStatus = "active";
            SubscriptionStartedAt = renewalDate;
            SubscriptionEndsAt = null;

            AddDomainEvent(new UserUpdatedEvent(this));
        }

        public void UpdateSubscriptionEndDate(DateTime? endDate)
        {
            SubscriptionEndsAt = endDate;
            AddDomainEvent(new UserUpdatedEvent(this));
        }

        public void MarkTrialExtensionUsed()
        {
            HasUsedTrialExtension = true;
            AddDomainEvent(new UserUpdatedEvent(this));
        }

        public void MarkDiscountUsed()
        {
            HasUsedDiscount = true;
            AddDomainEvent(new UserUpdatedEvent(this));
        }

        public bool IsTrialActive()
        {
            return SubscriptionStatus == "trialing" && TrialEndsAt.HasValue && DateTime.UtcNow <= TrialEndsAt.Value;
        }

        public bool IsSubscriptionActive()
        {
            return SubscriptionStatus == "active" || IsTrialActive();
        }

        public void Suspend(string reason)
        {
            Status = UserStatus.Suspended;
            SuspensionReason = reason;
            AddDomainEvent(new UserSuspendedEvent(this));
            AddDomainEvent(new UserUpdatedEvent(this));
        }
        public void RemoveSuspension()
        {
            Status = UserStatus.Active;
            SuspensionReason = null;
            AddDomainEvent(new UserSuspensionRemovedEvent(this));
            AddDomainEvent(new UserUpdatedEvent(this));
        }
        public void Activate()
        {
            Status = UserStatus.Active;
            AddDomainEvent(new UserActivatedEvent(this));
            AddDomainEvent(new UserUpdatedEvent(this));
        }
        public void Deactivate()
        {
            Status = UserStatus.Deactivated;
            AddDomainEvent(new UserDeactivatedEvent(this));
            AddDomainEvent(new UserUpdatedEvent(this));
        }
        public void Delete()
        {
            Status = UserStatus.Deleted;
            AddDomainEvent(new UserDeletedEvent(this));
            AddDomainEvent(new UserUpdatedEvent(this));
        }
        public void UpdatePassword(string oldPassword, string ipAddress)
        {
            AddDomainEvent(new PasswordChangedEvent(this, oldPassword, ipAddress));
            PasswordResetToken = null;
        }

        public async Task SetProfilePicture(IMediaUpsertData data, IMediaStorage mediaStorage)
        {
            await RemoveProfilePicture(mediaStorage, false);
            await Media.Save(data, Id, mediaStorage);
            AddDomainEvent(new UserUpdatedEvent(this));
        }

        public async Task RemoveProfilePicture(IMediaStorage mediaStorage, bool raiseEvent = true)
        {
            var existedPhoto = Media.GetMainOrFirstImage();

            if (existedPhoto != null)
            {
                await Media.Delete(existedPhoto.Id, Id, mediaStorage);

                if (raiseEvent)
                {
                    AddDomainEvent(new UserUpdatedEvent(this));
                }
            }
        }

        private void SetEmailCofirmed()
        {
            EmailConfirmed = true;
            Status = UserStatus.Active;
        }

        public async Task GenereatePasswordResetCode(IAuthCodeProvider codeProvider)
        {
            PasswordResetToken = await codeProvider.GenereatePasswordResetCode(this);
        }

        public void SetEmailVerificationToken(string token)
        {
            EmailVerificationToken = token;
        }

        public bool HasUsedTrial()
        {
            return TrialStartedAt.HasValue || HasUsedTrialExtension;
        }

        public void UpdateExternalCalendarUrl(string? url)
        {
            ExternalCalendarUrl = url;
            AddDomainEvent(new UserUpdatedEvent(this));
        }

        public void UpdateCalendlyProfileUrl(string? url)
        {
            CalendlyProfileUrl = url;
            AddDomainEvent(new UserUpdatedEvent(this));
        }

        public void UpdateCalendlyDetails(IUserCalendlyCredentialsData data)
        {
            CalendlyClientId = data.CalendlyClientId;
            CalendlyClientSecret = data.CalendlyClientSecret;
            CalendlyAccessToken = data.CalendlyAccessToken;
            CalendlyCode = data.CalendlyCode;
            CalendlyRedirectUri = data.CalendlyRedirectUri;
            CalendlyRefreshToken = data.CalendlyRefreshToken;
            CalendlyTokenExpiresAt = data.CalendlyTokenExpiresAt;

            AddDomainEvent(new UserUpdatedEvent(this));
        }
    }
}
