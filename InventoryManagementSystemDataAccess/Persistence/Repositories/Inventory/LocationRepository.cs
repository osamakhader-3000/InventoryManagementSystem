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
    public class LocationRepository : ILocationRepository
    {
        private readonly SecurityApplicationDataAccessContext _context;
        public LocationRepository(SecurityApplicationDataAccessContext context)
        {
            _context = context;
        }

        public Task Create(Location location)
        {
            var dbLocation = new Invent.Location
            {
                Id = location.Id,
                Name = location.Name
            };

            _context.InventoryLocations.Add(dbLocation);
            return _context.SaveChangesAsync();
        }

        public Task Delete(Guid id)
        {
            _context.InventoryLocations.Remove(new Invent.Location { Id = id });
            return _context.SaveChangesAsync();
        }

        public async Task<Location?> GetById(Guid id)
        {
            Invent.Location? dbLocation = await _context.InventoryLocations.FindAsync(id);

            if (dbLocation == null)
                return null;

            return dbLocation.ToDomainModel();
        }

        public async Task<List<Location>> ListAll(string? name)
        {
            var query = _context.InventoryLocations.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                // Escape LIKE wildcards so user input like "50%" or "a_b" is matched literally
                var escaped = name.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
                query = query.Where(l => EF.Functions.ILike(l.Name, $"%{escaped}%"));
            }

            var dbLocations = await query.ToListAsync();
            return dbLocations.ToDomainModelList(x => x.ToDomainModel());
        }

        public Task Update(Location location)
        {
            var dbLocation = new Invent.Location
            {
                Id = location.Id,
                Name = location.Name
            };
            _context.InventoryLocations.Update(dbLocation);
            return _context.SaveChangesAsync();
        }
    }
}
