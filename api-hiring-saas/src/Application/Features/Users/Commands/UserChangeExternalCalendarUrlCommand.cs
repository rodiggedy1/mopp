using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Commands;

public sealed record UserChangeExternalCalendarUrlCommand(
    string? ExternalCalendarUrl) : ICommand;

public sealed class UserChangeExternalCalendarUrlCommandHandler : ICommandHandler<UserChangeExternalCalendarUrlCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILocalizationService _localizationService;


    public UserChangeExternalCalendarUrlCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILocalizationService localizationService)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _localizationService = localizationService;
    }
    public async Task Handle(UserChangeExternalCalendarUrlCommand command, CancellationToken cancellationToken)
    {
        var user = await _dbContext.User
            .FirstOrDefaultAsync(u => u.Id == _currentUserService.UserId, cancellationToken);

        if (user == null)
            throw new NotFoundException(_localizationService.GetValue("user.notFound.error.message"));

        user.UpdateExternalCalendarUrl(command.ExternalCalendarUrl);

        _dbContext.User.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}