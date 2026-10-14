using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDataAccess.Models.Inventory
{
    public class Accessory
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }

        public DateTime Date { get; set; }

        public List<AssignmentAccessory> AssignmentAccessories { get; set; } = new List<AssignmentAccessory>();
    }
}
