using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDataAccess.Models.Inventory
{
    public class AssignmentAccessory
    {
        public Int64 AssignmentId { get; set; }
        public Assignment Assignment { get; set; } = null!;

        public Guid AccessoryId { get; set; }
        public Accessory Accessory { get; set; } = null!;
    }
}
