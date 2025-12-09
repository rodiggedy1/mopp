using Application.Features.Authentication.Commands.Login;
using Application.Features.Authentication.Commands.ResendCode;
using Application.Features.Authentication.Commands.TokenRefresh;
using Application.Features.Authentication.Commands.VerifyEmail.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class AuthenticateController : ApiControllerBase
{   

    /// <summary>
    /// Authenticates a user based on the provided login credentials.
    /// </summary>
    /// <remarks>This endpoint is accessible without authentication and is intended for user login.  Ensure
    /// that the <paramref name="request"/> contains valid credentials to avoid authentication errors.</remarks>
    /// <param name="request">The login command containing the user's credentials.</param>
    /// <returns>An <see cref="IActionResult"/> containing the result of the authentication process.  Typically, this includes a
    /// success response with authentication details or an error response if authentication fails.</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromBody] LoginCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    /// <summary>
    /// Verifies the email address associated with the provided request.
    /// </summary>
    /// <remarks>This endpoint is accessible without authentication. Ensure that the <paramref
    /// name="request"/> contains valid data to avoid errors during processing.</remarks>
    /// <param name="request">The command containing the email verification details. This must include the necessary information to complete
    /// the verification process.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Returns <see cref="OkResult"/> if the
    /// verification is successful.</returns>
    [AllowAnonymous]
    [HttpPut("verify")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand request)
    {
        await Mediator.Send(request);
        return Ok();
    }

    /// <summary>
    /// Resends the verification code to the client email addresss.
    /// </summary>
    /// <remarks>This endpoint is accessible anonymously and does not require authentication. Ensure that the
    /// <paramref name="request"/> contains valid data to avoid errors during processing.</remarks>
    /// <param name="request">The command containing the necessary information to resend the verification code.  This must include the details
    /// required to identify the recipient.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.  Returns <see cref="OkResult"/> if the
    /// verification code is successfully resent.</returns>
    [AllowAnonymous]
    [HttpPut("verify/resend-code")]
    public async Task<IActionResult> ResendVerificationCodeForCientApp([FromBody] ResendVeirifcationCommand request)
    {
        await Mediator.Send(request);
        return Ok();
    }

    /// <summary>
    /// Refreshes the authentication token for a user.
    /// </summary>
    /// <remarks>This method is accessible anonymously and is typically used to obtain a new authentication
    /// token  when the current token is expired or nearing expiration. The request must include valid credentials  or a
    /// valid refresh token as required by the implementation.</remarks>
    /// <param name="request">The command containing the necessary information to refresh the token.</param>
    /// <returns>An <see cref="ActionResult"/> containing the result of the token refresh operation.</returns>
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenCommand request)
    {
        return Ok(await Mediator.Send(request));
    }
}
