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
        public double Price { get; private set; }
        public double SoldPrice { get; private set; }
        public Enums.OperatingSystem OperatingSystem { get; private set; } = Enums.OperatingSystem.Other;

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
            double price,
            double soldPrice,
            Enums.OperatingSystem operatingSystem)
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
                OperatingSystem = operatingSystem
            };
        }

        public static Device? Reconstitute(
            Guid id,
            string model,
            string serialNumber,
            string specs,
            DateTime? purchasedDate,
            DateTime? retirementDate,
            double price,
            double soldPrice,
            Enums.OperatingSystem operatingSystem)
        {
            if (id == Guid.Empty || string.IsNullOrWhiteSpace(model))
                return null;
            return new Device(id)
            {
                Model = model,
                SerialNumber = serialNumber,
                Specs = specs,
                PurchasedDate = purchasedDate,
                RetirementDate = retirementDate,
                Price = price,
                SoldPrice = soldPrice,
                OperatingSystem = operatingSystem
            };
        }
    }
}
