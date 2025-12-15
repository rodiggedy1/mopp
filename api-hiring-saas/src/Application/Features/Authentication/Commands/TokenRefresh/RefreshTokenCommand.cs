using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Identity;
using Domain.Entities.RefreshTokens;
using Domain.Entities.User;
using Domain.Interfaces;
using DTO.Authentication;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.Features.Authentication.Commands.TokenRefresh;

public sealed record RefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand<LoginResponse>
{
}

public sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IDateTime _dateTime;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IdentityConfig _config;

    public RefreshTokenCommandHandler(
        IJwtTokenService jwtTokenService,
        UserManager<ApplicationUser> userManager,
        IDateTime dateTime,
        IOptions<IdentityConfig> config,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _jwtTokenService = jwtTokenService;
        _userManager = userManager;
        _dateTime = dateTime;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _config = config.Value;
    }
    public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        string username = string.Empty;

        try
        {
            username = _jwtTokenService.GetClaimFromToken(request.AccessToken, "userName", false)!;
        }
        catch (Exception)
        {
            throw new UnauthorizedAccessException("Invalid accesss token");
        }

        var user = await _userManager.FindByNameAsync(username!);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid accesss token");

        var currentRefreshToken = await _dbContext.RefreshToken.FirstOrDefaultAsync(t => t.Value == request.RefreshToken &&
                                                                                  t.UserId == user.Id);

        if (currentRefreshToken == null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        if (currentRefreshToken.ExpiryTime <= _dateTime.Now)
            throw new UnauthorizedAccessException("Expired refresh token");

        var (accessToken, _, newRefreshToken) = await _jwtTokenService.CreateAsync(user);

        var refreshToken = RefreshToken.Create(
            newRefreshToken,
            user.Id,
            _dateTime.Now.Add(_config.RefreshTokenValidity)
            );

        _dbContext.RefreshToken.Remove(currentRefreshToken);

        if (_dbContext.Entry(currentRefreshToken) != null)
        {
            _dbContext.Entry(currentRefreshToken).State = EntityState.Deleted;
        }

        await _dbContext.RefreshToken.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new LoginResponse(accessToken, newRefreshToken);
    }
    public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(v => v.AccessToken)
                .NotEmpty();

            RuleFor(v => v.RefreshToken)
                .MaximumLength(200)
                .NotEmpty();
        }
    }
}
