using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDomain.modules.inventory.Entities
{
    public class Department
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;

        private Department(Guid id)
        {
            Id = id;
        }

        public static Department Create(string name)
        {
            return new Department(Guid.NewGuid())
            {
                Name = name
            };
        }

        public static Department? Reconstitute(Guid id, string name)
        {
            if (id == Guid.Empty || string.IsNullOrWhiteSpace(name))
                return null;
            return new Department(id)
            {
                Name = name
            };

        }
    }
}
