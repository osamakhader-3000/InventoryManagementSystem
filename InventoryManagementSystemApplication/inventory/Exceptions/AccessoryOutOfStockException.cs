using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Exceptions
{
    public class AccessoryOutOfStockException : Exception
    {
        public Guid AccessoryId { get; }

        public AccessoryOutOfStockException(Guid accessoryId)
            : base($"Accessory out of stock: {accessoryId}")
        {
            AccessoryId = accessoryId;
        }
    }
}
