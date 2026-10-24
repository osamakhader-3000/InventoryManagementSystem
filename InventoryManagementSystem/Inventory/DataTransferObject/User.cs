namespace InventoryManagementSystem.Inventory.DataTransferObject
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Department Department { get; set; } = new Department();

        public Location Location { get; set; } = new Location();

        public byte IsSuspended { get; set; } = 0;
    }
}
