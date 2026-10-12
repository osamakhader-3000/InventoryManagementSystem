using InventoryManagementSystemApplication.inventory.Interfaces;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _assignmentRepository;
        public AssignmentService(IAssignmentRepository assignmentRepository)
        {
            _assignmentRepository = assignmentRepository;
        }
        public async Task<long> Create(Assignment assignment)
        {
            return await _assignmentRepository.Create(assignment);
        }
        public async Task<List<Assignment>> List(Guid? userId, Guid? depId, DateTime? purDate)
        {
            return await _assignmentRepository.ListAll(userId, depId, purDate);
        }
        public async Task<Assignment?> Get(long assignmentId)
        {
            return await _assignmentRepository.GetById(assignmentId);
        }
        public async Task Update(Assignment assignment)
        {
            await _assignmentRepository.Update(assignment);
        }
        public async Task Delete(long assignmentId)
        {
            await _assignmentRepository.Delete(assignmentId);
        }
    }
}
