using InventoryManagementSystemDataAccess.Mappers.Inventory;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using Microsoft.EntityFrameworkCore;
using Domain = InventoryManagementSystemDomain.modules.inventory.Entities;
using Invent = InventoryManagementSystemDataAccess.Models.Inventory;

namespace InventoryManagementSystemDataAccess.Persistence.Repositories.Inventory
{
    public class OperatingSystemRepository : IOperatingSystemRepository
    {
        private readonly SecurityApplicationDataAccessContext _context;
        public OperatingSystemRepository(SecurityApplicationDataAccessContext context)
        {
            _context = context;
        }

        public Task Create(Domain.OperatingSystem operatingSystem)
        {
            var dbOperatingSystem = new Invent.OperatingSystem
            {
                Id = operatingSystem.Id,
                Name = operatingSystem.Name
            };

            _context.InventoryOperatingSystems.Add(dbOperatingSystem);
            return _context.SaveChangesAsync();
        }

        public Task Delete(Guid id)
        {
            _context.InventoryOperatingSystems.Remove(new Invent.OperatingSystem { Id = id });
            return _context.SaveChangesAsync();
        }

        public async Task<Domain.OperatingSystem?> GetById(Guid id)
        {
            Invent.OperatingSystem? dbOperatingSystem = await _context.InventoryOperatingSystems.FindAsync(id);

            if (dbOperatingSystem == null)
                return null;

            return dbOperatingSystem.ToDomainModel();
        }

        public async Task<List<Domain.OperatingSystem>> ListAll()
        {
            var dbOperatingSystems = await _context.InventoryOperatingSystems.AsNoTracking().ToListAsync();
            return dbOperatingSystems.ToDomainModelList(x => x.ToDomainModel());
        }

        public async Task Update(Domain.OperatingSystem operatingSystem)
        {
            Invent.OperatingSystem? dbOperatingSystem = await _context.InventoryOperatingSystems.FindAsync(operatingSystem.Id);

            if (dbOperatingSystem == null)
                return;

            dbOperatingSystem.Name = operatingSystem.Name;
            await _context.SaveChangesAsync();
        }
    }
}
