using InventoryManagementSystemDomain.modules.deploymentRegistry.Enums.Errors;
using SecurityApplicationCore.Models;
using SecurityDomain.Models;

namespace InventoryManagementSystemDomain.modules.deploymentRegistry.Entities
{
    // Aggregate root that owns the build <-> client association; clients are referenced by id only
    public sealed class Build : AggregateRoot<Guid>
    {
        private readonly HashSet<Guid> _clientIds = new();

        public string Version { get; private set; } = string.Empty;
        public string? Link { get; private set; }
        public IReadOnlyCollection<Guid> ClientIds => _clientIds;

        private Build(Guid id) : base(id) { }

        public static Result<Build, BuildError> Create(string? version, string? link, IEnumerable<Guid>? clientIds)
        {
            var build = new Build(Guid.NewGuid());
            var result = build.Update(version, link, clientIds);

            return result.IsSuccess
                ? Result<Build, BuildError>.Success(build)
                : Result<Build, BuildError>.Failure(result.Error!.Value, result.Message);
        }

        public static Build Reconstitute(Guid id, string version, string? link, IEnumerable<Guid> clientIds)
        {
            var build = new Build(id)
            {
                Version = version,
                Link = link
            };
            build._clientIds.UnionWith(clientIds);
            return build;
        }

        public Result<BuildError> Update(string? version, string? link, IEnumerable<Guid>? clientIds)
        {
            if (string.IsNullOrWhiteSpace(version))
                return Result<BuildError>.Failure(BuildError.VersionRequired, "Build version is required.");

            Version = version.Trim();
            Link = string.IsNullOrWhiteSpace(link) ? null : link.Trim();

            _clientIds.Clear();
            _clientIds.UnionWith(clientIds ?? Enumerable.Empty<Guid>());

            return Result<BuildError>.Success();
        }

        public Result<BuildError> EnsureCanBeDeleted()
        {
            if (_clientIds.Count > 0)
                return Result<BuildError>.Failure(BuildError.LinkedToClients, "Cannot delete build because it is related to one or more clients.");

            return Result<BuildError>.Success();
        }
    }
}
