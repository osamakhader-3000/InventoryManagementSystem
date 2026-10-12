using Dto = InventoryManagementSystem.Inventory.DataTransferObject;

namespace InventoryManagementSystem.Inventory.Mappers
{
    public class UserMapper
    {
        public static Dto.User ToDto(InventoryManagementSystemDomain.modules.inventory.Entities.User user)
        {
            return new Dto.User
            {
                Id = user.Id,
                Name = user.Name,
                DepartmentId = user.Department.Id,
                DepartmentName = user.Department.Name
            };
        }

        public static List<Dto.User> ToDtoList(IEnumerable<InventoryManagementSystemDomain.modules.inventory.Entities.User> users)
        {
            return users.Select(ToDto).ToList();
        }
    }
}
