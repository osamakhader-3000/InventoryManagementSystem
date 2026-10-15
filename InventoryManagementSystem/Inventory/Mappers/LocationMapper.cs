using Dto = InventoryManagementSystem.Inventory.DataTransferObject;

namespace InventoryManagementSystem.Inventory.Mappers
{
    public class LocationMapper
    {
        public static Dto.Location ToDto(InventoryManagementSystemDomain.modules.inventory.Entities.Location location)
        {
            return new Dto.Location
            {
                Id = location.Id,
                Name = location.Name
            };
        }

        public static List<Dto.Location> ToDtoList(IEnumerable<InventoryManagementSystemDomain.modules.inventory.Entities.Location> locations)
        {
            return locations.Select(ToDto).ToList();
        }
    }
}
