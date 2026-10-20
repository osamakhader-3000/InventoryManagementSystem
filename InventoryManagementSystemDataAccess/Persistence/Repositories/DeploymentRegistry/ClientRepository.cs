using InventoryManagementSystemApplication.deploymentRegistry.IRepositories;
using InventoryManagementSystemApplication.deploymentRegistry.ReadModels;
using InventoryManagementSystemDataAccess.Mappers.DeploymentRegistry;
using InventoryManagementSystemDataAccess.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Domain = InventoryManagementSystemDomain.modules.deploymentRegistry.Entities;
using DbModels = InventoryManagementSystemDataAccess.Models.DeploymentRegistry;

namespace InventoryManagementSystemDataAccess.Persistence.Repositories.DeploymentRegistry
{
    public class ClientRepository : IClientRepository
    {
        private readonly SecurityApplicationDataAccessContext _context;

        public ClientRepository(SecurityApplicationDataAccessContext context)
        {
            _context = context;
        }

        public async Task<Domain.Client?> GetById(Guid id)
        {
            var dbClient = await _context.DeploymentRegistryClients
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            return dbClient?.ToDomainModel();
        }

        public Task Create(Domain.Client client)
        {
            var dbClient = new DbModels.Client
            {
                Id = client.Id,
                Name = client.Name,
                Description = client.Description
            };

            _context.DeploymentRegistryClients.Add(dbClient);
            return _context.SaveChangesAsync();
        }

        public Task Update(Domain.Client client)
        {
            var dbClient = new DbModels.Client
            {
                Id = client.Id,
                Name = client.Name,
                Description = client.Description
            };

            // Builds own the association, so only scalar columns are updated here
            _context.DeploymentRegistryClients.Update(dbClient);
            return _context.SaveChangesAsync();
        }

        public Task Delete(Guid id)
        {
            _context.DeploymentRegistryClients.Remove(new DbModels.Client { Id = id });
            return _context.SaveChangesAsync();
        }

        public async Task<List<Guid>> ListExistingIds(IEnumerable<Guid> ids)
        {
            var idList = ids.ToList();
            if (idList.Count == 0)
                return new List<Guid>();

            return await _context.DeploymentRegistryClients
                .Where(c => idList.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync();
        }

        public Task<ClientDetails?> GetDetails(Guid id)
        {
            return _context.DeploymentRegistryClients
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new ClientDetails(
                    c.Id,
                    c.Name,
                    c.Description,
                    c.Builds.OrderBy(b => b.Version).Select(b => new BuildSummary(b.Id, b.Version)).ToList()))
                .FirstOrDefaultAsync();
        }

        public Task<List<ClientDetails>> Search(string? name)
        {
            return _context.DeploymentRegistryClients
                .AsNoTracking()
                .WhereContainsIgnoreCase(c => c.Name, name)
                .OrderBy(c => c.Name)
                .Select(c => new ClientDetails(
                    c.Id,
                    c.Name,
                    c.Description,
                    c.Builds.OrderBy(b => b.Version).Select(b => new BuildSummary(b.Id, b.Version)).ToList()))
                .ToListAsync();
        }
    }
}
