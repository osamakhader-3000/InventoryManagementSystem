using InventoryManagementSystemDataAccess.Mappers.Inventory;
using InventoryManagementSystemDataAccess.Persistence.Extensions;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Invent = InventoryManagementSystemDataAccess.Models.Inventory;
using Enums = InventoryManagementSystemDomain.modules.inventory.Enums;

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

        public async Task<List<TrialLog>> ListAll(Guid? userId, Guid? departmentId, DateTime? eventDate, Enums.TransactionType? transaction, string? serialNumber, string? model, string? status, Guid? locationId)
        {
            IQueryable<Invent.TrialLog> query = _context.InventoryTrialLogs
                .Include(x => x.Device)
                .Include(x => x.User).ThenInclude(x => x.Location)
                .Include(x => x.Agent);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (departmentId.HasValue)
                query = query.Where(x => x.User.DepartmentId == departmentId.Value);

            if (locationId.HasValue)
                query = query.Where(x => x.User.LocationId == locationId.Value);

            if (eventDate.HasValue)
            {
                var date = DateTime.SpecifyKind(eventDate.Value.Date, DateTimeKind.Utc);
                var nextDay = date.AddDays(1);
                query = query.Where(x => x.EventDate >= date && x.EventDate < nextDay);
            }

            if (transaction.HasValue)
            {
                byte transactionType = (byte)transaction.Value;
                query = query.Where(x => x.TransactionType == transactionType);
            }

            if (!string.IsNullOrEmpty(serialNumber))
                query = query.Where(x => x.Device.SerialNumber == serialNumber);

            if (!string.IsNullOrEmpty(model))
                query = query.WhereContainsIgnoreCase(x => x.Device.Model, model);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(x => x.Device.Status == status);

            var dbTrialLogs = await query
                .OrderByDescending(x => x.EventDate)
                .ToListAsync();
            return dbTrialLogs.ToDomainModelList(x => x.ToDomainModel());
        }
    }
}
