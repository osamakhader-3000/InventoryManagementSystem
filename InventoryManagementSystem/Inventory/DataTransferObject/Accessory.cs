namespace InventoryManagementSystem.Inventory.DataTransferObject
{
    public class Accessory
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }

        public int AssignedQuantity { get; set; }

        public int AvailableQuantity { get; set; }

        public DateTime Date { get; set; }
    }
}
