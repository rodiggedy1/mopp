using Domain.Entities.Base;
using Domain.Entities.Base.Interfaces;
using Domain.Entities.JobApplicationSectionIcons;
using Domain.Entities.JobApplicationSectionProperties;
using Domain.Entities.JobFormSectionIcons;
using Domain.Entities.JobFormSections;
using Domain.Entities.Medias;
using Infrastructure.Common.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations.Base;

public abstract class EntityTypeConfigurationBase
{
    internal const int NameMaxLength = 200;
    internal const int ExternalIdMaxLength = 64;
    internal const int DescriptionMaxLength = 2048;
}

public abstract class EntityTypeConfiguration<T> : EntityTypeConfigurationBase, IEntityTypeConfiguration<T>
    where T : BaseEntity
{
    public void Configure(EntityTypeBuilder<T> builder)
    {

        if (typeof(IWithMedia).IsAssignableFrom(typeof(T)))
        {
            builder.Property(nameof(IWithMedia.Media)).HasConversion<MediaToDbJsonConverter>();
        }

        if (typeof(IWithJobFormSectionProperty).IsAssignableFrom(typeof(T)))
        {
            builder.Property(nameof(IWithJobFormSectionProperty.JobFormSectionProperties)).HasConversion<JobFormSectionPropertiesToDbJsonConverter>();
        }

        if (typeof(IWithJobApplicationSectionProperty).IsAssignableFrom(typeof(T)))
        {
            builder.Property(nameof(IWithJobApplicationSectionProperty.JobApplicationSectionProperties)).HasConversion<JobApplicationSectionPropertiesToDbJsonConverter>();
        }

        if (typeof(IWithJobFormSectionIcon).IsAssignableFrom(typeof(T)))
        {
            builder.Property(nameof(IWithJobFormSectionIcon.Icon)).HasConversion<JobFormSectionIconToDbJsonConverter>();
        }

        if (typeof(IWithJobApplicationSectionIcon).IsAssignableFrom(typeof(T)))
        {
            builder.Property(nameof(IWithJobApplicationSectionIcon.Icon)).HasConversion<JobApplicationSectionIconToDbJsonConverter>();
        }

        if (typeof(IAuditableEntity).IsAssignableFrom(typeof(T)))
        {
            builder.HasOne(nameof(IAuditableEntity.Creator))
                   .WithMany()
                   .HasForeignKey(nameof(IAuditableEntity.CreatedBy));


            builder.HasOne(nameof(IAuditableEntity.LastModifier))
                   .WithMany()
                   .HasForeignKey(nameof(IAuditableEntity.LastModifiedBy));


            OnConfigure(builder);
        }
    }
    protected abstract void OnConfigure(EntityTypeBuilder<T> builder);
}
