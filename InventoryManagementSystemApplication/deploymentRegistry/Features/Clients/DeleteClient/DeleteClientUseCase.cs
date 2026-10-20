using InventoryManagementSystemApplication.deploymentRegistry.IRepositories;
using InventoryManagementSystemDomain.modules.deploymentRegistry.Enums.Errors;
using SecurityDomain.Models;

namespace InventoryManagementSystemApplication.deploymentRegistry.Features.Clients.DeleteClient
{
    public class DeleteClientUseCase
    {
        private readonly IClientRepository _clientRepository;

        public DeleteClientUseCase(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<Result<ClientError>> Execute(Guid id)
        {
            var client = await _clientRepository.GetById(id);
            if (client == null)
                return Result<ClientError>.Failure(ClientError.NotFound, $"Client not found: {id}");

            // The build <-> client association rows are removed by cascade
            await _clientRepository.Delete(id);
            return Result<ClientError>.Success();
        }
    }
}
