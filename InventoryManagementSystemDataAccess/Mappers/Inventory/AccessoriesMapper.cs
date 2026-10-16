using System;
using System.Collections.Generic;
using Domain = InventoryManagementSystemDomain.modules.inventory.Entities;
using Invent = InventoryManagementSystemDataAccess.Models.Inventory;

namespace InventoryManagementSystemDataAccess.Mappers.Inventory
{
    internal static class AccessoriesMapper
    {
        public static Domain.Accessory? ToDomainModel(this Invent.Accessory accessory)
        {
            return Domain.Accessory.Reconstitute(accessory.Id, accessory.Name, accessory.Quantity, accessory.AssignedQuantity, accessory.Date);
        }

        public static Invent.Accessory ToDataAccessModel(this Domain.Accessory accessory)
        {
            return new Invent.Accessory
            {
                Id = accessory.Id,
                Name = accessory.Name,
                Quantity = accessory.Quantity,
                AssignedQuantity = accessory.AssignedQuantity,
                Date = accessory.Date
            };
        }
    }
}
