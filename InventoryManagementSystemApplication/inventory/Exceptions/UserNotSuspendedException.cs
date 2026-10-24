using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Exceptions
{
    public class UserNotSuspendedException : Exception
    {
        public Guid UserId { get; }

        public UserNotSuspendedException(Guid userId)
            : base($"User is not suspended: {userId}")
        {
            UserId = userId;
        }
    }
}
