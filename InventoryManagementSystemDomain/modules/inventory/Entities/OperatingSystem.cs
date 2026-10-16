using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDomain.modules.inventory.Entities
{
    public class OperatingSystem
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        private OperatingSystem(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public static OperatingSystem Create(string name)
        {
            return new OperatingSystem(Guid.NewGuid(), name);
        }

        public static OperatingSystem Reconstitute(Guid id, string name)
        {
            return new OperatingSystem(id, name);
        }
    }
}
