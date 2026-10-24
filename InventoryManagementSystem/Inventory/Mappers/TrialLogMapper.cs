using Dto = InventoryManagementSystem.Inventory.DataTransferObject;

namespace InventoryManagementSystem.Inventory.Mappers
{
    public class TrialLogMapper
    {
        public static Dto.TrialLog ToDto(InventoryManagementSystemDomain.modules.inventory.Entities.TrialLog trialLog)
        {
            return new Dto.TrialLog
            {
                Id = trialLog.Id ?? 0,
                DeviceId = trialLog.DeviceId,
                DeviceModel = trialLog.DeviceModel ?? string.Empty,
                DeviceSerialNumber = trialLog.DeviceSerialNumber ?? string.Empty,
                UserId = trialLog.UserId,
                UserName = trialLog.UserName ?? string.Empty,
                EventDate = trialLog.EventDate,
                TransactionType = (byte)trialLog.TransactionType,
                AgentId = trialLog.AgentId,
                AgentName = trialLog.AgentName ?? string.Empty,
                LocationName = trialLog.LocationName ?? string.Empty,
                Status = trialLog.Status ?? string.Empty,
                EntityType = (byte)trialLog.EntityType
            };
        }

        public static List<Dto.TrialLog> ToDtoList(IEnumerable<InventoryManagementSystemDomain.modules.inventory.Entities.TrialLog> trialLogs)
        {
            return trialLogs.Select(ToDto).ToList();
        }

        public static List<Dto.TrialLogAgent> ToDtoList(IEnumerable<InventoryManagementSystemApplication.inventory.ReadModels.TrialLogAgent> agents)
        {
            return agents.Select(x => new Dto.TrialLogAgent
            {
                AgentId = x.AgentId,
                AgentName = x.AgentName ?? string.Empty
            }).ToList();
        }
    }
}
