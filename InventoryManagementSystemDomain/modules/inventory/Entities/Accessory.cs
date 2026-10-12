using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDomain.modules.inventory.Entities
{
    public class Accessory
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public int Quantity { get; private set; }
        public DateTime Date { get; private set; }

        private Accessory(Guid id)
        {
            Id = id;
        }

        public static Accessory Create(string name, int quantity, DateTime date)
        {
            return new Accessory(Guid.NewGuid())
            {
                Name = name,
                Quantity = quantity,
                Date = date
            };
        }

        public static Accessory? Reconstitute(Guid id, string name, int quantity, DateTime date)
        {
            if (id == Guid.Empty || string.IsNullOrWhiteSpace(name))
                return null;
            return new Accessory(id)
            {
                Name = name,
                Quantity = quantity,
                Date = date
            };
        }
    }
}
