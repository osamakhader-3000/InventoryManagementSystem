using InventoryManagementSystemApplication.deploymentRegistry.IRepositories;
using InventoryManagementSystemDomain.modules.deploymentRegistry.Enums.Errors;
using SecurityDomain.Models;

namespace InventoryManagementSystemApplication.deploymentRegistry.Features.Clients.DeleteClient
{
    public class DeleteClientUseCase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IBuildRepository _buildRepository;

        public DeleteClientUseCase(IClientRepository clientRepository, IBuildRepository buildRepository)
        {
            _clientRepository = clientRepository;
            _buildRepository = buildRepository;
        }

        public async Task<Result<ClientError>> Execute(Guid id)
        {
            var client = await _clientRepository.GetById(id);
            if (client == null)
                return Result<ClientError>.Failure(ClientError.NotFound, $"Client not found: {id}");

            var isRelatedToBuilds = await _buildRepository.IsAnyRelatedToClient(id);
            var canBeDeleted = client.EnsureCanBeDeleted(isRelatedToBuilds);
            if (!canBeDeleted.IsSuccess)
                return canBeDeleted;

            await _clientRepository.Delete(id);
            return Result<ClientError>.Success();
        }
    }
}
