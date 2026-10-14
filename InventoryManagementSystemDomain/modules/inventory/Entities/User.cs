using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDomain.modules.inventory.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;

        public Department Department { get; private set; }

        public Location? Location { get; private set; }

        private User(Guid id, Department department, Location? location)
        {
            Id = id;
            Department = department;
            Location = location;
        }

        public static User Create(string name, Department department, Location? location = null)
        {
            return new User(Guid.NewGuid(), department, location)
            {
                Name = name
            };
        }

        public static User? Reconstitute(Guid id, string name, Department? department, Location? location = null)
        {
            if (id == Guid.Empty || string.IsNullOrWhiteSpace(name) || department == null)
                return null;
            return new User(id, department, location)
            {
                Name = name
            };
        }
    }
}
