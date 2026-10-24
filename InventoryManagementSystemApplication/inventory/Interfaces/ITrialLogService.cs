using InventoryManagementSystemApplication.inventory.ReadModels;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Interfaces
{
    public interface ITrialLogService
    {
        public Task<List<TrialLog>> List(Guid? userid, DateTime? eventDate, InventoryManagementSystemDomain.modules.inventory.Enums.TransactionType? transaction, string? serialNumber, string? model, string? status, string? location, Guid? agentId, InventoryManagementSystemDomain.modules.inventory.Enums.LogEntityType? entityType);
        public Task<List<TrialLogAgent>> ListAgents();
    }
}
