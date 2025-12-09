using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces;
using Application.Common.MediaStorage;
using AutoMapper;
using Domain.Entities.Medias;
using Domain.Entities.User;
using Domain.Interfaces;
using DTO.Medias;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Application.Features.Medias.Validators;
using Application.Common.Localization;

namespace Application.Features.Users.Commands;

public sealed record UserProfilePictureUpdateCommand(IFormFile Picture) : ICommand<MediaItemResponse>;

public sealed class UserProfilePictureUpdateCommandHandler : ICommandHandler<UserProfilePictureUpdateCommand, MediaItemResponse>
{
    private readonly IApplicationUserManager _applicationUserManager;
    private readonly IMediaStorage _mediaStorage;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILocalizationService _localizationService;

    public UserProfilePictureUpdateCommandHandler(
        IApplicationUserManager applicationUserManager,
        IMediaStorage mediaStorage,
        IMapper mapper,
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILocalizationService localizationService)
    {
        _applicationUserManager = applicationUserManager;
        _mediaStorage = mediaStorage;
        _mapper = mapper;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _localizationService = localizationService;
    }
    public async Task<MediaItemResponse> Handle(UserProfilePictureUpdateCommand command, CancellationToken cancellationToken)
    {
        var user = await _applicationUserManager.GetAsync((int)_currentUserService.UserId!);

        if (user == null)
            throw new NotFoundException(_localizationService.GetValue("user.notFound.error.message"));


        var existedPhoto = user.Media.GetMainOrFirstImage();
        var mediaCreateData = new MediaCreateData(command.Picture, true, 1);
        await user.SetProfilePicture(mediaCreateData, _mediaStorage);

        if (existedPhoto != null)
        {
            await user.Media.Delete(existedPhoto.Id, user.Id, _mediaStorage);
        }

        await _userManager.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<MediaItemResponse>(user.Media.GetMainOrFirstImage());
    }
}

public sealed class UserProfilePictureUpdateCommandValidator : AbstractValidator<UserProfilePictureUpdateCommand>
{
    public UserProfilePictureUpdateCommandValidator(
        IOptions<MediaConfig> mediaConfigOpt,
        FileSizeValidator fileSizeValidator,
        FileExtensionValidator fileExtensionValidator)
    {
        var mediaConfig = mediaConfigOpt.Value;

        RuleFor(cmd => cmd.Picture)
            .NotEmpty();

        RuleFor(cmd => FileSizeValidatorData.FromFile(cmd.Picture, mediaConfig.MaxFileSize))
            .SetValidator(fileSizeValidator)
            .OverridePropertyName(nameof(UserProfilePictureUpdateCommand.Picture));

        RuleFor(cmd => FileExtensionValidatorData.FromFile(cmd.Picture, mediaConfig.AllowedExtensions))
            .SetValidator(fileExtensionValidator)
            .OverridePropertyName(nameof(UserProfilePictureUpdateCommand.Picture));
    }
}
