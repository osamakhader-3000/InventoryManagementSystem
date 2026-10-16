using InventoryManagementSystemDataAccess.Persistence.Extensions;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Invent = InventoryManagementSystemDataAccess.Models.Inventory;

namespace InventoryManagementSystemDataAccess.Persistence.Repositories.Inventory
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly SecurityApplicationDataAccessContext _context;
        public DepartmentRepository(SecurityApplicationDataAccessContext context)
        {
            _context = context;
        }

        public Task Create(Department department)
        {
            var dbDepartment = new Invent.Department
            {
                Id = department.Id,
                Name = department.Name
            };

            _context.InventoryDepartments.Add(dbDepartment);
            return _context.SaveChangesAsync();
        }

        public Task Delete(Guid id)
        {
            _context.InventoryDepartments.Remove(new Invent.Department { Id = id });
            return _context.SaveChangesAsync();
        }

        public async Task<Department?> GetById(Guid id)
        {
            Invent.Department? dbDepartment = await _context.InventoryDepartments.FindAsync(id);

            if(dbDepartment == null)            
                return null;            

           return Department.Reconstitute(dbDepartment.Id, dbDepartment.Name);
        }

        public async Task<List<Department>> ListAll(string? name)
        {
            var dbDepartments = await _context.InventoryDepartments
                .WhereContainsIgnoreCase(d => d.Name, name)
                .ToListAsync();
            var departments = new List<Department>();
            foreach (var dbDepartment in dbDepartments)
            {
                var department = Department.Reconstitute(dbDepartment.Id, dbDepartment.Name);
                if (department != null)
                {
                    departments.Add(department);
                }
            }
            return departments;

        }

        public Task Update(Department department)
        {
            var dbDepartment = new Invent.Department
            {
                Id = department.Id,
                Name = department.Name
            };
            _context.InventoryDepartments.Update(dbDepartment);
            return _context.SaveChangesAsync();

        }
    }
}
