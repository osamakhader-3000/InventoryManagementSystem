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

        public DateTime AssignmentDate { get; private set; }

        public DateTime? ReturnDate { get; private set; }

        public IEnumerable<Accessory> Accessories => _accessories;

        public string Remarks { get; private set; } = string.Empty;

        public Enums.AssignmentActivity Activity { get; private set; }

        public DateTime? LastUpdateDate { get; private set; }


        private readonly List<Accessory> _accessories = new();

        private Assignment(long? id, Device device, User assignedUser)
        {
            Id = id;
            Device = device;
            AssignedUser = assignedUser;
        }

        public static Assignment Create(
            Device device,
            User assignedUser,
            DateTime assignmentDate,
            DateTime? returnDate,
            IEnumerable<Accessory> accessories,
            string remarks)
        {
            var assignment = new Assignment(null, device, assignedUser)
            {
                AssignmentDate = assignmentDate,
                ReturnDate = returnDate,
                Remarks = remarks,
                Activity = Enums.AssignmentActivity.Active
            };

            assignment._accessories.AddRange(accessories);
            return assignment;
        }

        public static Assignment Reconstitute(
            long id,
            Device device,
            User assignedUser,
            DateTime assignmentDate,
            DateTime? returnDate,
            IEnumerable<Accessory> accessories,
            string remarks,
            Enums.AssignmentActivity activity,
            DateTime? lastUpdateDate = null)
        {
            var assignment = new Assignment(id, device, assignedUser)
            {
                AssignmentDate = assignmentDate,
                ReturnDate = returnDate,
                Remarks = remarks,
                Activity = activity,
                LastUpdateDate = lastUpdateDate
            };

            assignment._accessories.AddRange(accessories);
            return assignment;
        }
    }
}
