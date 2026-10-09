
using Microsoft.EntityFrameworkCore;
using SecurityApplicationDataAccess.Models;
using SecurityApplicationDataAccess.Persistence;
using SecurityApplicationLogic.IRepositories;
using CoreModels = SecurityApplicationCore.Models;

namespace SecurityApplicationDataAccess.Persistence.Repositories
{
    public class TokensRepository : SecurityApplicationLogic.IRepositories.ITokensRepository
    {
        private readonly SecurityApplicationDataAccessContext _context;
        public TokensRepository(SecurityApplicationDataAccessContext context)
        {
            _context = context;
        }

      

        public Task CreateRefreshAsync(SecurityApplicationCore.Models.RefreshToken newRefreshToken)
        {
            _context.RefreshTokens.Add(new RefreshToken
            {
                Id = newRefreshToken.Id,
                UserId = newRefreshToken.UserId,
                Hash = newRefreshToken.Hash,
                IssuedAt = newRefreshToken.IssuedAt,
                ExpiresAt = newRefreshToken.ExpiresAt,
                Status = (byte)newRefreshToken.Status,
                FamilyId = newRefreshToken.FamilyId
            });
            return Task.CompletedTask;
        }

     
        public async Task<SecurityApplicationCore.Models.RefreshToken?> FindCurrentByHashAsync(string hash)
        {
            RefreshToken? rt = _context.RefreshTokens.FirstOrDefault(rt => rt.Hash == hash && rt.Status == (int)CoreModels.RefreshTokenStatus.Current);
            return rt == null ?  null : new SecurityApplicationCore.Models.RefreshToken(rt.Id,rt.UserId,string.Empty,rt.Hash,rt.IssuedAt,rt.ExpiresAt,(CoreModels.RefreshTokenStatus)rt.Status,rt.RotatedAt,rt.FamilyId);
        }

        
        public async Task RevokeRefreshTokenByUserID(Guid userId)
        {
            var refreshTokens = _context.RefreshTokens.Where(rt => rt.UserId == userId);
            RevokeRefreshTokens(refreshTokens);
        }

        private static void RotateRefreshTokens(IEnumerable<RefreshToken> refreshTokens)
        {
            foreach (RefreshToken refreshToken in refreshTokens)
            {                
                if (refreshToken != null)
                {
                    refreshToken.Status = (int)CoreModels.RefreshTokenStatus.Rotated;
                    refreshToken.RotatedAt = DateTime.UtcNow;
                }

            }
        }

        private static void RevokeRefreshTokens(IEnumerable<RefreshToken> refreshTokens)
        {
            foreach (RefreshToken refreshToken in refreshTokens)
            {
                if (refreshToken != null)
                {
                    refreshToken.Status = (int)CoreModels.RefreshTokenStatus.Revoked;
                }

            }
        }

      

        public async Task<int> SaveChanges(Action _action)
        {
            _action();
            int numberOfEntriesWritten = await _context.SaveChangesAsync();
            return numberOfEntriesWritten;
        }

        public async Task SaveRefreshToken(SecurityApplicationCore.Models.RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(new RefreshToken
            {
                Id = refreshToken.Id,
                UserId = refreshToken.UserId,
                Hash = refreshToken.Hash,
                IssuedAt = refreshToken.IssuedAt,
                ExpiresAt = refreshToken.ExpiresAt,
                Status = (byte)refreshToken.Status,
                FamilyId = refreshToken.FamilyId
            });                 
        }

             

        public async Task RevokeRefreshTokenByFamilyID(Guid familyId)
        {
            var refreshTokens = _context.RefreshTokens.Where(rt => rt.FamilyId == familyId);
            RevokeRefreshTokens(refreshTokens);
        }

       

        public async Task RotateRefreshToken(CoreModels.RefreshToken refTokInUse)
        {
            IEnumerable<RefreshToken> refreshTokens = _context.RefreshTokens.Where(rt => rt.Id == refTokInUse.Id);
            RotateRefreshTokens(refreshTokens);
        }

        public async Task RevokeRefreshTokenByHash(string hash)
        {
            RefreshToken? rt = _context.RefreshTokens.First(rt => rt.Hash == hash && rt.Status == (int)CoreModels.RefreshTokenStatus.Current);
            RevokeRefreshTokens([rt]);
        }
    }
}
