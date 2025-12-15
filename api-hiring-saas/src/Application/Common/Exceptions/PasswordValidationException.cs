using Microsoft.AspNetCore.Identity;

namespace Application.Common.Exceptions;

public class PasswordValidationException: Exception
{
    List<IdentityError> Errors;
    public PasswordValidationException()
        : base()
    {
    }

    public PasswordValidationException(string message)
        : base(message)
    {
    }

    public PasswordValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
    public PasswordValidationException(IEnumerable<IdentityError> identityErrors)
        : base()
    {

    }
}
