using InventoryManagementSystemDomain.modules.deploymentRegistry.Enums.Errors;
using SecurityApplicationCore.Models;
using SecurityDomain.Models;

namespace InventoryManagementSystemDomain.modules.deploymentRegistry.Entities
{
    public sealed class Client : AggregateRoot<Guid>
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }

        private Client(Guid id) : base(id) { }

        public static Result<Client, ClientError> Create(string? name, string? description)
        {
            var client = new Client(Guid.NewGuid());
            var result = client.Update(name, description);

            return result.IsSuccess
                ? Result<Client, ClientError>.Success(client)
                : Result<Client, ClientError>.Failure(result.Error!.Value, result.Message);
        }

        public static Client Reconstitute(Guid id, string name, string? description)
        {
            return new Client(id)
            {
                Name = name,
                Description = description
            };
        }

        public Result<ClientError> Update(string? name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<ClientError>.Failure(ClientError.NameRequired, "Client name is required.");

            Name = name.Trim();
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

            return Result<ClientError>.Success();
        }

        // Builds own the association, so whether this client is still in use is supplied by the caller
        public Result<ClientError> EnsureCanBeDeleted(bool isRelatedToBuilds)
        {
            if (isRelatedToBuilds)
                return Result<ClientError>.Failure(ClientError.LinkedToBuilds, "Cannot delete client because it is related to one or more builds.");

            return Result<ClientError>.Success();
        }
    }
}
