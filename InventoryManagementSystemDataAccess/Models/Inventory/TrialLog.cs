using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDataAccess.Models.Inventory
{
    public class TrialLog
    {
        public Int64 Id { get; set; }

        public Guid DeviceId { get; set; }
        public Device Device { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime EventDate { get; set; }

        public byte TransactionType { get; set; }

        public Guid AgentId { get; set; }
        public Security.User Agent { get; set; } = null!;
    }
}
