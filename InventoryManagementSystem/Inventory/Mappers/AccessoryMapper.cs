using Dto = InventoryManagementSystem.Inventory.DataTransferObject;

namespace InventoryManagementSystem.Inventory.Mappers
{
    public class AccessoryMapper
    {
        public static Dto.Accessory ToDto(InventoryManagementSystemDomain.modules.inventory.Entities.Accessory accessory)
        {
            return new Dto.Accessory
            {
                Id = accessory.Id,
                Name = accessory.Name,
                Quantity = accessory.Quantity,
                AssignedQuantity = accessory.AssignedQuantity,
                AvailableQuantity = accessory.Quantity - accessory.AssignedQuantity,
                Date = accessory.Date

            };
        }

        public static List<Dto.Accessory> ToDtoList(IEnumerable<InventoryManagementSystemDomain.modules.inventory.Entities.Accessory> accessories)
        {
            return accessories.Select(ToDto).ToList();
        }
    }
}