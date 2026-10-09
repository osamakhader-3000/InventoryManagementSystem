using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityApplicationDataAccess.Mappers
{
    internal static class RolesMapper
    {
        public static SecurityApplicationCore.Models.Role ToCoreModel(this SecurityApplicationDataAccess.Models.Role role)
        {
            return new SecurityApplicationCore.Models.Role(role.Id, role.Name);
        }

        public static SecurityApplicationDataAccess.Models.Role ToDataAccessModel(this SecurityApplicationCore.Models.Role role)
        {
            return new SecurityApplicationDataAccess.Models.Role
            {
                Id = role.Id,
                Name = role.Name
            };
        }

        public static IEnumerable<SecurityApplicationCore.Models.Role> ToCoreModel(this IEnumerable<SecurityApplicationDataAccess.Models.Role> roles)
        {
            foreach (var role in roles)
            {
                yield return role.ToCoreModel();
            }
        }

        public static IEnumerable<SecurityApplicationDataAccess.Models.Role> ToDataAccessModel(this IEnumerable<SecurityApplicationCore.Models.Role> roles)
        {
            foreach (var role in roles)
            {
                yield return role.ToDataAccessModel();
            }
        }

    }
}
