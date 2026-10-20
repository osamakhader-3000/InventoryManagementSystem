using InventoryManagementSystemApplication.deploymentRegistry.IRepositories;
using InventoryManagementSystemApplication.deploymentRegistry.ReadModels;
using InventoryManagementSystemDataAccess.Mappers.DeploymentRegistry;
using InventoryManagementSystemDataAccess.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Domain = InventoryManagementSystemDomain.modules.deploymentRegistry.Entities;
using DbModels = InventoryManagementSystemDataAccess.Models.DeploymentRegistry;

namespace InventoryManagementSystemDataAccess.Persistence.Repositories.DeploymentRegistry
{
    public class BuildRepository : IBuildRepository
    {
        private readonly SecurityApplicationDataAccessContext _context;

        public BuildRepository(SecurityApplicationDataAccessContext context)
        {
            _context = context;
        }

        public async Task<Domain.Build?> GetById(Guid id)
        {
            var dbBuild = await _context.DeploymentRegistryBuilds
                .AsNoTracking()
                .Include(b => b.Clients)
                .FirstOrDefaultAsync(b => b.Id == id);

            return dbBuild?.ToDomainModel();
        }

        public async Task Create(Domain.Build build)
        {
            var dbBuild = new DbModels.Build
            {
                Id = build.Id,
                Version = build.Version,
                Link = build.Link,
                Clients = await LoadClients(build.ClientIds)
            };

            _context.DeploymentRegistryBuilds.Add(dbBuild);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Domain.Build build)
        {
            var dbBuild = await _context.DeploymentRegistryBuilds
                .Include(b => b.Clients)
                .FirstOrDefaultAsync(b => b.Id == build.Id)
                ?? throw new KeyNotFoundException($"Build not found: {build.Id}");

            dbBuild.Version = build.Version;
            dbBuild.Link = build.Link;

            // Sync the association with the aggregate's client ids
            dbBuild.Clients.RemoveAll(c => !build.ClientIds.Contains(c.Id));
            var currentClientIds = dbBuild.Clients.Select(c => c.Id).ToHashSet();
            var addedClientIds = build.ClientIds.Where(id => !currentClientIds.Contains(id)).ToList();
            dbBuild.Clients.AddRange(await LoadClients(addedClientIds));

            await _context.SaveChangesAsync();
        }

        public Task Delete(Guid id)
        {
            _context.DeploymentRegistryBuilds.Remove(new DbModels.Build { Id = id });
            return _context.SaveChangesAsync();
        }

        public Task<bool> IsAnyRelatedToClient(Guid clientId)
        {
            return _context.DeploymentRegistryBuilds.AnyAsync(b => b.Clients.Any(c => c.Id == clientId));
        }

        public Task<BuildDetails?> GetDetails(Guid id)
        {
            return _context.DeploymentRegistryBuilds
                .AsNoTracking()
                .Where(b => b.Id == id)
                .Select(b => new BuildDetails(
                    b.Id,
                    b.Version,
                    b.Link,
                    b.Clients.OrderBy(c => c.Name).Select(c => new ClientSummary(c.Id, c.Name)).ToList()))
                .FirstOrDefaultAsync();
        }

        public Task<List<BuildDetails>> Search(string? version)
        {
            return _context.DeploymentRegistryBuilds
                .AsNoTracking()
                .WhereContainsIgnoreCase(b => b.Version, version)
                .OrderBy(b => b.Version)
                .Select(b => new BuildDetails(
                    b.Id,
                    b.Version,
                    b.Link,
                    b.Clients.OrderBy(c => c.Name).Select(c => new ClientSummary(c.Id, c.Name)).ToList()))
                .ToListAsync();
        }

        private async Task<List<DbModels.Client>> LoadClients(IEnumerable<Guid> clientIds)
        {
            var ids = clientIds.ToList();
            if (ids.Count == 0)
                return new List<DbModels.Client>();

            return await _context.DeploymentRegistryClients
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();
        }
    }
}
