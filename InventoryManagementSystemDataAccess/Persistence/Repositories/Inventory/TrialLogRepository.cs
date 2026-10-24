using InventoryManagementSystemApplication.inventory.ReadModels;
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

        // Copies the device, user and agent details into the log so it survives their deletion; log a device delete before deleting it
        public async Task Create(TrialLog trialLog)
        {
            // User events have no device, so their log has no model, serial number or status
            var device = trialLog.DeviceId == null ? null : await _context.InventoryDevices
                .Where(x => x.Id == trialLog.DeviceId)
                .Select(x => new { x.Model, x.SerialNumber, x.Status })
                .FirstOrDefaultAsync();

            // Device events have no user, so their log has no user or location; user events still get both
            var user = trialLog.UserId == null ? null : await _context.InventoryUsers
                .Where(x => x.Id == trialLog.UserId)
                .Select(x => new { x.Name, x.LocationId, LocationName = x.Location.Name })
                .FirstOrDefaultAsync();

            var agentName = await _context.Users
                .Where(x => x.Id == trialLog.AgentId)
                .Select(x => x.UserName)
                .FirstOrDefaultAsync();

            var dbTrialLog = new Invent.TrialLog
            {
                DeviceId = trialLog.DeviceId,
                UserId = trialLog.UserId,
                EventDate = trialLog.EventDate,
                TransactionTypeId = (byte)trialLog.TransactionType,
                TransactionType = trialLog.TransactionType.ToString(),
                AgentId = trialLog.AgentId,
                // Accessory events have no device and carry the accessory name as their model
                Model = device?.Model ?? trialLog.DeviceModel,
                SerialNumber = device?.SerialNumber,
                Status = device?.Status,
                UserName = user?.Name,
                LocationId = user?.LocationId,
                Location = user?.LocationName,
                AgentName = agentName
            };

            _context.InventoryTrialLogs.Add(dbTrialLog);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TrialLog>> ListAll(Guid? userId, DateTime? eventDate, Enums.TransactionType? transaction, string? serialNumber, string? model, string? status, string? location, Guid? agentId)
        {
            IQueryable<Invent.TrialLog> query = _context.InventoryTrialLogs;

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (agentId.HasValue)
                query = query.Where(x => x.AgentId == agentId.Value);

            if (!string.IsNullOrEmpty(location))
                query = query.WhereContainsIgnoreCase(x => x.Location ?? string.Empty, location);

            if (eventDate.HasValue)
            {
                var date = DateTime.SpecifyKind(eventDate.Value.Date, DateTimeKind.Utc);
                var nextDay = date.AddDays(1);
                query = query.Where(x => x.EventDate >= date && x.EventDate < nextDay);
            }

            if (transaction.HasValue)            
                query = query.Where(x => x.TransactionTypeId == (byte)transaction.Value);
            

            if (!string.IsNullOrEmpty(serialNumber))
                query = query.Where(x => x.SerialNumber == serialNumber);

            if (!string.IsNullOrEmpty(model))
                query = query.WhereContainsIgnoreCase(x => x.Model?? string.Empty, model);

            if (!string.IsNullOrEmpty(status))
                query = query.WhereContainsIgnoreCase(x => x.Status ?? string.Empty, status);

            var dbTrialLogs = await query
                .OrderByDescending(x => x.EventDate)
                .ToListAsync();
            return dbTrialLogs.ToDomainModelList(x => x.ToDomainModel());
        }

        // One row per agent with the name from their latest log, so a renamed agent appears once
        public async Task<List<TrialLogAgent>> ListAgents()
        {
            var agents = await _context.InventoryTrialLogs
                .Where(x => x.AgentId != null)
                .GroupBy(x => x.AgentId!.Value)
                .Select(g => new
                {
                    AgentId = g.Key,
                    AgentName = g
                        .Where(x => x.AgentName != null)
                        .OrderByDescending(x => x.EventDate)
                        .Select(x => x.AgentName)
                        .FirstOrDefault()
                })
                .OrderBy(x => x.AgentName)
                .ToListAsync();

            return agents.Select(x => new TrialLogAgent(x.AgentId, x.AgentName)).ToList();
        }
    }
}
