using System;
using System.Collections.Generic;
using Domain = InventoryManagementSystemDomain.modules.inventory.Entities;
using DomainEnums = InventoryManagementSystemDomain.modules.inventory.Enums;
using Invent = InventoryManagementSystemDataAccess.Models.Inventory;

namespace InventoryManagementSystemDataAccess.Mappers.Inventory
{
    internal static class AssignmentsMapper
    {
        public static Domain.Assignment ToDomainModel(this Invent.Assignment assignment)
        {
            return Domain.Assignment.Reconstitute(
                assignment.Id,
                assignment.Device.ToDomainModel(),
                assignment.AssignedUser.ToDomainModel(),
                assignment.AssignmentDate,
                assignment.ReturnDate,
                assignment.AssignmentAccessories.ToDomainModelList(a => a.Accessory.ToDomainModel()),
                assignment.Remarks,
                assignment.IsReturned == 0 ? DomainEnums.AssignmentActivity.Active : DomainEnums.AssignmentActivity.Returned);
        }
    }
}
