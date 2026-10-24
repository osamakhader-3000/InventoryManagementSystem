using System;
using System.Collections.Generic;
using System.Text;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using Enums = InventoryManagementSystemDomain.modules.inventory.Enums;

namespace InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces
{
    public interface ITrialLogRepository
    {
        Task Create(TrialLog trialLog);
        Task<List<TrialLog>> ListAll(Guid? userId, DateTime? eventDate, Enums.TransactionType? transaction, string? serialNumber, string? model, string? status, string? location, Guid? agentId);
    }
}
