using InventoryManagementSystemDataAccess.Mappers.Inventory;
using InventoryManagementSystemDataAccess.Persistence.Extensions;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Invent = InventoryManagementSystemDataAccess.Models.Inventory;

namespace InventoryManagementSystemDataAccess.Persistence.Repositories.Inventory
{
    public class AccessoryRepository : IAccessoryRepository
    {
        private readonly SecurityApplicationDataAccessContext _context;
        public AccessoryRepository(SecurityApplicationDataAccessContext context)
        {
            _context = context;
        }

        public Task Create(Accessory accessory)
        {
            _context.InventoryAccessories.Add(accessory.ToDataAccessModel());
            return _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            // FindAsync returns the already-tracked instance (e.g. loaded by GetById), avoiding duplicate tracking
            Invent.Accessory? dbAccessory = await _context.InventoryAccessories.FindAsync(id);
            if (dbAccessory == null)
                return;

            _context.InventoryAccessories.Remove(dbAccessory);
            await _context.SaveChangesAsync();
        }

        public async Task<Accessory?> GetById(Guid id)
        {
            Invent.Accessory? dbAccessory = await _context.InventoryAccessories.FirstOrDefaultAsync(x => x.Id == id);

            if (dbAccessory == null)
                return null;

            return dbAccessory.ToDomainModel();
        }

        public async Task<List<Accessory>> ListAll()
        {
            var dbAccessorys = await _context.InventoryAccessories.ToListAsync();
            return dbAccessorys.ToDomainModelList(x => x.ToDomainModel());
        }

        public async Task<IEnumerable<Accessory>> ListByIds(List<Guid> ids)
        {
            var dbAccessories = await _context.InventoryAccessories.Where(x => ids.Contains(x.Id)).ToListAsync();
            return dbAccessories.ToDomainModelList(x => x.ToDomainModel());
        }

        // Single UPDATE statement so concurrent assignments can't lose an increment; returns false when out of stock
        public async Task<bool> IncrementAssignedQuantity(Guid id)
        {
            int affectedRows = await _context.InventoryAccessories
                .Where(x => x.Id == id && x.AssignedQuantity < x.Quantity)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.AssignedQuantity, x => x.AssignedQuantity + 1));
            return affectedRows == 1;
        }

        public async Task<bool> DecrementAssignedQuantity(Guid id)
        {
            int affectedRows = await _context.InventoryAccessories
                .Where(x => x.Id == id && x.AssignedQuantity > 0)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.AssignedQuantity, x => x.AssignedQuantity - 1));
            return affectedRows == 1;
        }

        public async Task<List<Accessory>> List(string? name,bool? stock)
        {
            var query = _context.InventoryAccessories.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }

            if (stock.HasValue)
            {
                query = query.Where(x => x.Quantity > 0 && stock == true || x.Quantity == 0 && stock == false || !stock.HasValue);
            }

            var dbAccessories = await query.ToListAsync();
            return dbAccessories.ToDomainModelList(x => x.ToDomainModel());
        }

        public async Task<List<Accessory>> List(string? name,bool? stock, DateTime? date)
        {
            var query = _context.InventoryAccessories.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.WhereContainsIgnoreCase(x => x.Name, name);
            }

            if (stock.HasValue)
            {
                // In stock means some quantity is still available (not assigned).
                query = stock.Value
                    ? query.Where(x => x.Quantity - x.AssignedQuantity > 0)
                    : query.Where(x => x.Quantity - x.AssignedQuantity <= 0);
            }

            if (date.HasValue)
            {
                // Npgsql only accepts UTC DateTimes for timestamptz; match the whole day.
                var dayStart = DateTime.SpecifyKind(date.Value.Date, DateTimeKind.Utc);
                var dayEnd = dayStart.AddDays(1);
                query = query.Where(x => x.Date >= dayStart && x.Date < dayEnd);
            }

            var dbAccessories = await query.ToListAsync();
            return dbAccessories.ToDomainModelList(x => x.ToDomainModel());
        }

        public async Task Update(Accessory accessory)
        {
            // FindAsync returns the already-tracked instance if one exists, avoiding duplicate tracking
            Invent.Accessory? dbAccessory = await _context.InventoryAccessories.FindAsync(accessory.Id);
            if (dbAccessory == null)
                return;

            _context.Entry(dbAccessory).CurrentValues.SetValues(accessory.ToDataAccessModel());
            await _context.SaveChangesAsync();
        }
    }
}
