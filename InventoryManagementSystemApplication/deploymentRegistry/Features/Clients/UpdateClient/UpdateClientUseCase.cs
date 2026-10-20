using InventoryManagementSystemApplication.deploymentRegistry.IRepositories;
using InventoryManagementSystemDomain.modules.deploymentRegistry.Enums.Errors;
using SecurityDomain.Models;

namespace InventoryManagementSystemApplication.deploymentRegistry.Features.Clients.UpdateClient
{
    public class UpdateClientUseCase
    {
        private readonly IClientRepository _clientRepository;

        public UpdateClientUseCase(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<Result<ClientError>> Execute(UpdateClientRequest request)
        {
            var client = await _clientRepository.GetById(request.Id);
            if (client == null)
                return Result<ClientError>.Failure(ClientError.NotFound, $"Client not found: {request.Id}");

            var updateResult = client.Update(request.Name, request.Description);
            if (!updateResult.IsSuccess)
                return updateResult;

            await _clientRepository.Update(client);
            return Result<ClientError>.Success();
        }
    }
}
