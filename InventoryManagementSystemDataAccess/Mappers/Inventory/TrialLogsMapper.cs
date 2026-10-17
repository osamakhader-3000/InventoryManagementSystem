using System;
using System.Collections.Generic;
using Domain = InventoryManagementSystemDomain.modules.inventory.Entities;
using DomainEnums = InventoryManagementSystemDomain.modules.inventory.Enums;
using Invent = InventoryManagementSystemDataAccess.Models.Inventory;

namespace InventoryManagementSystemDataAccess.Mappers.Inventory
{
    internal static class TrialLogsMapper
    {
        public static Domain.TrialLog ToDomainModel(this Invent.TrialLog trialLog)
        {
            return Domain.TrialLog.Reconstitute(
                trialLog.Id,
                trialLog.DeviceId,
                trialLog.UserId,
                trialLog.EventDate,
                (DomainEnums.TransactionType)trialLog.TransactionType,
                trialLog.AgentId,
                trialLog.Device?.Model,
                trialLog.Device?.SerialNumber,
                trialLog.User?.Name,
                trialLog.Agent?.UserName,
                trialLog.User?.Location?.Name,
                (DomainEnums.DeviceStatus?)trialLog.Device?.Status);
        }
    }
}
