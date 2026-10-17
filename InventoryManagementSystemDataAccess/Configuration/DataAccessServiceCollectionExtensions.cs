using InventoryManagementSystemDataAccess.Models.Security;
using InventoryManagementSystemDataAccess.Persistence;
using InventoryManagementSystemDataAccess.Persistence.Repositories.Inventory;
using InventoryManagementSystemDataAccess.Persistence.Repositories.Security;
using InventoryManagementSystemDataAccess.Services;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SecurityApplicationLogic.Interfaces;
using SecurityApplicationLogic.IRepositories;
using SecurityApplicationLogic.Settings;

namespace InventoryManagementSystemDataAccess.Configuration
{
    public static class DataAccessServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SecurityApplicationDataAccessContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")).UseSnakeCaseNamingConvention());

            IdentityPasswordSettings passwordSettings = configuration.GetSection("IdentityPasswordSettings").Get<IdentityPasswordSettings>() ?? new IdentityPasswordSettings();

            services.AddIdentityCore<User>(options =>
            {
                options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = passwordSettings.RequiredLength;
                options.Password.RequireDigit = passwordSettings.RequireDigit;
                options.Password.RequireLowercase = passwordSettings.RequireLowercase;
                options.Password.RequireUppercase = passwordSettings.RequireUppercase;
                options.Password.RequireNonAlphanumeric = passwordSettings.RequireNonAlphanumeric;
                options.Password.RequiredUniqueChars = passwordSettings.RequiredUniqueChars;

            }).AddRoles<Role>().AddEntityFrameworkStores<SecurityApplicationDataAccessContext>().AddDefaultTokenProviders();

            services.AddScoped<IRoleSeeder, IdentityRoleSeeder>();
            services.AddScoped<SecurityApplicationLogic.IRepositories.IRolesRepository, RolesRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<ITokensRepository, TokensRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IAccessoryRepository, AccessoryRepository>();
            services.AddScoped<IDeviceRepository, DeviceRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAssignmentRepository, AssignmentRepository>();
            services.AddScoped<IOperatingSystemRepository, OperatingSystemRepository>();
            services.AddScoped<ITrialLogRepository, TrialLogRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<SecurityApplicationLogic.Interfaces.IRoleDataService, RoleDataService>();

            return services;
        }
    }
}
