using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Exceptions
{
    public class DeviceUnavailableException : Exception
    {
        public Guid DeviceId { get; }

        public DeviceUnavailableException(Guid deviceId)
            : base($"Device is already assigned: {deviceId}")
        {
            DeviceId = deviceId;
        }
    }
}
