using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDataAccess.Models.Inventory
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public Guid DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        public Guid? LocationId { get; set; }
        public Location? Location { get; set; }

        public List<TrialLog> TrialLogs { get; set; } = new List<TrialLog>();
    }
}
