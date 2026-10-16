using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDomain.modules.inventory.Entities
{
    public class Device
    {
        public Guid Id { get; private set; }
        public string Model { get; private set; } = string.Empty;

        public string SerialNumber { get; private set; } = string.Empty;

        public string Specs { get; private set; } = string.Empty;

        public DateTime? PurchasedDate { get; private set; }
        public DateTime? RetirementDate { get; private set; }
        public decimal Price { get; private set; }
        public decimal SoldPrice { get; private set; }
        public Enums.OperatingSystem OperatingSystem { get; private set; } = Enums.OperatingSystem.Other;

        public Enums.DeviceStatus Status { get; private set; } = Enums.DeviceStatus.New;

        private Device(Guid id)
        {
            Id = id;
        }

        public static Device Create(
            string model,
            string serialNumber,
            string specs,
            DateTime? purchasedDate,
            DateTime? retirementDate,
            decimal price,
            decimal soldPrice,
            Enums.OperatingSystem operatingSystem,
            Enums.DeviceStatus status)
        {
            return new Device(Guid.NewGuid())
            {
                Model = model,
                SerialNumber = serialNumber,
                Specs = specs,
                PurchasedDate = purchasedDate,
                RetirementDate = retirementDate,
                Price = price,
                SoldPrice = soldPrice,
                OperatingSystem = operatingSystem,
                Status = status
            };
        }

        public static Device Reconstitute(
            Guid id,
            string model,
            string serialNumber,
            string specs,
            DateTime? purchasedDate,
            DateTime? retirementDate,
            decimal price,
            decimal soldPrice,
            Enums.OperatingSystem operatingSystem,
            Enums.DeviceStatus status)
        {            
            return new Device(id)
            {
                Model = model,
                SerialNumber = serialNumber,
                Specs = specs,
                PurchasedDate = purchasedDate,
                RetirementDate = retirementDate,
                Price = price,
                SoldPrice = soldPrice,
                OperatingSystem = operatingSystem,
                Status = status
            };
        }
    }
}
