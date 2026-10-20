using InventoryManagementSystemApplication.deploymentRegistry.IRepositories;
using InventoryManagementSystemDomain.modules.deploymentRegistry.Enums.Errors;
using SecurityDomain.Models;

namespace InventoryManagementSystemApplication.deploymentRegistry.Features.Builds.UpdateBuild
{
    public class UpdateBuildUseCase
    {
        private readonly IBuildRepository _buildRepository;
        private readonly IClientRepository _clientRepository;

        public UpdateBuildUseCase(IBuildRepository buildRepository, IClientRepository clientRepository)
        {
            _buildRepository = buildRepository;
            _clientRepository = clientRepository;
        }

        public async Task<Result<BuildError>> Execute(UpdateBuildRequest request)
        {
            var build = await _buildRepository.GetById(request.Id);
            if (build == null)
                return Result<BuildError>.Failure(BuildError.NotFound, $"Build not found: {request.Id}");

            var clientIds = (request.ClientIds ?? Array.Empty<Guid>()).Distinct().ToList();

            var existingClientIds = await _clientRepository.ListExistingIds(clientIds);
            var missingClientIds = clientIds.Except(existingClientIds).ToList();
            if (missingClientIds.Any())
                return Result<BuildError>.Failure(BuildError.ClientNotFound, $"Clients not found: {string.Join(", ", missingClientIds)}");

            var updateResult = build.Update(request.Version, request.Link, clientIds);
            if (!updateResult.IsSuccess)
                return updateResult;

            await _buildRepository.Update(build);
            return Result<BuildError>.Success();
        }
    }
}
