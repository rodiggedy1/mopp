using Domain.Entities.JobApplications;
using Domain.Entities.JobApplicationSections;
using Domain.Entities.JobForms;
using Domain.Entities.JobFormSections;
using Domain.Entities.JobsDetails;
using Domain.Entities.Languages;
using Domain.Entities.Notifications;
using Domain.Entities.Payments;
using Domain.Entities.RefreshTokens;
using Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
        where TEntity : class;
    DbSet<ApplicationUser> User { get; }
    DbSet<RefreshToken> RefreshToken { get; }
    DbSet<Notification> Notification { get; }
    DbSet<Language> Language { get; }
    DbSet<JobDetails> JobDetails { get; }
    DbSet<JobForm> JobForm { get; }
    DbSet<JobFormSection> JobFormSection { get; }
    DbSet<JobApplication> JobApplication { get; }
    DbSet<JobApplicationSection> JobApplicationSection { get; }
    DbSet<PaymentTransaction> PaymentTransaction { get; }
}
