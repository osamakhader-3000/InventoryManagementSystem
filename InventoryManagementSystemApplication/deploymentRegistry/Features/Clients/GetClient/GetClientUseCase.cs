using InventoryManagementSystemApplication.deploymentRegistry.IRepositories;
using InventoryManagementSystemApplication.deploymentRegistry.ReadModels;

namespace InventoryManagementSystemApplication.deploymentRegistry.Features.Clients.GetClient
{
    public class GetClientUseCase
    {
        private readonly IClientRepository _clientRepository;

        public GetClientUseCase(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public Task<ClientDetails?> Execute(Guid id)
        {
            return _clientRepository.GetDetails(id);
        }
    }
}
