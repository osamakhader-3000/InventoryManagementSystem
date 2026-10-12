namespace InventoryManagementSystem.Inventory.DataTransferObject
{
    public class Device
    {
        public Guid Id { get; set; }
        public string Model { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string Specs { get; set; } = string.Empty;
        public DateTime? PurchasedDate { get; set; }
        public DateTime? RetirementDate { get; set; }
        public double Price { get; set; } = 0d;
        public double SoldPrice { get; set; } = 0d;
        public byte OperatingSystem { get; set; } = 0;
    }
}
