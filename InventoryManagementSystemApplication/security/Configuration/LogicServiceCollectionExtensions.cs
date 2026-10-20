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
            services.AddScoped<InventoryManagementSystemApplication.inventory.Interfaces.ILocationService, InventoryManagementSystemApplication.inventory.Services.LocationService>();
            services.AddScoped<InventoryManagementSystemApplication.inventory.Interfaces.IAccessoryService, InventoryManagementSystemApplication.inventory.Services.AccessoryService>();
            services.AddScoped<InventoryManagementSystemApplication.inventory.Interfaces.IDeviceService, InventoryManagementSystemApplication.inventory.Services.DeviceService>();
            services.AddScoped<InventoryManagementSystemApplication.inventory.Interfaces.IUserService, InventoryManagementSystemApplication.inventory.Services.UserService>();
            services.AddScoped<InventoryManagementSystemApplication.inventory.Interfaces.IAssignmentService, InventoryManagementSystemApplication.inventory.Services.AssignmentService>();
            services.AddScoped<InventoryManagementSystemApplication.inventory.Interfaces.ITrialLogService, InventoryManagementSystemApplication.inventory.Services.TrialLogService>();
            services.AddScoped<InventoryManagementSystemApplication.inventory.Interfaces.IOperatingSystemService, InventoryManagementSystemApplication.inventory.Services.OperatingSystemService>();

            services.AddScoped<InventoryManagementSystemApplication.deploymentRegistry.Features.Builds.CreateBuild.CreateBuildUseCase>();
            services.AddScoped<InventoryManagementSystemApplication.deploymentRegistry.Features.Builds.UpdateBuild.UpdateBuildUseCase>();
            services.AddScoped<InventoryManagementSystemApplication.deploymentRegistry.Features.Builds.DeleteBuild.DeleteBuildUseCase>();
            services.AddScoped<InventoryManagementSystemApplication.deploymentRegistry.Features.Builds.GetBuild.GetBuildUseCase>();
            services.AddScoped<InventoryManagementSystemApplication.deploymentRegistry.Features.Builds.ListBuilds.ListBuildsUseCase>();
            services.AddScoped<InventoryManagementSystemApplication.deploymentRegistry.Features.Clients.CreateClient.CreateClientUseCase>();
            services.AddScoped<InventoryManagementSystemApplication.deploymentRegistry.Features.Clients.UpdateClient.UpdateClientUseCase>();
            services.AddScoped<InventoryManagementSystemApplication.deploymentRegistry.Features.Clients.DeleteClient.DeleteClientUseCase>();
            services.AddScoped<InventoryManagementSystemApplication.deploymentRegistry.Features.Clients.GetClient.GetClientUseCase>();
            services.AddScoped<InventoryManagementSystemApplication.deploymentRegistry.Features.Clients.ListClients.ListClientsUseCase>();

            return services;
        }
    }
}
