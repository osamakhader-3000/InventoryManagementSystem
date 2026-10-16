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

        public Task Delete(Guid id)
        {
            _context.InventoryAccessories.Remove(new Invent.Accessory { Id = id });
            return _context.SaveChangesAsync();
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
                query = query.Where(x => x.Quantity > 0 && stock == true || x.Quantity == 0 && stock == false || !stock.HasValue);
            }

            if (date.HasValue)
            {
                query = query.Where(x => x.Date == date.Value.Date);
            }

            var dbAccessories = await query.ToListAsync();
            return dbAccessories.ToDomainModelList(x => x.ToDomainModel());
        }

        public Task Update(Accessory accessory)
        {
            _context.InventoryAccessories.Update(accessory.ToDataAccessModel());
            return _context.SaveChangesAsync();
        }
    }
}
