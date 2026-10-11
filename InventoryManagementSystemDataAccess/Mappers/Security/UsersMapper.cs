using SecurityApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystemDataAccess.Mappers.Security
{
    internal static class UsersMapper
    {
        public static SecurityApplicationCore.Models.User ToCoreModel(this Models.Security.User user)
        {
            SecurityApplicationCore.Models.UserAccountStatus accountStatus =user.AccountStatus != null ? (SecurityApplicationCore.Models.UserAccountStatus)user.AccountStatus : SecurityApplicationCore.Models.UserAccountStatus.Temporary;
            SecurityApplicationCore.Models.UserEmailVerificationStatus emailVerificationStatus = user.EmailVerificationStatus != null ? (SecurityApplicationCore.Models.UserEmailVerificationStatus)user.EmailVerificationStatus : SecurityApplicationCore.Models.UserEmailVerificationStatus.Unverified;
            IEnumerable<SecurityApplicationCore.Models.Role> roles = user.UserRoles != null ? user.UserRoles.Select(ur => ur.ToCoreModel()) : new List<SecurityApplicationCore.Models.Role>();
            SecurityApplicationCore.Models.User coreUser = SecurityApplicationCore.Models.User.Reconstitute(user.Id, user.Email ?? string.Empty, user.UserName,  user.PasswordHash, user.LocalAccount??false, emailVerificationStatus, accountStatus, roles);
            return coreUser;
        }
        
        public static Models.Security.User ToDataAccessModel(this SecurityApplicationCore.Models.User user)
        {
            return new InventoryManagementSystemDataAccess.Models.Security.User
            {
                Id = user.Id,
                UserName = user.Username,
                Email = user.Email,
                PasswordHash = user.Password,
                EmailVerificationStatus = (byte?)user.EmailVerificationStatus,
                AccountStatus = (byte?)user.AccountStatus,
                LocalAccount = user.LocalAccount
            };
        }
    }
}
