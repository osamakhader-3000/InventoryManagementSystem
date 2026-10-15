using System;
using System.Collections.Generic;
using Domain = InventoryManagementSystemDomain.modules.inventory.Entities;
using Invent = InventoryManagementSystemDataAccess.Models.Inventory;

namespace InventoryManagementSystemDataAccess.Mappers.Inventory
{
    internal static class DepartmentsMapper
    {
        public static Domain.Department? ToDomainModel(this Invent.Department department)
        {
            return Domain.Department.Reconstitute(department.Id, department.Name);
        }
    }
}
