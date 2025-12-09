using Application.Common.Interfaces;
using Application.Common.Localization;
using Application.Identity;
using Domain.Entities.User;
using Domain.Interfaces;
using Infrastructure.Identity;
using Infrastructure.Search;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Persistence.Configuration;

namespace Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly DatabaseConfig _dbConfig;
    private readonly ApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISender _mediatr;
    private readonly IMediaStorage _mediaStorage;
    private readonly IConfiguration _configuration;

    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IServiceProvider serviceProvider,
        IOptions<DatabaseConfig> dbConfig,
        ApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        ISender mediatr,
        IMediaStorage mediaStorage,
        IConfiguration configuration)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _serviceProvider = serviceProvider;
        _dbConfig = dbConfig.Value;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _mediatr = mediatr;
        _mediaStorage = mediaStorage;
        _configuration = configuration;
    }

    public async Task InitialiseAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            if (_dbConfig.MigrationEnabled && _context.Database.IsNpgsql())
            {
                var adminContext = GetAdminContext();

                await adminContext.Database.MigrateAsync();
            }

            if (_dbConfig.SeedEnabled)
            {
                var defaultApplicationUser = await ApplicationDbContextSeeder.SeedDefaultRolesAndUsersAsync(_userManager, _roleManager);

                var identityContextSetter = services.GetRequiredService<IIdentityContextAccessor>();
                identityContextSetter.IdentityContext = new IdentityContextCustom(new DefaultUserInfo(defaultApplicationUser));

                await _dbContext.Database.ExecuteSqlRawAsync("SELECT set_config('app.is_admin', 'true', true)");

                await ApplicationDbContextSeeder.SeedDefaultLanguages(_mediatr, _dbContext);
                await ApplicationDbContextSeeder.SeedDefaultJobOffer(_mediatr, _dbContext);
                var localizationManageer = services.GetRequiredService<ILocalizationManager>();
                await localizationManageer.InitializeAsync(_dbContext, _logger);

                // Search indexed
                _logger.LogDebug("Starting index rebuild");
                await SearchIndexInitializer.InitializeIndexes(_mediatr, _logger);
                _logger.LogDebug("Finished index rebuild");

                identityContextSetter.IdentityContext = null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    private ApplicationDbContext GetAdminContext()
    {
        var adminConnection = _configuration.GetConnectionString("DefaultAdministrator");

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(adminConnection);

        var adminContext = new ApplicationDbContext(optionsBuilder.Options);

        return adminContext;
    }

    public class DefaultUserInfo : IUserInfo
    {
        private readonly ApplicationUser _user;

        public DefaultUserInfo(ApplicationUser user)
        {
            _user = user;
        }

        public int Id => _user.Id;
        public string UserName => _user.UserName!;
        public bool? IsAdmin => default;
    }
}
