using Application.Features.Enums.Queries;
using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using Application.Features.Users.Search;
using AutoMapper;
using DTO.Enums.User;
using DTO.Medias;
using DTO.Pagination;
using DTO.Response;
using DTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class UserController : ApiControllerBase
{
    private readonly IMapper _mapper;

    public UserController(IMapper mapper)
    {
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new user based on the provided user creation command.
    /// </summary>
    /// <remarks>This method is accessible to anonymous users and processes the request
    /// asynchronously.</remarks>
    /// <param name="request">The user creation command containing the details required to create a new user.</param>
    /// <returns>A <see cref="UserResponse"/> object containing the details of the newly created user.</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<UserResponse> Create([FromBody] UserCreateCommand request)
    {
        return await Mediator.Send(request);
    }

    /// <summary>
    /// Updates the user information based on the provided update request.
    /// </summary>
    /// <remarks>This method processes the update request by mapping it to a command and sending it to the
    /// mediator. Ensure that the request contains valid data as required by the application.</remarks>
    /// <param name="request">The user update request containing the new user details. This parameter cannot be null.</param>
    /// <returns>A <see cref="UserResponse"/> object containing the updated user information.</returns>
    [HttpPut()]
    public async Task<UserResponse> Update([FromBody] UserUpdateRequest request)
    {
        return await Mediator.Send(_mapper.Map<UserUpdateCommand>(request));
    }

    /// <summary>
    /// Updates the user calendly credentials information based on the provided update request
    /// </summary>
    /// <remarks>This method processes the update calendly credentials request by sending it to the mediator.</remarks>
    /// <param name="request">The user calendly credentials update reuqest containing the new/existing calendly credentials. This parameter cannot be null.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Returns <see cref="OkResult"/> if the update is successful.</returns>
    [HttpPut("calendly-credentials")]
    public async Task<IActionResult> UpdateCalendlyCredentials([FromBody] UserUpdateCalendlyCredentialsCommand request)
    {
        await Mediator.Send(request);

        return Ok();
    }

    /// <summary>
    /// Updates the details of an existing customer.
    /// </summary>
    /// <remarks>This operation requires the caller to have the "Administrator" role. The customer ID must
    /// correspond to an existing customer.</remarks>
    /// <param name="customerId">The unique identifier of the customer to update.</param>
    /// <param name="request">An object containing the updated customer details, including first name, last name, and phone number.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Returns <see cref="OkResult"/> if the
    /// update is successful.</returns>
    [Authorize(Roles = "Administrator")]
    [HttpPut("{customerId}")]
    public async Task<IActionResult> Update([FromRoute] int customerId ,[FromBody] UpdateCustomerRequest request)
    {
        await Mediator.Send(new UpdateCustomerCommand(customerId, request.FirstName, request.LastName, request.PhoneNumber));
        return Ok();
    }

    /// <summary>
    /// Retrieves user information based on the specified user ID.
    /// </summary>
    /// <remarks>This method sends a query to retrieve user details and returns the result. If the user with
    /// the specified ID does not exist, the response may indicate this condition.</remarks>
    /// <param name="id">The unique identifier of the user to retrieve. Must be a positive integer.</param>
    /// <returns>A <see cref="UserInfoResponse"/> object containing the user's information.</returns>
    [HttpGet("{id:int}")]
    public async Task<UserInfoResponse> Get(int id)
    {
        var response = await Mediator.Send(new UserGetQuery(id));
        return response;
    }

    /// <summary>
    /// Retrieves information about the currently authenticated user.
    /// </summary>
    /// <remarks>This method returns details about the user associated with the current authentication
    /// context. It is typically used to display user-specific information in client applications.</remarks>
    /// <returns>A <see cref="MeResponse"/> object containing the details of the currently authenticated user.</returns>
    [HttpGet("me")]
    public async Task<MeResponse> GetUserInfo()
    {
        var response = await Mediator.Send(new UserGetCurrentDetailsQuery());
        return response;
    }

    /// <summary>
    /// Changes the user's password based on the provided request.
    /// </summary>
    /// <remarks>This method processes the password change request by delegating it to the application's
    /// mediator. Ensure that the request contains valid and complete information.</remarks>
    /// <param name="request">The command containing the user's current password, new password, and any other required information.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Returns <see cref="OkResult"/> if the
    /// password change is successful.</returns>
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeCommand request)
    {
        await Mediator.Send(request);
        return Ok();
    }

    /// <summary>
    /// Initiates the password reset process for a user based on the provided request.
    /// </summary>
    /// <remarks>This endpoint is accessible anonymously and does not verify the existence of the user  before
    /// responding. It is designed to prevent information disclosure by always returning  a generic success
    /// response.</remarks>
    /// <param name="request">The command containing the necessary information to initiate the password reset process.  This must include the
    /// user's email address or other identifying details.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Typically returns  an HTTP 200 OK
    /// response if the request is successfully processed.</returns>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand request)
    {
        await Mediator.Send(request);
        return Ok();
    }

    /// <summary>
    /// Resets the user's password based on the provided reset password request.
    /// </summary>
    /// <param name="request">The reset password request containing the necessary information to reset the password. This parameter must not
    /// be <c>null</c>.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Returns <see cref="OkResult"/> if the
    /// password reset is successful, or <see cref="BadRequestObjectResult"/> with an error message if the operation
    /// fails.</returns>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand request)
    {
        try
        {
            await Mediator.Send(request);
            return Ok();
        }
        catch (ApplicationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Change the URL of the Calendly Profile for the logged user.
    /// </summary>
    /// <remarks>This operation updates the user's calendly profile URL to the provided one. The change is
    /// processed asynchronously and requires the user to be authenticated.</remarks>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.  Returns <see cref="OkResult"/> if the
    /// activation is successful.</returns>
    [HttpPut("calendly-profile-url")]
    public async Task<IActionResult> ChangeCalendlyProfileUrl([FromQuery] UserChangeCalendlyProfileUrlCommand command)
    {
        await Mediator.Send(command);
        return Ok();
    }

    /// <summary>
    /// Change the URL of the External Calendar for the logged user.
    /// </summary>
    /// <remarks>This operation updates the user's external calendar URL to the provided one. The change is
    /// processed asynchronously and requires the user to be authenticated.</remarks>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.  Returns <see cref="OkResult"/> if the
    /// update is successful.</returns>
    [HttpPut("external-calendar-url")]
    public async Task<IActionResult> ChangeExternalCalendarUrl([FromQuery] UserChangeExternalCalendarUrlCommand command)
    {
        await Mediator.Send(command);
        return Ok();
    }


    /// <summary>
    /// Updates the user's profile picture.
    /// </summary>
    /// <remarks>This method processes the profile picture update asynchronously. The uploaded picture  must
    /// meet any validation requirements defined by the server, such as file size or format.</remarks>
    /// <param name="request">The request containing the new profile picture to be uploaded.  The picture must be provided as a form file.</param>
    /// <returns>A <see cref="MediaItemResponse"/> containing details about the uploaded profile picture.</returns>
    [HttpPut("profile-picture")]
    public async Task<MediaItemResponse> UpdateProfilePicture([FromForm] UserProfilePictureUpdateRequest request)
    {
        var response = await Mediator.Send(new UserProfilePictureUpdateCommand(request.Picture));
        return response;
    }

    /// <summary>
    /// Activates the current user by changing their status to active.
    /// </summary>
    /// <remarks>This operation updates the user's status to <see cref="UserStatus.Active"/>.  The change is
    /// processed asynchronously and requires the user to be authenticated.</remarks>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.  Returns <see cref="OkResult"/> if the
    /// activation is successful.</returns>
    [HttpPut("activate")]
    public async Task<IActionResult> Activate()
    {
        await Mediator.Send(new UserChangeStatusCommand(UserStatus.Active));
        return Ok();
    }

    /// <summary>
    /// Deactivates the current user by changing their status to "Deactivated."
    /// </summary>
    /// <remarks>This operation updates the status of the currently authenticated user to indicate that they
    /// are deactivated.  The user must be authenticated for this action to succeed.</remarks>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Returns <see cref="OkResult"/> if the
    /// operation is successful.</returns>
    [HttpPut("deactivate")]
    public async Task<IActionResult> Deactivate()
    {
        await Mediator.Send(new UserChangeStatusCommand(UserStatus.Deactivated));
        return Ok();
    }

    /// <summary>
    /// Suspends a user account with the specified ID and suspension reason.
    /// </summary>
    /// <remarks>This operation requires the caller to have the "Administrator" role.  The user suspension
    /// reason must be provided in the request body.</remarks>
    /// <param name="id">The unique identifier of the user to suspend.</param>
    /// <param name="request">An object containing the suspension reason.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
    [Authorize(Roles = "Administrator")]
    [HttpPut("{id}/suspend")]
    public async Task<IActionResult> Suspend([FromRoute] int id, [FromBody] UserSuspendRequest request)
    {
        await Mediator.Send(new UserSuspendCommand(id, request.SuspensionReason));
        return Ok();
    }

    /// <summary>
    /// Removes the suspension for the user with the specified identifier.
    /// </summary>
    /// <remarks>This action requires the caller to have the "Administrator" role.  The user ID must
    /// correspond to an existing user in the system.</remarks>
    /// <param name="id">The unique identifier of the user whose suspension is to be removed.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
    [Authorize(Roles = "Administrator")]
    [HttpPut("{id}/remove-suspension")]
    public async Task<IActionResult> Unsuspend([FromRoute] int id)
    {
        await Mediator.Send(new UserUnsuspendCommand(id));
        return Ok();
    }

    /// <summary>
    /// Performs a full search for users based on the specified query parameters.
    /// </summary>
    /// <remarks>This method requires the caller to have the "Administrator" role. Ensure that the provided
    /// query parameters are valid and complete to avoid unexpected results.</remarks>
    /// <param name="reqeust">The search query containing the criteria for filtering users.</param>
    /// <returns>A <see cref="PaginatedList{T}"/> containing the search results, where each item represents a user matching the
    /// specified criteria. The list is paginated based on the query parameters.</returns>
    [Authorize(Roles = "Administrator")]
    [HttpPost("search")]
    public async Task<PaginatedList<UserSearchable>> FullSearch([FromBody] UserFullSearchQuery reqeust)
    {
        return await Mediator.Send(reqeust);
    }

    /// <summary>
    /// Retrieves a collection of user statuses as a list of key-value pairs.
    /// </summary>
    /// <remarks>This method is accessible only to users with the "Administrator" role.  The returned
    /// collection contains the names and values of all possible user statuses.</remarks>
    /// <returns>A read-only collection of <see cref="ListItemBaseResponse"/> objects, where each item represents a user status.</returns>
    [Authorize(Roles = "Administrator")]
    [HttpGet("status")]
    public async Task<IReadOnlyCollection<ListItemBaseResponse>> GetStatuses()
    {
        return await Mediator.Send(new GetEnumValuesQuery(typeof(UserStatus)));
    }

    /// <summary>
    /// Retrieves a list of roles available in the system.
    /// </summary>
    /// <remarks>This method is restricted to users with the "Administrator" role.  It returns a collection of
    /// roles, where each role is represented as a lightweight response object.</remarks>
    /// <returns>A read-only collection of <see cref="ListItemBaseResponse"/> objects, each representing a role.</returns>
    [Authorize(Roles = "Administrator")]
    [HttpGet("role")]
    public async Task<IReadOnlyCollection<ListItemBaseResponse>> GetRoles()
    {
        return await Mediator.Send(new UserGetRolesQuery());
    }

    /// <summary>
    /// Initiates the rebuilding of the search index.
    /// </summary>
    /// <remarks>This operation is restricted to users with the "Administrator" role.  It triggers an
    /// asynchronous process to rebuild the search index, which may impact search functionality during the
    /// rebuild.</remarks>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Returns <see cref="OkResult"/> if the
    /// request is successfully initiated.</returns>
    [Authorize(Roles = "Administrator")]
    [HttpPut("search/rebuild")]
    public async Task<IActionResult> RebuildSearchIndex()
    {
        await Mediator.Send(new UserInitiateSearchIndexRebuildCommand());
        return Ok();
    }


    /// <summary>
    /// Creates a new note.
    /// </summary>
    /// <remarks>This method is accessible to logged users and processes the request
    /// asynchronously.</remarks>
    /// <param name="request">The note creation details required to send an email note.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Returns <see cref="OkResult"/> if the
    /// request is successfully initiated.</returns>
    [HttpPost("note")]
    public async Task<IActionResult> CreateNote([FromBody] UserNoteCreateCommand request)
    {
        await Mediator.Send(request);
        return Ok();
    }

    /// <summary>
    /// Retrieves a list of note types available in the system.
    /// </summary>
    /// <returns>A read-only collection of <see cref="ListItemBaseResponse"/> objects, each representing a note type.</returns>
    [HttpGet("note/type")]
    public async Task<IReadOnlyCollection<ListItemBaseResponse>> GetNoteTypes()
    {
        return await Mediator.Send(new GetEnumValuesQuery(typeof(UserNoteType)));
    }

}
