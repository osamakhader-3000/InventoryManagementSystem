using InventoryManagementSystemApplication.deploymentRegistry.IRepositories;
using InventoryManagementSystemApplication.deploymentRegistry.ReadModels;

namespace InventoryManagementSystemApplication.deploymentRegistry.Features.Builds.ListBuilds
{
    public class ListBuildsUseCase
    {
        private readonly IBuildRepository _buildRepository;

        public ListBuildsUseCase(IBuildRepository buildRepository)
        {
            _buildRepository = buildRepository;
        }

        // Matches builds whose version contains the given text (case-insensitive LIKE)
        public Task<List<BuildDetails>> Execute(string? version)
        {
            return _buildRepository.Search(version);
        }
    }
}
