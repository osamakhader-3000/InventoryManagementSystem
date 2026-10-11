using InventoryManagementSystemDataAccess.Models.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDataAccess.Mappers.Security
{
    internal static class RolesMapper
    {
        public static SecurityApplicationCore.Models.Role ToCoreModel(this Role role)
        {
            return new SecurityApplicationCore.Models.Role(role.Id, role.Name);
        }

        public static Role ToDataAccessModel(this SecurityApplicationCore.Models.Role role)
        {
            return new InventoryManagementSystemDataAccess.Models.Security.Role
            {
                Id = role.Id,
                Name = role.Name
            };
        }

        public static IEnumerable<SecurityApplicationCore.Models.Role> ToCoreModel(this IEnumerable<Role> roles)
        {
            foreach (var role in roles)
            {
                yield return role.ToCoreModel();
            }
        }

        public static IEnumerable<Role> ToDataAccessModel(this IEnumerable<SecurityApplicationCore.Models.Role> roles)
        {
            foreach (var role in roles)
            {
                yield return role.ToDataAccessModel();
            }
        }

    }
}
