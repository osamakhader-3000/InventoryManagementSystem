using SecurityApplicationCore.Models;
using SecurityApplicationLogic.Interfaces;
using SecurityApplicationLogic.Services;

namespace SecurityApplicationPresentation.Cookies
{
    public static class CookiesSupplier
    {
        private static void AppendAccessCookie(IResponseCookies cookies, SecurityApplicationCore.Models.AccessToken token)
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


        private static void AppendRefreshCookie(IResponseCookies cookies, RefreshToken token)
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

        public async static Task<SecurityApplicationCore.Models.AccessToken> AppendUserTokens(IResponseCookies cookies, SecurityApplicationCore.Models.User user , IUsersTokenService usersTokenService)
        {
            (SecurityApplicationCore.Models.AccessToken? accessToken, SecurityApplicationCore.Models.RefreshToken? refreshToken) = await usersTokenService.GenerateUserTokens(user);

            if (accessToken == null || refreshToken == null)
            {
                throw new Exception("Token generation failed");
            }

            AppendAccessCookie(cookies, accessToken);

            AppendRefreshCookie(cookies, refreshToken);

            return accessToken;
        }
    }
}
