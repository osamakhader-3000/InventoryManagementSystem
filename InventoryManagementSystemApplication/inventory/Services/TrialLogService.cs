using InventoryManagementSystemApplication.inventory.Interfaces;
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
        public async Task<List<TrialLog>> List(Guid? userid, Guid? departmentId, DateTime? eventDate, InventoryManagementSystemDomain.modules.inventory.Enums.TransactionType? transaction, string? serialNumber, string? model, InventoryManagementSystemDomain.modules.inventory.Enums.DeviceStatus? status)
        {
            return await _trialLogRepository.ListAll(userid, departmentId, eventDate, transaction, serialNumber, model, status);
        }
    }
}
