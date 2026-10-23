using System;
using System.Collections.Generic;
using Domain = InventoryManagementSystemDomain.modules.inventory.Entities;
using Invent = InventoryManagementSystemDataAccess.Models.Inventory;

namespace InventoryManagementSystemDataAccess.Mappers.Inventory
{
    internal static class DevicesMapper
    {
        public static Domain.Device ToDomainModel(this Invent.Device device)
        {
            return Domain.Device.Reconstitute(
                device.Id,
                device.Model,
                device.SerialNumber,
                device.Specs,
                device.PurchasedDate,
                device.RetirementDate,
                device.Price,
                device.SoldPrice ?? 0m,
                device.Status?? string.Empty,
                (InventoryManagementSystemDomain.modules.inventory.Enums.Availability)device.IsTaken,
                (InventoryManagementSystemDomain.modules.inventory.Enums.DeviceSoldState)device.IsSold);
        }

        public static Invent.Device ToDataAccessModel(this Domain.Device device)
        {
            return new Invent.Device
            {
                Id = device.Id,
                Model = device.Model,
                SerialNumber = device.SerialNumber,
                Specs = device.Specs,
                PurchasedDate = device.PurchasedDate,
                RetirementDate = device.RetirementDate,
                Price = device.Price,
                SoldPrice = device.SoldPrice,
                Status = device.Status,
                IsTaken = (byte)device.IsTaken,
                IsSold = (byte)device.IsSold
            };
        }
    }
}
