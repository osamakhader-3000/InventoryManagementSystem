using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDomain.modules.inventory.Entities
{
    public class Accessory
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public int Quantity { get; private set; }
        public DateTime Date { get; private set; }

        private Accessory()
        {
        }

        public static Accessory Create(string name, int quantity, DateTime date)
        {
            return new Accessory
            {
                Name = name,
                Quantity = quantity,
                Date = date
            };
        }
    }
}
