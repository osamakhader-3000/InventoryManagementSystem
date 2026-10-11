using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDataAccess.Models.Inventory
{
    public class Department
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
