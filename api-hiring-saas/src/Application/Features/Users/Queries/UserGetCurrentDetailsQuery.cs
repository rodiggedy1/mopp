using Application.Common.Caching;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using AutoMapper;
using Domain.Entities.User;
using DTO.Enums.User;
using DTO.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Queries;

public sealed record UserGetCurrentDetailsQuery : IQuery<MeResponse>;

public sealed class UserGetCurrentDetailsQueryHandler : IQueryHandler<UserGetCurrentDetailsQuery, MeResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILocalizationService _localizationService;

    public UserGetCurrentDetailsQueryHandler(
        ICurrentUserService currentUserService,
        IMapper mapper,
        IApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        ILocalizationService localizationService)
    {
        _currentUserService = currentUserService;
        _mapper = mapper;
        _dbContext = dbContext;
        _userManager = userManager;
        _localizationService = localizationService;
    }

    public async Task<MeResponse> Handle(UserGetCurrentDetailsQuery request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.User
                               .FirstOrDefaultAsync(u => u.Id == _currentUserService.UserId, cancellationToken);

        if (user == null)
            throw new NotFoundException(_localizationService.GetValue("user.notFound.error.message"));

        if (user.Status != UserStatus.Active)
            throw new UnauthorizedAccessException(_localizationService.GetValue("unauthorizedAccess.inactive.error.message"));

        var result = _mapper.Map<MeResponse>(user);
        var roles = await _userManager.GetRolesAsync(user);
        result.Type = roles.First();

        return result;
    }
}
