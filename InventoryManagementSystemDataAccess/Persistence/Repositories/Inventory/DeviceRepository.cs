using InventoryManagementSystemDataAccess.Mappers.Inventory;
using InventoryManagementSystemDataAccess.Persistence.Extensions;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.Enums;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Invent = InventoryManagementSystemDataAccess.Models.Inventory;

namespace InventoryManagementSystemDataAccess.Persistence.Repositories.Inventory
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly SecurityApplicationDataAccessContext _context;
        public DeviceRepository(SecurityApplicationDataAccessContext context)
        {
            _context = context;
        }

        public Task Create(Device device)
        {
            _context.InventoryDevices.Add(device.ToDataAccessModel());
            return _context.SaveChangesAsync();
        }

        public Task Delete(Guid id)
        {
            _context.InventoryDevices.Remove(new Invent.Device { Id = id });
            return _context.SaveChangesAsync();
        }

        public async Task<Device?> GetById(Guid id)
        {
            Invent.Device? dbDevice = await _context.InventoryDevices.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (dbDevice == null)
                return null;

            return dbDevice.ToDomainModel();
        }

        public async Task<Device?> GetBySerialNumber(string serialNumber)
        {
            Invent.Device? dbDevice = await _context.InventoryDevices.AsNoTracking().FirstOrDefaultAsync(x => x.SerialNumber == serialNumber);

            if (dbDevice == null)
                return null;

            return dbDevice.ToDomainModel();
        }

        public async Task<List<Device>> List(string? model,string? serialNumber,DateTime? purchasedDate)
        {
            var query = _context.InventoryDevices.AsQueryable();

            if (!string.IsNullOrEmpty(model))            
                query = query.WhereContainsIgnoreCase(x => x.Model, model);            

            if (!string.IsNullOrEmpty(serialNumber))            
                query = query.Where(x => x.SerialNumber == serialNumber);            

            if (purchasedDate.HasValue)
            {
                var dayStart = DateTime.SpecifyKind(purchasedDate.Value.Date, DateTimeKind.Utc);
                var dayEnd = dayStart.AddDays(1);
                query = query.Where(x => x.PurchasedDate >= dayStart && x.PurchasedDate < dayEnd);
            }

            var dbDevices = await query.ToListAsync();
            return dbDevices.ToDomainModelList(x => x.ToDomainModel());
        }

        public Task Update(Device device)
        {
            var entry = _context.InventoryDevices.Update(device.ToDataAccessModel());
            // Availability is owned by assignments, so device edits must not overwrite it
            entry.Property(x => x.IsTaken).IsModified = false;
            // Sold state is owned by the sell flow
            entry.Property(x => x.IsSold).IsModified = false;
            return _context.SaveChangesAsync();
        }

        // Single UPDATE statement so two concurrent assignments can't both take the device; returns false when already taken or sold
        public async Task<bool> MarkTaken(Guid id)
        {
            int affectedRows = await _context.InventoryDevices
                .Where(x => x.Id == id && x.IsTaken == (byte)Availability.Available && x.IsSold == (byte)DeviceSoldState.NotSold)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsTaken, (byte)Availability.Taken));
            return affectedRows == 1;
        }

        // Single UPDATE statement so a device can't be sold twice or sold while assigned; returns false when already sold or taken
        public async Task<bool> MarkSold(Guid id, decimal soldPrice)
        {
            int affectedRows = await _context.InventoryDevices
                .Where(x => x.Id == id && x.IsSold == (byte)DeviceSoldState.NotSold && x.IsTaken == (byte)Availability.Available)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.IsSold, (byte)DeviceSoldState.Sold)
                    .SetProperty(x => x.SoldPrice, soldPrice));
            return affectedRows == 1;
        }

        public Task MarkAvailable(Guid id)
        {
            return _context.InventoryDevices
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsTaken, (byte)Availability.Available));
        }
    }
}
