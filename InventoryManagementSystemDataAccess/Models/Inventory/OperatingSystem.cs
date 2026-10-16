namespace InventoryManagementSystemDataAccess.Models.Inventory
{
    public class OperatingSystem
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<Device> Devices { get; set; } = new List<Device>();
    }
}
