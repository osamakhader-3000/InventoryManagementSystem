using Microsoft.IdentityModel.Tokens;
using SecurityApplicationCore.Models;
using SecurityApplicationLogic.Interfaces;
using SecurityApplicationLogic.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecurityApplicationLogic.Services
{
    public class UsersTokenService : SecurityApplicationLogic.Interfaces.IUsersTokenService
    {
        private readonly ITokensRepository _tokensRepository;
        private readonly Settings.JwtSettings _jwtSettings;
        private readonly IUsersRepository _usersRepository;


        public UsersTokenService(Microsoft.Extensions.Options.IOptions<Settings.JwtSettings> jwtSettings, ITokensRepository tokensRepository, IUsersRepository usersRepository)
        {
            _jwtSettings = jwtSettings.Value;
            _tokensRepository = tokensRepository;
            _usersRepository = usersRepository;
        }


        public AccessToken GenerateAccessToken(User user, IEnumerable<Claim>? extraClaims = null)
        {
            List<Claim> claims = CreateUserClaims(user, extraClaims);


            var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey)),
            SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenMinutes);


            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds
            );

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return new AccessToken(Guid.NewGuid(), jwt, expires);
        }

        private static List<Claim> CreateUserClaims(User user, IEnumerable<Claim>? extraClaims = null)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier,  user.Id.ToString()),
                new Claim(nameof(UserAccountStatus), ((int)user.AccountStatus).ToString()),
                new Claim(nameof(UserEmailVerificationStatus), ((int)user.EmailVerificationStatus).ToString()),
            };

            foreach (var role in user.Roles.Where(r => r != null))
                claims.Add(new Claim(ClaimTypes.Role, role.Name ?? string.Empty));

           
            Guid? userRoleId = user.Roles.FirstOrDefault(r => r != null)?.Id;
            if (userRoleId != null)
                claims.Add(new Claim(nameof(UserRole), userRoleId.Value.ToString()));


            extraClaims?.ToList().ForEach(claim => claims.Add(claim));
            return claims;
        }

        public static string GenerateRefreshTokenPlaintext(int bytes = 64)
        {
            var raw = new byte[bytes];
            RandomNumberGenerator.Fill(raw);
            return Base64UrlEncode(raw);
        }



        private static string Sha256Base64Url(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Base64UrlEncode(hash);
        }

        private static string Base64UrlEncode(ReadOnlySpan<byte> input)
        {
            var s = Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');
            return s;
        }

        public RefreshToken GenerateNewRefreshToken(Guid userid)
        {
            return GenerateRefreshToken(userid, Guid.NewGuid());

        }


        public async Task<(AccessToken?, RefreshToken?)> GenerateUserTokens(User user)
        {
            AccessToken accessToken = GenerateAccessToken(user);
            RefreshToken refreshToken = GenerateNewRefreshToken(user.Id);
            await _tokensRepository.SaveChanges(() =>
            {
                _tokensRepository.RevokeRefreshTokenByUserID(user.Id);
                _tokensRepository.SaveRefreshToken(refreshToken);
            });
            return (accessToken, refreshToken);
        }

        public RefreshToken GenerateFamilyRefreshToken(Guid userid, Guid familyId)
        {
            return GenerateRefreshToken(userid, familyId);
        }

        public RefreshToken GenerateRefreshToken(Guid userid, Guid familyId)
        {
            string plaintext = GenerateRefreshTokenPlaintext();
            string hash = Sha256Base64Url(plaintext);
            RefreshToken refreshToken = new(
                Guid.NewGuid(),
                userid,
                plaintext,
                hash,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays),
                RefreshTokenStatus.Current,
                null,
                familyId
                );
            return refreshToken;
        }

        public async Task SaveRefreshToken(RefreshToken refreshToken)
        {
            await _tokensRepository.SaveRefreshToken(refreshToken);
        }


        public async Task<(AccessToken?, RefreshToken?)> ValidateAndRotateAsync(string refreshTokenPlaintext)
        {
            string hash = Sha256Base64Url(refreshTokenPlaintext);
            RefreshToken? refTokInUse = await _tokensRepository.FindCurrentByHashAsync(hash);

            if (refTokInUse is null || refTokInUse.ExpiresAt <= DateTime.UtcNow)
                throw new Exception("refTokInUse is not of state current");

            if (refTokInUse.Status != RefreshTokenStatus.Current)
            {
                await _tokensRepository.SaveChanges(() =>
                {
                    _tokensRepository.RevokeRefreshTokenByFamilyID(refTokInUse.Id);
                });

                throw new Exception("refTokInUse.Status != RefreshTokenStatus.Current.");
            }

            User? user = await _usersRepository.GetUserByIdAsync(refTokInUse.UserId);

            if (user is null)
            {
                throw new Exception("user is null.");
            }


            RefreshToken newRefreshToken = GenerateFamilyRefreshToken(refTokInUse.UserId, refTokInUse.FamilyId);


            AccessToken newAccessToken = GenerateAccessToken(user);


            await _tokensRepository.SaveChanges(() =>
            {
                _tokensRepository.RotateRefreshToken(refTokInUse);
                _tokensRepository.CreateRefreshAsync(newRefreshToken);
            });

            return (newAccessToken, newRefreshToken);

        }



        public async Task RevokeRefreshTokenByPlainText(string refreshTokenPlaintext)
        {
            string hash = Sha256Base64Url(refreshTokenPlaintext);
            await _tokensRepository.SaveChanges(() =>
             {
                 _tokensRepository.RevokeRefreshTokenByHash(hash);
             });

        }
    }
}
