using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDataAccess.Models.Inventory
{
    public class TrialLog
    {

        public Int64 Id { get; set; }

        public Guid? DeviceId { get; set; }

        public byte? TransactionTypeId { get; set; }

        public string? TransactionType { get; set; }

        public string? Model { get; set; }

        public string? SerialNumber { get; set; }

        public Guid? UserId { get; set; }
        public string? UserName { get; set; }

        public DateTime? EventDate { get; set; }

        public Guid? AgentId { get; set; }

        public string? AgentName { get; set; }

        public Guid? LocationId { get; set; }

        public string? Location { get; set; }

        public string? Status { get; set; }

        public byte EntityType { get; set; } = 0;
    }
}
