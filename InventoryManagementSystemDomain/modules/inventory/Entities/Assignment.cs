using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDomain.modules.inventory.Entities
{
    public class Assignment
    {
        public long? Id { get; private set; }

        public Device Device { get; private set; }

        public User AssignedUser { get; private set; }

        public DateTime? AssignmentDate { get; private set; }

        public DateTime? ReturnDate { get; private set; }

        public IEnumerable<Accessory> Accessories => _accessories;

        public string Remarks { get; private set; } = string.Empty;

        private readonly List<Accessory> _accessories = new();

        
        public static Assignment Create(
            Device device,
            User assignedUser,
            DateTime? assignmentDate,
            DateTime? returnDate,
            IEnumerable<Accessory> accessories,
            string remarks)
        {
            var assignment = new Assignment
            {
                Device = device,
                AssignedUser = assignedUser,
                AssignmentDate = assignmentDate,
                ReturnDate = returnDate,
                Remarks = remarks
            };

            assignment._accessories.AddRange(accessories);
            return assignment;
        }
    }
}
