using Microsoft.Extensions.DependencyInjection;

namespace SecurityApplicationCore.Configuration
{
    public static class LogicServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationLogic(this IServiceCollection services, Microsoft.Extensions.Configuration.ConfigurationManager configuration)
        {            
            services.AddScoped<SecurityApplicationLogic.Interfaces.IUserService, SecurityApplicationLogic.Services.UserService>();
            services.AddScoped<SecurityApplicationLogic.Interfaces.IRoleService, SecurityApplicationLogic.Services.RolesService>();
            services.AddScoped<SecurityApplicationLogic.Interfaces.IUsersTokenService, SecurityApplicationLogic.Services.UsersTokenService>();
            services.AddScoped<SecurityApplicationLogic.Interfaces.ICookiesService, SecurityApplicationLogic.Services.CookiesService>();
            services.Configure<SecurityApplicationLogic.Settings.JwtSettings>(configuration.GetSection("JwtSettings"));
            services.Configure<SecurityApplicationLogic.Settings.EmailVerificaionSettings>(configuration.GetSection("EmailVerificaionSettings"));
            services.Configure<SecurityApplicationLogic.Settings.ActivateAccountSettings>(configuration.GetSection("ActivateAccountSettings"));

            services.AddScoped<InventoryManagementSystemApplication.inventory.Interfaces.IDepartmentService, InventoryManagementSystemApplication.inventory.Services.DepartmentService>();
            services.AddScoped<InventoryManagementSystemApplication.inventory.Interfaces.IAccessoryService, InventoryManagementSystemApplication.inventory.Services.AccessoryService>();
            services.AddScoped<InventoryManagementSystemApplication.inventory.Interfaces.IDeviceService, InventoryManagementSystemApplication.inventory.Services.DeviceService>();
            services.AddScoped<InventoryManagementSystemApplication.inventory.Interfaces.IUserService, InventoryManagementSystemApplication.inventory.Services.UserService>();
            services.AddScoped<InventoryManagementSystemApplication.inventory.Interfaces.IAssignmentService, InventoryManagementSystemApplication.inventory.Services.AssignmentService>();


            return services;
        }
    }
}
