namespace InventoryManagementSystem.Inventory.DataTransferObject
{
    public class TrialLog
    {
        public long Id { get; set; }
        public Guid DeviceId { get; set; }
        public string DeviceModel { get; set; } = string.Empty;
        public string DeviceSerialNumber { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public byte TransactionType { get; set; }
        public Guid AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;
    }
}
