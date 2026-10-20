using InventoryManagementSystemApplication.deploymentRegistry.IRepositories;
using InventoryManagementSystemApplication.deploymentRegistry.ReadModels;

namespace InventoryManagementSystemApplication.deploymentRegistry.Features.Clients.ListClients
{
    public class ListClientsUseCase
    {
        private readonly IClientRepository _clientRepository;

        public ListClientsUseCase(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        // Matches clients whose name contains the given text (case-insensitive LIKE)
        public Task<List<ClientDetails>> Execute(string? name)
        {
            return _clientRepository.Search(name);
        }
    }
}
