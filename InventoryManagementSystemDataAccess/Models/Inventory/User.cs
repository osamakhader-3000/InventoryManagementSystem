using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDataAccess.Models.Inventory
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public Department Department { get; set; } = new Department();
    }
}
