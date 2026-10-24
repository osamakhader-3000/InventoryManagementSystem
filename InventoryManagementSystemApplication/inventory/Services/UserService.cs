using InventoryManagementSystemApplication.inventory.Exceptions;
using InventoryManagementSystemApplication.inventory.Interfaces;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.Enums;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using IUnitOfWork = SecurityApplicationLogic.Interfaces.IUnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly AssignmentReleaser _assignmentReleaser;
        private readonly IUnitOfWork _unitOfWork;
        public UserService(
            IUserRepository userRepository,
            IAssignmentRepository assignmentRepository,
            AssignmentReleaser assignmentReleaser,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _assignmentRepository = assignmentRepository;
            _assignmentReleaser = assignmentReleaser;
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Create(User user)
        {
            await _userRepository.Create(user);
            return user.Id;
        }
        public async Task<List<User>> List(string? name, Guid? departmentId, Guid? locationId, UserSuspensionState? status)
        {
            return await _userRepository.List(name, departmentId, locationId, status);
        }
        public async Task<User?> Get(Guid userId)
        {
            return await _userRepository.GetById(userId);
        }
        public async Task Update(User user)
        {
            await _userRepository.Update(user);
        }
        public async Task Suspend(Guid userId, Guid agentId)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Throwing rolls back the whole transaction, including any assignments already released
                if (!await _userRepository.Suspend(userId))
                    throw new UserAlreadySuspendedException(userId);

                var openAssignments = await _assignmentRepository.ListAll(userId, null, null, null, null, null, false);
                foreach (var assignment in openAssignments)
                    await _assignmentReleaser.Release(assignment, agentId);
            });
        }
        public async Task Unsuspend(Guid userId)
        {
            if (!await _userRepository.Unsuspend(userId))
                throw new UserNotSuspendedException(userId);
        }
    }
}
