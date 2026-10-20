using InventoryManagementSystemApplication.deploymentRegistry.IRepositories;
using InventoryManagementSystemDomain.modules.deploymentRegistry.Entities;
using InventoryManagementSystemDomain.modules.deploymentRegistry.Enums.Errors;
using SecurityDomain.Models;

namespace InventoryManagementSystemApplication.deploymentRegistry.Features.Clients.CreateClient
{
    public class CreateClientUseCase
    {
        private readonly IClientRepository _clientRepository;

        public CreateClientUseCase(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<Result<Guid, ClientError>> Execute(CreateClientRequest request)
        {
            var clientResult = Client.Create(request.Name, request.Description);
            if (!clientResult.IsSuccess)
                return Result<Guid, ClientError>.Failure(clientResult.Error!.Value, clientResult.Message);

            await _clientRepository.Create(clientResult.Value!);
            return Result<Guid, ClientError>.Success(clientResult.Value!.Id);
        }
    }
}
