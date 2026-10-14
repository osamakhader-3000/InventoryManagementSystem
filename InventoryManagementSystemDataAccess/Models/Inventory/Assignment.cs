using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDataAccess.Models.Inventory
{
    public class Assignment
    {
     
        public Int64 Id { get; set; }

        public Guid DeviceId { get; set; }
        public Device Device { get; set; } = null!;

        public Guid AssignedUserId { get; set; }
        public User AssignedUser { get; set; } = null!;

        public DateTime AssignmentDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public List<AssignmentAccessory> AssignmentAccessories { get; set; } = new List<AssignmentAccessory>();

        public string? Remarks { get; set; }

    }
}
