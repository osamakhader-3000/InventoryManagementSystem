using InventoryManagementSystemDomain.modules.inventory.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Interfaces
{
    public interface ITrialLogService
    {
        public Task<List<TrialLog>> List(Guid? userid, Guid? departmentId, DateTime? eventDate, InventoryManagementSystemDomain.modules.inventory.Enums.TransactionType? transaction, string? serialNumber, string? model, string? status, Guid? locationId);
    }
}
