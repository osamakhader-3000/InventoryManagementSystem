using InventoryManagementSystemApplication.inventory.Exceptions;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.Enums;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Services
{
    // Frees an assignment's device and accessories; callers must run it inside their own transaction
    public class AssignmentReleaser
    {
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IAccessoryRepository _accessoryRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ITrialLogRepository _trialLogRepository;
        public AssignmentReleaser(
            IAssignmentRepository assignmentRepository,
            IAccessoryRepository accessoryRepository,
            IDeviceRepository deviceRepository,
            ITrialLogRepository trialLogRepository)
        {
            _assignmentRepository = assignmentRepository;
            _accessoryRepository = accessoryRepository;
            _deviceRepository = deviceRepository;
            _trialLogRepository = trialLogRepository;
        }

        public async Task Release(Assignment assignment, Guid agentId)
        {
            long assignmentId = assignment.Id!.Value;

            // Rows returned before returns started deleting still exist; their device and accessories are already released
            if (assignment.Activity == AssignmentActivity.Returned)
                throw new AssignmentAlreadyReturnedException(assignmentId);

            // Delete first: a concurrent return of the same assignment gets false here and rolls back before releasing anything twice
            if (!await _assignmentRepository.Delete(assignmentId))
                throw new AssignmentAlreadyReturnedException(assignmentId);

            await _deviceRepository.MarkAvailable(assignment.Device.Id);

            // Release only this assignment's share; other assignments may still hold the same accessory
            foreach (var accessoryId in assignment.Accessories.Select(x => x.Id).Distinct())
                await _accessoryRepository.DecrementAssignedQuantity(accessoryId);

            await _trialLogRepository.Create(TrialLog.Create(
                assignment.Device.Id,
                assignment.AssignedUser.Id,
                DateTime.UtcNow,
                TransactionType.Return,
                agentId));
        }
    }
}
