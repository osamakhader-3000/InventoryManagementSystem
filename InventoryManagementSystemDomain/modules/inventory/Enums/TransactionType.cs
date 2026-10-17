using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDomain.modules.inventory.Enums
{
    public enum TransactionType : byte
    {
        Assign = 0,
        Update = 1,
        Relocate = 2,
    }
}
