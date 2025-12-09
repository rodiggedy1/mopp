using Microsoft.AspNetCore.Identity;

namespace Application.Features.Authentication.Exceptions;

public class InvalidAuthTokenException : Exception
{
    public InvalidAuthTokenException()
        : base()
    {
    }

    public InvalidAuthTokenException(string message)
        : base(message)
    {
    }

    public InvalidAuthTokenException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
    public InvalidAuthTokenException(IEnumerable<IdentityError> identityErrors)
        : base()
    {

    }
}
