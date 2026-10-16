using Domain = InventoryManagementSystemDomain.modules.inventory.Entities;
using Invent = InventoryManagementSystemDataAccess.Models.Inventory;

namespace InventoryManagementSystemDataAccess.Mappers.Inventory
{
    internal static class OperatingSystemsMapper
    {
        public static Domain.OperatingSystem ToDomainModel(this Invent.OperatingSystem operatingSystem)
        {
            return Domain.OperatingSystem.Reconstitute(operatingSystem.Id, operatingSystem.Name);
        }
    }
}
