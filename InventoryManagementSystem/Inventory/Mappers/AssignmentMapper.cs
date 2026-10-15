using Dto = InventoryManagementSystem.Inventory.DataTransferObject;

namespace InventoryManagementSystem.Inventory.Mappers
{
    public class AssignmentMapper
    {
        public static Dto.Assignment ToDto(InventoryManagementSystemDomain.modules.inventory.Entities.Assignment assignment)
        {
            return new Dto.Assignment
            {
                Id = assignment.Id ?? 0,
                Device.Id = assignment.Device.Id,
                AssignedUser.Id = assignment.AssignedUser.Id,
                AssignmentDate = assignment.AssignmentDate,
                ReturnDate = assignment.ReturnDate,
                AccessoryIds = assignment.Accessories.Select(x => x.Id).ToList(),
                Remarks = assignment.Remarks,
                Device = DeviceMapper.ToDto(assignment.Device),
                AssignedUser = UserMapper.ToDto(assignment.AssignedUser),
                Accessories = AccessoryMapper.ToDtoList(assignment.Accessories)
            };
        }

        public static List<Dto.Assignment> ToDtoList(IEnumerable<InventoryManagementSystemDomain.modules.inventory.Entities.Assignment> assignments)
        {
            return assignments.Select(ToDto).ToList();
        }
    }
}
