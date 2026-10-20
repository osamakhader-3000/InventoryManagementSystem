using InventoryManagementSystemApplication.deploymentRegistry.ReadModels;
using InventoryManagementSystemDomain.modules.deploymentRegistry.Entities;

namespace InventoryManagementSystemApplication.deploymentRegistry.IRepositories
{
    public interface IClientRepository
    {
        Task<Client?> GetById(Guid id);
        Task Create(Client client);
        Task Update(Client client);
        Task Delete(Guid id);
        Task<List<Guid>> ListExistingIds(IEnumerable<Guid> ids);

        // Read side
        Task<ClientDetails?> GetDetails(Guid id);
        Task<List<ClientDetails>> Search(string? name);
    }
}
