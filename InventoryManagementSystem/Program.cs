using InventoryManagementSystemDataAccess.Configuration;
using InventoryManagementSystemDataAccess.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SecurityApplicationCore.Configuration;
using SecurityApplicationLogic.Interfaces;
using SecurityApplicationPresentation.AuthorizationPolicies;
using SecurityApplicationPresentation.Bootstrap;
using SecurityApplicationPresentation.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EmailService.Services.EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddApplicationLogic(builder.Configuration);

builder.Services.AddDataAccess(builder.Configuration);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Configuration"];
    options.InstanceName = builder.Configuration["Redis:InstanceName"];
});

builder.Services.AddScoped<IUserDataService, UserDataService>();


builder.Services.AddScoped<SecurityApplicationLogic.Interfaces.IEmailSender, EmailService.Services.EmailSender>();



var authenticationBuilder = builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

})
.AddCookie("External")
.AddOpenIdConnect("Microsoft", options =>
{
    options.SignInScheme = "External";
    options.Authority = "https://login.microsoftonline.com/common/v2.0";

    options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
    options.ResponseType = "code";
    options.CallbackPath = builder.Configuration["Authentication:Microsoft:Callback_Url"];
    options.SaveTokens = true;

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");

    options.GetClaimsFromUserInfoEndpoint = true;

    options.TokenValidationParameters.NameClaimType = "email";

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        IssuerValidator = (issuer, token, parameters) =>
        {
            if (issuer.StartsWith("https://login.microsoftonline.com/", StringComparison.OrdinalIgnoreCase) &&
                issuer.EndsWith("/v2.0", StringComparison.OrdinalIgnoreCase))
            {
                return issuer;
            }

            throw new SecurityTokenInvalidIssuerException($"Invalid issuer: {issuer}");
        }
    };



    options.CorrelationCookie.SameSite = SameSiteMode.None;
    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
    options.CorrelationCookie.Path = "/security";

    options.NonceCookie.SameSite = SameSiteMode.None;
    options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;
    options.NonceCookie.Path = "/security";

    options.Events = new OpenIdConnectEvents
    {
        OnRedirectToIdentityProvider = context =>
        {
            context.ProtocolMessage.RedirectUri = builder.Configuration["Authentication:Microsoft:ForceCallbackUrl"];
            return Task.CompletedTask;
        }
    };

});

SecurityApplicationPresentation.Settings.AuthenticationBuilder.ApplySettings(authenticationBuilder, builder.Configuration);


builder.Services.AddAuthorization(options =>
{

    options.AddPolicy("ExternalOnly", policy =>
    {
        policy.AddAuthenticationSchemes("External");
        policy.RequireAuthenticatedUser();
    });

    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

}).AddAuthorization(options =>
{
    options.AddPolicy("VerifiedEmail", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new EmailVerifiedRequirement());
    });

    options.AddPolicy("ActiveAccount", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new ActiveAccountRequirement());
    });

    options.AddPolicy("AdminLevel", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new AdminLevelRequirement());

    });
});


builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IAuthorizationHandler, EmailVerifiedHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, ActiveAccountHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, AdminLevelRequirementHandler>();

builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationResultHandler>();

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.SingleLine = false;
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
});
var app = builder.Build();

app.UsePathBase("/security");

app.UseHttpsRedirection();

app.UseRouting();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None,
    Secure = CookieSecurePolicy.Always,
    HttpOnly = HttpOnlyPolicy.Always
});

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();


var apiGroup = app.MapGroup("/api");
apiGroup.AddEndpointFilter<SingleFlightSameResponseFilter>();

var securityGroup = apiGroup.MapGroup("/security");

await app.SeedRolesAsync();

UsersApplicationPresentation.APIs.User.Map(app, securityGroup);
UsersApplicationPresentation.APIs.Authentication.Map(app, securityGroup, builder.Configuration);

var inventoryGroup = apiGroup.MapGroup("/inventory");

InventoryManagementSystem.Inventory.APIs.Department.Map(app, inventoryGroup);
InventoryManagementSystem.Inventory.APIs.Location.Map(app, inventoryGroup);
InventoryManagementSystem.Inventory.APIs.Accessory.Map(app, inventoryGroup);
InventoryManagementSystem.Inventory.APIs.Device.Map(app, inventoryGroup);
InventoryManagementSystem.Inventory.APIs.User.Map(app, inventoryGroup);
InventoryManagementSystem.Inventory.APIs.Assignment.Map(app, inventoryGroup);
InventoryManagementSystem.Inventory.APIs.OperatingSystem.Map(app, inventoryGroup);
InventoryManagementSystem.Inventory.APIs.TrialLog.Map(app, inventoryGroup);

var deploymentRegistryGroup = apiGroup.MapGroup("/deploymentregistry");

InventoryManagementSystem.DeploymentRegistry.APIs.Build.Map(app, deploymentRegistryGroup);
InventoryManagementSystem.DeploymentRegistry.APIs.Client.Map(app, deploymentRegistryGroup);

app.Run();
