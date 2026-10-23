using Dto = InventoryManagementSystem.Inventory.DataTransferObject;

namespace InventoryManagementSystem.Inventory.Mappers
{
    public class DeviceMapper
    {
        public static Dto.Device ToDto(InventoryManagementSystemDomain.modules.inventory.Entities.Device device)
        {
            return new Dto.Device
            {
                Id = device.Id,
                Model = device.Model,
                SerialNumber = device.SerialNumber,
                Specs = device.Specs,
                PurchasedDate = device.PurchasedDate,
                RetirementDate = device.RetirementDate,
                Price = device.Price,
                SoldPrice = device.SoldPrice,
                Status = device.Status,
                IsTaken = device.IsTaken == InventoryManagementSystemDomain.modules.inventory.Enums.Availability.Taken,
                IsSold = device.IsSold == InventoryManagementSystemDomain.modules.inventory.Enums.DeviceSoldState.Sold
            };
        }

        public static List<Dto.Device> ToDtoList(IEnumerable<InventoryManagementSystemDomain.modules.inventory.Entities.Device> devices)
        {
            return devices.Select(ToDto).ToList();
        }
    }
}
