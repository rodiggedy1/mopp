using Application.Common.Interfaces;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;

namespace Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>, IApplicationDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public override EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
        where TEntity : class => base.Entry(entity);
    public DbSet<ApplicationUser> User => Set<ApplicationUser>();
    public DbSet<RefreshToken> RefreshToken => Set<RefreshToken>();
    public DbSet<Notification> Notification => Set<Notification>();
    public DbSet<JobDetails> JobDetails => Set<JobDetails>();
    public DbSet<JobForm> JobForm => Set<JobForm>();
    public DbSet<JobFormSection> JobFormSection => Set<JobFormSection>();
    public DbSet<JobApplication> JobApplication => Set<JobApplication>();
    public DbSet<JobApplicationSection> JobApplicationSection => Set<JobApplicationSection>();
    public DbSet<Language> Language => Set<Language>();
    public DbSet<PaymentTransaction> PaymentTransaction => Set<PaymentTransaction>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}