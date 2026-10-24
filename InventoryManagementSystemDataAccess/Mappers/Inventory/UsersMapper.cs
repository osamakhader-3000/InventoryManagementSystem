using System;
using System.Collections.Generic;
using Domain = InventoryManagementSystemDomain.modules.inventory.Entities;
using Invent = InventoryManagementSystemDataAccess.Models.Inventory;

namespace InventoryManagementSystemDataAccess.Mappers.Inventory
{
    internal static class UsersMapper
    {
        public static Domain.User ToDomainModel(this Invent.User user)
        {
            return Domain.User.Reconstitute(
                user.Id,
                user.Name,
                user.Department.ToDomainModel(),
                user.Location.ToDomainModel(),
                (InventoryManagementSystemDomain.modules.inventory.Enums.UserSuspensionState)user.IsSuspended);
        }

        public static Invent.User ToDataAccessModel(this Domain.User user)
        {
            return new Invent.User
            {
                Id = user.Id,
                Name = user.Name,
                DepartmentId = user.Department.Id,
                LocationId = user.Location.Id,
                IsSuspended = (byte)user.IsSuspended
            };
        }
    }
}
