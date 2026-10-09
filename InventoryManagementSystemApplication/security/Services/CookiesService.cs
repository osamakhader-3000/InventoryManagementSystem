using Microsoft.AspNetCore.Http;
using SecurityApplicationCore.Models;
using SecurityApplicationLogic.Interfaces;

namespace SecurityApplicationLogic.Services
{
    public class CookiesService : ICookiesService
    {
        public void AppendAccessCookie(IResponseCookies cookies,SecurityApplicationCore.Models.AccessToken token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = token.ExpiresAt,
                Path = "/",
                IsEssential = true
            };

            cookies.Append("access_token", token.Token, cookieOptions);
        }


        public void AppendRefreshCookie(IResponseCookies cookies, RefreshToken token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = token.ExpiresAt,
                Path = "/",
                IsEssential = true
            };

            cookies.Append("refresh_token", token.Plaintext, cookieOptions);
        }

        public void AppendUserTokens(IResponseCookies cookies, User user)
        {
            throw new NotImplementedException();
        }
    }
}
