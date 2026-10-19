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
        public OperatingSystem OperatingSystem { get; private set; }

        public Enums.DeviceStatus Status { get; private set; } = Enums.DeviceStatus.New;

        public Enums.Availability IsTaken { get; private set; } = Enums.Availability.Taken;

        public Enums.DeviceSoldState IsSold { get; private set; } = Enums.DeviceSoldState.NotSold;

        private Device(Guid id, OperatingSystem operatingSystem)
        {
            Id = id;
            OperatingSystem = operatingSystem;
        }

        public static Device Create(
            string model,
            string serialNumber,
            string specs,
            DateTime? purchasedDate,
            DateTime? retirementDate,
            decimal price,
            decimal soldPrice,
            OperatingSystem operatingSystem,
            Enums.DeviceStatus status)
        {
            return new Device(Guid.NewGuid(), operatingSystem)
            {
                Model = model,
                SerialNumber = serialNumber,
                Specs = specs,
                PurchasedDate = purchasedDate,
                RetirementDate = retirementDate,
                Price = price,
                SoldPrice = soldPrice,
                Status = status,
                IsTaken = Enums.Availability.Available
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
            OperatingSystem operatingSystem,
            Enums.DeviceStatus status,
            Enums.Availability isAvailable = Enums.Availability.Available,
            Enums.DeviceSoldState isSold = Enums.DeviceSoldState.NotSold)
        {
            return new Device(id, operatingSystem)
            {
                Model = model,
                SerialNumber = serialNumber,
                Specs = specs,
                PurchasedDate = purchasedDate,
                RetirementDate = retirementDate,
                Price = price,
                SoldPrice = soldPrice,
                Status = status,
                IsTaken = isAvailable,
                IsSold = isSold
            };
        }
    }
}
