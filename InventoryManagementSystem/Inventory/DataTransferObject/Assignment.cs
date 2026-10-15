namespace InventoryManagementSystem.Inventory.DataTransferObject
{
    public class Assignment
    {
        public long Id { get; set; }
        public DateTime AssignmentDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public Device Device { get; set; } = new Device();

        public bool Headset { get; set; } = false;

        public User? AssignedUser { get; set; }

        public User? OldUser { get; set; }
        public List<Accessory> Accessories { get; set; } = new List<Accessory>();

        public DateTime? LastUpdateDate { get; set; }
    }
}
