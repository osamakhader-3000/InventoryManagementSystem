using InventoryManagementSystemDataAccess.Mappers.Inventory;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
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
            Invent.Device? dbDevice = await _context.InventoryDevices.FirstOrDefaultAsync(x => x.Id == id);

            if (dbDevice == null)
                return null;

            return dbDevice.ToDomainModel();
        }

        public async Task<List<Device>> ListAll()
        {
            var dbDevices = await _context.InventoryDevices.ToListAsync();
            return dbDevices.ToDomainModelList(x => x.ToDomainModel());
        }

        public Task Update(Device device)
        {
            _context.InventoryDevices.Update(device.ToDataAccessModel());
            return _context.SaveChangesAsync();
        }
    }
}
