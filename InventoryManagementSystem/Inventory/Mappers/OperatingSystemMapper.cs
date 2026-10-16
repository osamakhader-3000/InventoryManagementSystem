using Dto = InventoryManagementSystem.Inventory.DataTransferObject;
using DomainEntities = InventoryManagementSystemDomain.modules.inventory.Entities;

namespace InventoryManagementSystem.Inventory.Mappers
{
    public class OperatingSystemMapper
    {
        public static Dto.OperatingSystem ToDto(DomainEntities.OperatingSystem operatingSystem)
        {
            return new Dto.OperatingSystem
            {
                Id = operatingSystem.Id,
                Name = operatingSystem.Name
            };
        }

        public static List<Dto.OperatingSystem> ToDtoList(IEnumerable<DomainEntities.OperatingSystem> operatingSystems)
        {
            return operatingSystems.Select(ToDto).ToList();
        }
    }
}
