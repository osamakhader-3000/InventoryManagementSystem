using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Exceptions
{
    public class AssignmentAlreadyReturnedException : Exception
    {
        public long AssignmentId { get; }

        public AssignmentAlreadyReturnedException(long assignmentId)
            : base($"Assignment is already returned: {assignmentId}")
        {
            AssignmentId = assignmentId;
        }
    }
}
