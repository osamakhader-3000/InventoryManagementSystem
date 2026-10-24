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
                trialLog.DeviceId.GetValueOrDefault(),
                trialLog.UserId,
                trialLog.EventDate.GetValueOrDefault(),
                (DomainEnums.TransactionType)trialLog.TransactionTypeId.GetValueOrDefault(),
                trialLog.AgentId.GetValueOrDefault(),
                trialLog.Model,
                trialLog.SerialNumber,
                trialLog.UserName,
                trialLog.AgentName,
                trialLog.Location,
                trialLog.Status);
        }
    }
}
