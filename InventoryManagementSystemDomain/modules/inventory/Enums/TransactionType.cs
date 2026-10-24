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
        Return = 3,
        Suspend = 4,
        Activate = 5,
        Sell = 6,
        Create = 7,
        Delete = 8,
    }
}
