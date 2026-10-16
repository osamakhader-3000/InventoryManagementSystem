using InventoryManagementSystemApplication.inventory.Interfaces;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }
        public async Task<Guid> Create(Department department)
        {
            await _departmentRepository.Create(department);
            return department.Id;
        }
        public async Task<List<Department>> List(string? name)
        {
            return await _departmentRepository.ListAll(name);
        }
        public async Task<Department?> Get(Guid departmentId)
        {
            return await _departmentRepository.GetById(departmentId);
        }
        public async Task Update(Department department)
        {
            await _departmentRepository.Update(department);
        }
        public async Task Delete(Guid departmentId)
        {
            await _departmentRepository.Delete(departmentId);
        }
    }
}
