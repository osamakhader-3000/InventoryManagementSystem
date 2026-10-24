using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Exceptions
{
    public class UserAlreadySuspendedException : Exception
    {
        public Guid UserId { get; }

        public UserAlreadySuspendedException(Guid userId)
            : base($"User is already suspended: {userId}")
        {
            UserId = userId;
        }
    }
}
