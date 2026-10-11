using InventoryManagementSystemApplication.inventory.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Dto = InventoryManagementSystem.Inventory.DataTransferObject;
using DomainEntities = InventoryManagementSystemDomain.modules.inventory.Entities;

namespace InventoryManagementSystem.Inventory.APIs
{
    public static class Department
    {
        public static void Map(WebApplication app, RouteGroupBuilder apiGroup)
        {
            RouteGroupBuilder departmentGroup = apiGroup.MapGroup("/department");
            departmentGroup.MapGet("/list", ListDepartments).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            departmentGroup.MapGet("/Get/{id}", GetDepartment).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            departmentGroup.MapPost("/create", CreateDepartment).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            departmentGroup.MapPut("/update", UpdateDepartment).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            departmentGroup.MapDelete("/delete/{id}", DeleteDepartment).RequireAuthorization("VerifiedEmail", "ActiveAccount");
        }

        private static async Task<IResult> ListDepartments([FromServices] IDepartmentService departmentService)
        {
            var departments = await departmentService.List();
            return Results.Ok(InventoryManagementSystem.Inventory.Mappers.DepartmentMapper.ToDtoList(departments));
        }

        private static async Task<IResult> GetDepartment([FromServices] IDepartmentService departmentService, Guid id)
        {
            var department = await departmentService.Get(id);
            if (department == null)
                return Results.NotFound();
            return Results.Ok(InventoryManagementSystem.Inventory.Mappers.DepartmentMapper.ToDto(department));
        }

        private static async Task<IResult> CreateDepartment([FromServices] IDepartmentService departmentService, [FromBody] Dto.Department dtoDepartment)
        {
            DomainEntities.Department department = DomainEntities.Department.Create(dtoDepartment.Name);

            var createdDepartmentId = await departmentService.Create(department);
            return Results.Created($"/department/Get/{createdDepartmentId}", new { Id = createdDepartmentId });
        }

        private static async Task<IResult> UpdateDepartment([FromServices] IDepartmentService departmentService, [FromBody] Dto.Department dtoDepartment)
        {
            DomainEntities.Department department = DomainEntities.Department.Create(dtoDepartment.Name);
            await departmentService.Update(department);     
            return Results.NoContent();
        }

        private static async Task<IResult> DeleteDepartment([FromServices] IDepartmentService departmentService, Guid id)
        {
            await departmentService.Delete(id);
            return Results.NoContent();
        }
    }
}
