using InventoryManagementSystemApplication.deploymentRegistry.IRepositories;
using InventoryManagementSystemDomain.modules.deploymentRegistry.Entities;
using InventoryManagementSystemDomain.modules.deploymentRegistry.Enums.Errors;
using SecurityDomain.Models;

namespace InventoryManagementSystemApplication.deploymentRegistry.Features.Builds.CreateBuild
{
    public class CreateBuildUseCase
    {
        private readonly IBuildRepository _buildRepository;
        private readonly IClientRepository _clientRepository;

        public CreateBuildUseCase(IBuildRepository buildRepository, IClientRepository clientRepository)
        {
            _buildRepository = buildRepository;
            _clientRepository = clientRepository;
        }

        public async Task<Result<Guid, BuildError>> Execute(CreateBuildRequest request)
        {
            var clientIds = (request.ClientIds ?? Array.Empty<Guid>()).Distinct().ToList();

            var existingClientIds = await _clientRepository.ListExistingIds(clientIds);
            var missingClientIds = clientIds.Except(existingClientIds).ToList();
            if (missingClientIds.Any())
                return Result<Guid, BuildError>.Failure(BuildError.ClientNotFound, $"Clients not found: {string.Join(", ", missingClientIds)}");

            var buildResult = Build.Create(request.Version, request.Link, clientIds);
            if (!buildResult.IsSuccess)
                return Result<Guid, BuildError>.Failure(buildResult.Error!.Value, buildResult.Message);

            await _buildRepository.Create(buildResult.Value!);
            return Result<Guid, BuildError>.Success(buildResult.Value!.Id);
        }
    }
}
