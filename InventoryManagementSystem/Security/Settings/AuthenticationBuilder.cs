using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace SecurityApplicationPresentation.Settings
{
    public static class AuthenticationBuilder
    {

        public const string SectionName = "JwtSettings";

        internal static void ApplySettings(Microsoft.AspNetCore.Authentication.AuthenticationBuilder authenticationBuilder, ConfigurationManager configuration)
        {
            IConfigurationSection jwtSettingsSection = configuration.GetSection(SectionName) ?? throw new Exception("JwtSettings section is missing from configuration.");
            SecurityApplicationLogic.Settings.JwtSettings jwtSettings = jwtSettingsSection.Get<SecurityApplicationLogic.Settings.JwtSettings>() 
                                                                        ?? throw new Exception("JwtSettings section is missing or has invalid configuration.");

            if (jwtSettings.SigningKey != null)
            {
                string jwtKey = jwtSettings.SigningKey ?? string.Empty;
                SymmetricSecurityKey SigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey));

                authenticationBuilder.AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = SigningKey,
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateLifetime = true,
                        NameClaimType = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName

                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            if (string.IsNullOrEmpty(ctx.Token) && ctx.Request.Cookies.TryGetValue("access_token", out var t))
                                ctx.Token = t;                           
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = ctx =>
                        {
                            Console.WriteLine($"Auth failed: {ctx.Exception.Message}");
                            return Task.CompletedTask;
                        }
                    };
                                        
                });

            }
        }


    }
}
