using Dto = InventoryManagementSystem.Inventory.DataTransferObject;

namespace InventoryManagementSystem.Inventory.Mappers
{
    public class DepartmentMapper
    {
        public static Dto.Department ToDto(InventoryManagementSystemDomain.modules.inventory.Entities.Department department)
        {
            return new Dto.Department
            {
                Id = department.Id,
                Name = department.Name
            };
        }

        public static List<Dto.Department> ToDtoList(IEnumerable<InventoryManagementSystemDomain.modules.inventory.Entities.Department> departments)
        {
            return departments.Select(ToDto).ToList();
        }
    }
}
