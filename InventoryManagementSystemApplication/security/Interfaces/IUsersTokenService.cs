
using SecurityApplicationCore.Models;
using System.Security.Claims;

namespace SecurityApplicationLogic.Interfaces
{
    public interface IUsersTokenService
    {
        public AccessToken GenerateAccessToken(User user, IEnumerable<Claim>? extraClaims = null);

        public RefreshToken GenerateNewRefreshToken(Guid userid);
        Task SaveRefreshToken(RefreshToken refreshToken);
        public Task<(AccessToken?, RefreshToken?)> ValidateAndRotateAsync(string refreshTokenPlaintext);

        public Task<(AccessToken?, RefreshToken?)> GenerateUserTokens(User user);

        Task RevokeRefreshTokenByPlainText(string refreshToken);
    }
}
