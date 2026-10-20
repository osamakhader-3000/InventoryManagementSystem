using InventoryManagementSystemApplication.deploymentRegistry.IRepositories;
using InventoryManagementSystemApplication.deploymentRegistry.ReadModels;

namespace InventoryManagementSystemApplication.deploymentRegistry.Features.Builds.GetBuild
{
    public class GetBuildUseCase
    {
        private readonly IBuildRepository _buildRepository;

        public GetBuildUseCase(IBuildRepository buildRepository)
        {
            _buildRepository = buildRepository;
        }

        public Task<BuildDetails?> Execute(Guid id)
        {
            return _buildRepository.GetDetails(id);
        }
    }
}
