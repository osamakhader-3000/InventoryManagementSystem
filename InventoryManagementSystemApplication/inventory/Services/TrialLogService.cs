using InventoryManagementSystemApplication.inventory.Interfaces;
using InventoryManagementSystemApplication.inventory.ReadModels;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Services
{
    public class TrialLogService : ITrialLogService
    {
        private readonly ITrialLogRepository _trialLogRepository;
        public TrialLogService(ITrialLogRepository trialLogRepository)
        {
            _trialLogRepository = trialLogRepository;
        }
        public async Task<List<TrialLog>> List(Guid? userid, DateTime? eventDate, InventoryManagementSystemDomain.modules.inventory.Enums.TransactionType? transaction, string? serialNumber, string? model, string? status, string? location, Guid? agentId, InventoryManagementSystemDomain.modules.inventory.Enums.LogEntityType? entityType)
        {
            return await _trialLogRepository.ListAll(userid, eventDate, transaction, serialNumber, model, status, location, agentId, entityType);
        }

        public async Task<List<TrialLogAgent>> ListAgents()
        {
            return await _trialLogRepository.ListAgents();
        }
    }
}
