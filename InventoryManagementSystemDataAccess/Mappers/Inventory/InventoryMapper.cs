using System;
using System.Collections.Generic;
using System.Text;
using Domain = InventoryManagementSystemDomain.modules.inventory.Entities;
using Invent = InventoryManagementSystemDataAccess.Models.Inventory;

namespace InventoryManagementSystemDataAccess.Mappers.Inventory
{
    internal static class InventoryMapper
    {
        public static Domain.Department? ToDomainModel(this Invent.Department department)
        {
            return Domain.Department.Reconstitute(department.Id, department.Name);
        }

        public static Domain.Accessory? ToDomainModel(this Invent.Accessory accessory)
        {
            return Domain.Accessory.Reconstitute(accessory.Id, accessory.Name, accessory.Quantity, accessory.Date);
        }

        public static Domain.Device? ToDomainModel(this Invent.Device device)
        {
            return Domain.Device.Reconstitute(
                device.Id,
                device.Model,
                device.SerialNumber,
                device.Specs,
                device.PurchasedDate,
                device.RetirementDate,
                (double)device.Price,
                (double)device.SoldPrice,
                (InventoryManagementSystemDomain.modules.inventory.Enums.OperatingSystem)device.OperatingSystem);
        }

        public static Domain.User? ToDomainModel(this Invent.User user)
        {
            return Domain.User.Reconstitute(user.Id, user.Name, user.Department?.ToDomainModel());
        }

        public static Domain.Assignment? ToDomainModel(this Invent.Assignment assignment)
        {
            return Domain.Assignment.Reconstitute(
                assignment.Id,
                assignment.Device?.ToDomainModel(),
                assignment.AssignedUser?.ToDomainModel(),
                assignment.AssignmentDate,
                assignment.ReturnDate,
                assignment.Accessories.ToDomainModelList(a => a.ToDomainModel()),
                assignment.Remarks);
        }

        public static Invent.Accessory ToDataAccessModel(this Domain.Accessory accessory)
        {
            return new Invent.Accessory
            {
                Id = accessory.Id,
                Name = accessory.Name,
                Quantity = accessory.Quantity,
                Date = accessory.Date
            };
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
                Price = (double)device.Price,
                SoldPrice = (double)device.SoldPrice,
                OperatingSystem = (byte)device.OperatingSystem
            };
        }

        public static Invent.User ToDataAccessModel(this Domain.User user)
        {
            return new Invent.User
            {
                Id = user.Id,
                Name = user.Name,
                DepartmentId = user.Department.Id
            };
        }

        public static List<T> ToDomainModelList<TDb, T>(this IEnumerable<TDb> dbItems, Func<TDb, T?> map) where T : class
        {
            var items = new List<T>();
            foreach (var dbItem in dbItems)
            {
                var item = map(dbItem);
                if (item != null)
                {
                    items.Add(item);
                }
            }
            return items;
        }
    }
}
