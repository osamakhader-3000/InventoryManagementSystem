using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDomain.modules.inventory.Entities
{
    public class Location
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;

        private Location(Guid id)
        {
            Id = id;
        }

        public static Location Create(string name)
        {
            return new Location(Guid.NewGuid())
            {
                Name = name
            };
        }

        public static Location Reconstitute(Guid id, string name)
        {
            return new Location(id)
            {
                Name = name
            };

        }
    }
}
