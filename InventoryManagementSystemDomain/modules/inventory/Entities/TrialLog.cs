using InventoryManagementSystemDomain.modules.inventory.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDomain.modules.inventory.Entities
{
    public class TrialLog
    {
        public long? Id { get; private set; }

        public Guid DeviceId { get; private set; }

        public Guid UserId { get; private set; }

        public DateTime EventDate { get; private set; }

        public TransactionType TransactionType { get; private set; }

        public Guid AgentId { get; private set; }

        public string? DeviceModel { get; private set; }

        public string? DeviceSerialNumber { get; private set; }

        public string? UserName { get; private set; }

        public string? AgentName { get; private set; }

        public string? LocationName { get; private set; }

        public string? Status { get; private set; }

        private TrialLog(long? id)
        {
            Id = id;
        }

        public static TrialLog Create(Guid deviceId, Guid userId, DateTime eventDate, TransactionType transactionType, Guid agentId)
        {
            return new TrialLog(null)
            {
                DeviceId = deviceId,
                UserId = userId,
                EventDate = eventDate,
                TransactionType = transactionType,
                AgentId = agentId
            };
        }

        public static TrialLog Reconstitute(
            long id,
            Guid deviceId,
            Guid userId,
            DateTime eventDate,
            TransactionType transactionType,
            Guid agentId,
            string? deviceModel,
            string? deviceSerialNumber,
            string? userName,
            string? agentName,
            string? locationName,
            string? status)
        {
            return new TrialLog(id)
            {
                DeviceId = deviceId,
                UserId = userId,
                EventDate = eventDate,
                TransactionType = transactionType,
                AgentId = agentId,
                DeviceModel = deviceModel,
                DeviceSerialNumber = deviceSerialNumber,
                UserName = userName,
                AgentName = agentName,
                LocationName = locationName,
                Status = status
            };
        }
    }
}
