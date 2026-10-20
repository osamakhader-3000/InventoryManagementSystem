using InventoryManagementSystem.DeploymentRegistry.Mappers;
using InventoryManagementSystemApplication.deploymentRegistry.Features.Clients.CreateClient;
using InventoryManagementSystemApplication.deploymentRegistry.Features.Clients.DeleteClient;
using InventoryManagementSystemApplication.deploymentRegistry.Features.Clients.GetClient;
using InventoryManagementSystemApplication.deploymentRegistry.Features.Clients.ListClients;
using InventoryManagementSystemApplication.deploymentRegistry.Features.Clients.UpdateClient;
using InventoryManagementSystemDomain.modules.deploymentRegistry.Enums.Errors;
using Microsoft.AspNetCore.Mvc;
using Dto = InventoryManagementSystem.DeploymentRegistry.DataTransferObject;

namespace InventoryManagementSystem.DeploymentRegistry.APIs
{
    public static class Client
    {
        public static void Map(WebApplication app, RouteGroupBuilder apiGroup)
        {
            RouteGroupBuilder clientGroup = apiGroup.MapGroup("/client");
            clientGroup.MapGet("/list", ListClients).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            clientGroup.MapGet("/Get/{id}", GetClient).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            clientGroup.MapPost("/create", CreateClient).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            clientGroup.MapPut("/update", UpdateClient).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            clientGroup.MapDelete("/delete/{id}", DeleteClient).RequireAuthorization("VerifiedEmail", "ActiveAccount");
        }

        private static async Task<IResult> ListClients([FromServices] ListClientsUseCase listClients, string? name)
        {
            var clients = await listClients.Execute(name);
            return Results.Ok(ClientMapper.ToDtoList(clients));
        }

        private static async Task<IResult> GetClient([FromServices] GetClientUseCase getClient, Guid id)
        {
            var client = await getClient.Execute(id);
            if (client == null)
                return Results.NotFound();
            return Results.Ok(ClientMapper.ToDto(client));
        }

        // Builds own the build <-> client association, so dtoClient.Builds is ignored on create/update
        private static async Task<IResult> CreateClient([FromServices] CreateClientUseCase createClient, [FromBody] Dto.Client dtoClient)
        {
            var result = await createClient.Execute(new CreateClientRequest(dtoClient.Name, dtoClient.Description));
            if (!result.IsSuccess)
                return ToErrorResult(result.Error!.Value, result.Message);

            return Results.Created($"/client/Get/{result.Value}", new { Id = result.Value });
        }

        private static async Task<IResult> UpdateClient([FromServices] UpdateClientUseCase updateClient, [FromBody] Dto.Client dtoClient)
        {
            if (dtoClient.Id == null)
                return Results.BadRequest("Client id is required.");

            var result = await updateClient.Execute(new UpdateClientRequest(dtoClient.Id.Value, dtoClient.Name, dtoClient.Description));
            if (!result.IsSuccess)
                return ToErrorResult(result.Error!.Value, result.Message);

            return Results.Ok();
        }

        private static async Task<IResult> DeleteClient([FromServices] DeleteClientUseCase deleteClient, Guid id)
        {
            var result = await deleteClient.Execute(id);
            if (!result.IsSuccess)
                return ToErrorResult(result.Error!.Value, result.Message);

            return Results.Ok();
        }

        private static IResult ToErrorResult(ClientError error, string? message) =>
            error switch
            {
                ClientError.NotFound => Results.NotFound(message),
                _ => Results.BadRequest(message)
            };
    }
}
