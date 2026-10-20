using Domain = InventoryManagementSystemDomain.modules.deploymentRegistry.Entities;
using DbModels = InventoryManagementSystemDataAccess.Models.DeploymentRegistry;

namespace InventoryManagementSystemDataAccess.Mappers.DeploymentRegistry
{
    internal static class DeploymentRegistryMapper
    {
        // Expects Clients to be loaded
        public static Domain.Build ToDomainModel(this DbModels.Build build)
        {
            return Domain.Build.Reconstitute(build.Id, build.Version, build.Link, build.Clients.Select(c => c.Id));
        }

        public static Domain.Client ToDomainModel(this DbModels.Client client)
        {
            return Domain.Client.Reconstitute(client.Id, client.Name, client.Description);
        }
    }
}
