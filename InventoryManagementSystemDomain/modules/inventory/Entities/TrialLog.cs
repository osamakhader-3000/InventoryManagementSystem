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
    }
}
