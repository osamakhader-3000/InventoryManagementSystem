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
        public async Task<List<TrialLog>> List()
        {
            return await _trialLogRepository.ListAll();
        }
    }
}
