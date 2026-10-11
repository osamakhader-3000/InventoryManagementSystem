using InventoryManagementSystemDataAccess.Models.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDataAccess.Mappers.Security
{
    internal static class UserRolesMapper
    {

        public static SecurityApplicationCore.Models.Role ToCoreModel(this UserRole userRole)
        {
            return new SecurityApplicationCore.Models.Role(userRole.RoleId, null);
        }

        public static UserRole ToUserRoleDataAccessModel(this SecurityApplicationCore.Models.Role role)
        {
            return new InventoryManagementSystemDataAccess.Models.Security.UserRole
            {
                RoleId = role.Id
            };
        }

        public static IEnumerable<SecurityApplicationCore.Models.Role> ToCoreModel(this IEnumerable<UserRole> userRoles)
        {
            foreach (var userRole in userRoles)
            {
                yield return userRole.ToCoreModel();
            }
        }

        public static IEnumerable<UserRole> ToUserRoleDataAccessModel(this IEnumerable<SecurityApplicationCore.Models.Role> roles)
        {
            foreach (var role in roles)
            {
                yield return role.ToUserRoleDataAccessModel();
            }
        }

    }
}
