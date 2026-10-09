using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;

namespace SecurityApplicationPresentation.Settings
{
    public class AuthenticationSettings
    {
        private readonly IConfigurationSection authentication;
        private readonly IConfigurationSection microsoft;

        public AuthenticationSettings(ConfigurationManager configuration)
        {
            authentication = configuration.GetSection("Authentication");
            microsoft = authentication.GetSection("Microsoft");
        }

        public string MicrosoftClientId
        {
            get { return microsoft["ClientId"] ?? string.Empty; }
        }

        public string MicrosoftClientSecret
        {
            get { return microsoft["ClientSecret"] ?? string.Empty; }
        }

        public string MicrosoftRedirectUri
        {
            get { return microsoft["redirect_uri"] ?? string.Empty; }
        }

        public string MicrosoftLoginUrl
        {
            get { return microsoft["login_url"] ?? string.Empty; }

        }

        public string CallbackUrl
        {
            get { return authentication["Callback_Url"] ?? string.Empty; }
        }

        public string LoginpageUrl
        {
            get { return authentication["Loginpage_Url"] ?? string.Empty; }
        }
    }
}

