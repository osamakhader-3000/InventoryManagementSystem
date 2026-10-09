using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityApplicationDataAccess.Mappers
{
    internal static class UserRolesMapper
    {

        public static SecurityApplicationCore.Models.Role ToCoreModel(this SecurityApplicationDataAccess.Models.UserRole userRole)
        {
            return new SecurityApplicationCore.Models.Role(userRole.RoleId, null);
        }

        public static SecurityApplicationDataAccess.Models.UserRole ToUserRoleDataAccessModel(this SecurityApplicationCore.Models.Role role)
        {
            return new SecurityApplicationDataAccess.Models.UserRole
            {
                RoleId = role.Id
            };
        }

        public static IEnumerable<SecurityApplicationCore.Models.Role> ToCoreModel(this IEnumerable<SecurityApplicationDataAccess.Models.UserRole> userRoles)
        {
            foreach (var userRole in userRoles)
            {
                yield return userRole.ToCoreModel();
            }
        }

        public static IEnumerable<SecurityApplicationDataAccess.Models.UserRole> ToUserRoleDataAccessModel(this IEnumerable<SecurityApplicationCore.Models.Role> roles)
        {
            foreach (var role in roles)
            {
                yield return role.ToUserRoleDataAccessModel();
            }
        }

    }
}
