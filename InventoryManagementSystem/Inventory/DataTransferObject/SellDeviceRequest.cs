namespace InventoryManagementSystem.Inventory.DataTransferObject
{
    public class SellDeviceRequest
    {
        public Guid DeviceId { get; set; }

        public decimal Price { get; set; }
    }
}
