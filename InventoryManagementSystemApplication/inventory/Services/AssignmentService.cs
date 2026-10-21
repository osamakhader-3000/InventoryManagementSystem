using InventoryManagementSystemApplication.inventory.Exceptions;
using InventoryManagementSystemApplication.inventory.Interfaces;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.Enums;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using SecurityApplicationLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IAccessoryRepository _accessoryRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ITrialLogRepository _trialLogRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AssignmentService(
            IAssignmentRepository assignmentRepository,
            IAccessoryRepository accessoryRepository,
            IDeviceRepository deviceRepository,
            ITrialLogRepository trialLogRepository,
            IUnitOfWork unitOfWork)
        {
            _assignmentRepository = assignmentRepository;
            _accessoryRepository = accessoryRepository;
            _deviceRepository = deviceRepository;
            _trialLogRepository = trialLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> Create(Assignment assignment, Guid agentId)
        {
            long createdAssignmentId = 0;

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                createdAssignmentId = await _assignmentRepository.Create(assignment);

                await TakeDevice(assignment.Device.Id);

                await IncrementAssignedQuantities(DistinctAccessoryIds(assignment));

                await AddTrialLog(assignment, TransactionType.Assign, agentId);
            });

            return createdAssignmentId;
        }

        public Task Update(Assignment assignment, Guid agentId)
        {
            return UpdateWithTrialLog(assignment, TransactionType.Update, agentId);
        }

        public Task Relocate(Assignment assignment, Guid agentId)
        {
            return UpdateWithTrialLog(assignment, TransactionType.Relocate, agentId);
        }

        public async Task Return(long assignmentId, Guid agentId)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var existingAssignment = await _assignmentRepository.GetById(assignmentId);
                if (existingAssignment == null)
                    return;

                // Rows returned before returns started deleting still exist; their device and accessories are already released
                if (existingAssignment.Activity == AssignmentActivity.Returned)
                    throw new AssignmentAlreadyReturnedException(assignmentId);

                // Delete first: a concurrent return of the same assignment gets false here and rolls back before releasing anything twice
                if (!await _assignmentRepository.Delete(assignmentId))
                    throw new AssignmentAlreadyReturnedException(assignmentId);

                await _deviceRepository.MarkAvailable(existingAssignment.Device.Id);

                // Release only this assignment's share; other assignments may still hold the same accessory
                foreach (var accessoryId in DistinctAccessoryIds(existingAssignment))
                    await _accessoryRepository.DecrementAssignedQuantity(accessoryId);

                await AddTrialLog(existingAssignment, TransactionType.Return, agentId);
            });
        }

        private async Task UpdateWithTrialLog(Assignment assignment, TransactionType transactionType, Guid agentId)
        {
            if (assignment.Id == null)
                throw new ArgumentException("Assignment id is required", nameof(assignment));

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var existingAssignment = await _assignmentRepository.GetById(assignment.Id.Value);
                if (existingAssignment == null)
                    return;

                var existingAccessoryIds = DistinctAccessoryIds(existingAssignment);
                var newAccessoryIds = DistinctAccessoryIds(assignment);

                // Throwing rolls back the whole transaction, so no device or accessory is taken for a returned assignment
                if (!await _assignmentRepository.Update(assignment))
                    throw new AssignmentAlreadyReturnedException(assignment.Id.Value);

                if (existingAssignment.Device.Id != assignment.Device.Id)
                {
                    await _deviceRepository.MarkAvailable(existingAssignment.Device.Id);
                    await TakeDevice(assignment.Device.Id);
                }

                await IncrementAssignedQuantities(newAccessoryIds.Except(existingAccessoryIds));

                // Already at 0 means the count was out of sync; nothing to release, so the result is ignored
                foreach (var accessoryId in existingAccessoryIds.Except(newAccessoryIds))
                    await _accessoryRepository.DecrementAssignedQuantity(accessoryId);

                await AddTrialLog(assignment, transactionType, agentId);
            });
        }

        private async Task TakeDevice(Guid deviceId)
        {
            // Throwing rolls back the whole transaction
            if (!await _deviceRepository.MarkTaken(deviceId))
                throw new DeviceUnavailableException(deviceId);
        }

        private async Task IncrementAssignedQuantities(IEnumerable<Guid> accessoryIds)
        {
            foreach (var accessoryId in accessoryIds)
            {
                // Throwing rolls back the whole transaction
                if (!await _accessoryRepository.IncrementAssignedQuantity(accessoryId))
                    throw new AccessoryOutOfStockException(accessoryId);
            }
        }

        private Task AddTrialLog(Assignment assignment, TransactionType transactionType, Guid agentId)
        {
            return _trialLogRepository.Create(TrialLog.Create(
                assignment.Device.Id,
                assignment.AssignedUser.Id,
                DateTime.UtcNow,
                transactionType,
                agentId));
        }

        private static List<Guid> DistinctAccessoryIds(Assignment assignment)
        {
            return assignment.Accessories.Select(x => x.Id).Distinct().ToList();
        }

        public async Task<List<Assignment>> List(Guid? userId, Guid? depId, Guid? locationId, DateTime? purDate, string? model, string? serialNumber, bool? isReturned)
        {
            return await _assignmentRepository.ListAll(userId, depId, locationId, purDate, model, serialNumber, isReturned);
        }
        public async Task<Assignment?> Get(long assignmentId)
        {
            return await _assignmentRepository.GetById(assignmentId);
        }

        public async Task<IEnumerable<Assignment>> ListByAccessoryId(Guid accessoryId)
        {
            return await _assignmentRepository.ListByAccessoryId(accessoryId);
        }
    }
}
