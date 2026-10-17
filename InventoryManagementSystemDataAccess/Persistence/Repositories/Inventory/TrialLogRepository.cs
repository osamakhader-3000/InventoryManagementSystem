using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Invent = InventoryManagementSystemDataAccess.Models.Inventory;

namespace InventoryManagementSystemDataAccess.Persistence.Repositories.Inventory
{
    public class TrialLogRepository : ITrialLogRepository
    {
        private readonly SecurityApplicationDataAccessContext _context;
        public TrialLogRepository(SecurityApplicationDataAccessContext context)
        {
            _context = context;
        }

        public Task Create(TrialLog trialLog)
        {
            var dbTrialLog = new Invent.TrialLog
            {
                DeviceId = trialLog.DeviceId,
                UserId = trialLog.UserId,
                EventDate = trialLog.EventDate,
                TransactionType = (byte)trialLog.TransactionType,
                AgentId = trialLog.AgentId
            };

            _context.InventoryTrialLogs.Add(dbTrialLog);
            return _context.SaveChangesAsync();
        }
    }
}
