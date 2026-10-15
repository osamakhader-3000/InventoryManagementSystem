using System;
using System.Collections.Generic;
using Domain = InventoryManagementSystemDomain.modules.inventory.Entities;
using Invent = InventoryManagementSystemDataAccess.Models.Inventory;

namespace InventoryManagementSystemDataAccess.Mappers.Inventory
{
    internal static class LocationsMapper
    {
        public static Domain.Location? ToDomainModel(this Invent.Location location)
        {
            return Domain.Location.Reconstitute(location.Id, location.Name);
        }
    }
}
