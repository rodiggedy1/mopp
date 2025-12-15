using Application.Common.Caching;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities.User;
using DTO.User;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.Queries;

public sealed record UserGetQuery(int UserId) : IQuery<UserInfoResponse>;

public sealed class UserGetQueryHandler : IQueryHandler<UserGetQuery, UserInfoResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public UserGetQueryHandler(
        UserManager<ApplicationUser> userManager,
        IApplicationDbContext dbContext,
        IMapper mapper,
        ICacheService cacheService)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _mapper = mapper;
        _cacheService = cacheService;
    }
    public async Task<UserInfoResponse> Handle(UserGetQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.User}-{query.UserId}";

        var user = await _cacheService.GetAsync<ApplicationUser>(cacheKey, cancellationToken);

        if (user == null)
        {
            user = await _userManager.FindByIdAsync(query.UserId.ToString());

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            await _cacheService.AddAsync(cacheKey, user, cancellationToken);
        }

        var result = _mapper.Map<UserInfoResponse>(user!);

        var roles = await _userManager.GetRolesAsync(user);
        result.Type = roles.First();

        return result;
    }
}

public sealed class UserGetQueryValidator : AbstractValidator<UserGetQuery>
{
    public UserGetQueryValidator()
    {
        RuleFor(qry => qry.UserId)
            .NotEmpty();
    }
}
