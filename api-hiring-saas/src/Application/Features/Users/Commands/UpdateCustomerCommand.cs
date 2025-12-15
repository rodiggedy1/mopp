using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Identity;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using Application.Features.Users.Validators;
using AutoMapper;
using Domain.Entities.User;
using Domain.Entities.Users;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.Commands;

public sealed record UpdateCustomerCommand(
    int CustomerId,
    string FirstName,
    string LastName,
    string? PhoneNumber) : IUserUpdateCustomerData, ICommand;

public sealed class UpdateCustomerCommandHandler : ICommandHandler<UpdateCustomerCommand>
{
    private readonly IApplicationUserManager _applicationUserManager;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PreventAdminModificationValidator _roleValidator;
    private readonly ILocalizationService _localizationService;

    public UpdateCustomerCommandHandler(
        IApplicationUserManager applicationUserManager,
        IMapper mapper,
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        PreventAdminModificationValidator roleValidator,
        ILocalizationService localizationService)
    {
        _applicationUserManager = applicationUserManager;
        _mapper = mapper;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _roleValidator = roleValidator;
        _localizationService = localizationService;
    }

    public async Task Handle(UpdateCustomerCommand command, CancellationToken cancellationToken)
    {
        var user = await _applicationUserManager.GetAsync(command.CustomerId);
        if (user == null)
            throw new NotFoundException(_localizationService.GetValue("user.notFound.error.message"));

        await _roleValidator.ValidateAndThrowCustomAsync(new PreventAdminModificationValidatorData(user), cancellationToken);

        user.UpdateCustomer(command);

        await _userManager.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }
}

public sealed class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(cmd => cmd.CustomerId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(cmd => cmd.FirstName)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(cmd => cmd.LastName)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(cmd => cmd.PhoneNumber)
            .MaximumLength(15);
    }
}
