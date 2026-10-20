using InventoryManagementSystemApplication.deploymentRegistry.ReadModels;
using InventoryManagementSystemDomain.modules.deploymentRegistry.Entities;

namespace InventoryManagementSystemApplication.deploymentRegistry.IRepositories
{
    public interface IBuildRepository
    {
        Task<Build?> GetById(Guid id);
        Task Create(Build build);
        Task Update(Build build);
        Task Delete(Guid id);

        // Read side
        Task<BuildDetails?> GetDetails(Guid id);
        Task<List<BuildDetails>> Search(string? version);
    }
}
