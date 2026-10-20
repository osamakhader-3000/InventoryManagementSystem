using InventoryManagementSystemApplication.deploymentRegistry.IRepositories;
using InventoryManagementSystemDomain.modules.deploymentRegistry.Enums.Errors;
using SecurityDomain.Models;

namespace InventoryManagementSystemApplication.deploymentRegistry.Features.Builds.DeleteBuild
{
    public class DeleteBuildUseCase
    {
        private readonly IBuildRepository _buildRepository;

        public DeleteBuildUseCase(IBuildRepository buildRepository)
        {
            _buildRepository = buildRepository;
        }

        public async Task<Result<BuildError>> Execute(Guid id)
        {
            var build = await _buildRepository.GetById(id);
            if (build == null)
                return Result<BuildError>.Failure(BuildError.NotFound, $"Build not found: {id}");

            var canBeDeleted = build.EnsureCanBeDeleted();
            if (!canBeDeleted.IsSuccess)
                return canBeDeleted;

            await _buildRepository.Delete(id);
            return Result<BuildError>.Success();
        }
    }
}
