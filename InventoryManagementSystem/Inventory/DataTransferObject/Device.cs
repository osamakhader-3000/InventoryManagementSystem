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
        public decimal Price { get; set; } = 0m;
        public decimal SoldPrice { get; set; } = 0m;
        public OperatingSystem OperatingSystem { get; set; } = new OperatingSystem();

        public byte Status { get; set; } = 0;
    }
}
