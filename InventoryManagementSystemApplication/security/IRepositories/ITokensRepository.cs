using SecurityApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityApplicationLogic.IRepositories
{
    public interface ITokensRepository
    {
        Task CreateRefreshAsync(RefreshToken newRefreshToken);       
        Task<RefreshToken?> FindCurrentByHashAsync(string hash);      
        Task RotateRefreshToken(RefreshToken refTokInUse);
        Task<int> SaveChanges(Action value);
        Task SaveRefreshToken(RefreshToken refreshToken);
        Task RevokeRefreshTokenByUserID(Guid userId);
        Task RevokeRefreshTokenByHash(string hash);

        Task RevokeRefreshTokenByFamilyID(Guid familyId);
    }
}
