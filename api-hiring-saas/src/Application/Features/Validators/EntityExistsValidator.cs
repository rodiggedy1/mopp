using Application.Common.Exceptions;
using Application.Common.Interfaces.Repository.Base;
using Application.Common.Localization;
using Application.Common.Validation;
using Domain.Entities.Base;
using FluentValidation;

namespace Application.Features.Validators;

public sealed record EntityExistsValidatorData(int? Id);

public sealed class EntityExistsValidator<TEntity> : BaseAbstractValidator<EntityExistsValidatorData>
    where TEntity : BaseEntity
{
    public EntityExistsValidator(
        IRepository<TEntity> repository,
        ILocalizationService localizationService)
    {
        RuleFor(x => x.Id)
            .MustAsync(async (id, cancellationToken) =>
            {
                var exists = await repository.ExistsAsync(id, cancellationToken);

                if (!exists)
                    throw new NotFoundException(localizationService.GetValue("value.notFound.error.message"));

                return exists;
            });
    }
}
