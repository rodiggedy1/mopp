using FluentValidation;

namespace Application.Common.Validation;

public abstract class BaseAbstractValidator<T> : AbstractValidator<T>
{
    public virtual async Task ValidateAndThrowCustomAsync(T data, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateAsync(data, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new Common.Exceptions.ValidationException(validationResult.Errors);
        }
    }
}
