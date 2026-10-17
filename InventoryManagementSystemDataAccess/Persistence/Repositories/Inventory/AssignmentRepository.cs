using InventoryManagementSystemDataAccess.Mappers.Inventory;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Invent = InventoryManagementSystemDataAccess.Models.Inventory;

namespace InventoryManagementSystemDataAccess.Persistence.Repositories.Inventory
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly SecurityApplicationDataAccessContext _context;
        public AssignmentRepository(SecurityApplicationDataAccessContext context)
        {
            _context = context;
        }

        private IQueryable<Invent.Assignment> AssignmentsWithDetails()
        {
            return _context.InventoryAssignments
                .Include(x => x.Device).ThenInclude(x => x.OperatingSystem)
                .Include(x => x.Device).ThenInclude(x => x.TrialLogs.OrderByDescending(t => t.EventDate).Take(1))
                .Include(x => x.AssignedUser).ThenInclude(x => x.Department)
                .Include(x => x.AssignedUser).ThenInclude(x => x.Location)
                .Include(x => x.AssignmentAccessories).ThenInclude(x => x.Accessory);
        }

        private async Task<List<Guid>> LoadAccessoryIds(Assignment assignment)
        {
            var accessoryIds = assignment.Accessories.Select(x => x.Id).Distinct().ToList();
            return await _context.InventoryAccessories.Where(x => accessoryIds.Contains(x.Id)).Select(x => x.Id).ToListAsync();
        }

        public async Task<long> Create(Assignment assignment)
        {
            var dbAssignment = new Invent.Assignment
            {
                DeviceId = assignment.Device.Id,
                AssignedUserId = assignment.AssignedUser.Id,
                AssignmentDate = assignment.AssignmentDate,
                ReturnDate = assignment.ReturnDate,
                Remarks = assignment.Remarks,
                AssignmentAccessories = (await LoadAccessoryIds(assignment))
                    .Select(id => new Invent.AssignmentAccessory { AccessoryId = id })
                    .ToList()
            };

            _context.InventoryAssignments.Add(dbAssignment);
            await _context.SaveChangesAsync();
            return dbAssignment.Id;
        }

        public async Task Delete(long id)
        {
            Invent.Assignment? dbAssignment = await _context.InventoryAssignments.FirstOrDefaultAsync(x => x.Id == id);

            if (dbAssignment == null)
                return;

            _context.InventoryAssignments.Remove(dbAssignment);
            await _context.SaveChangesAsync();
        }

        public async Task<Assignment?> GetById(long id)
        {
            Invent.Assignment? dbAssignment = await AssignmentsWithDetails().FirstOrDefaultAsync(x => x.Id == id);

            if (dbAssignment == null)
                return null;

            return dbAssignment.ToDomainModel();
        }

        public async Task<List<Assignment>> ListAll(Guid? userId, Guid? depId, DateTime? purDate)
        {
            var query = AssignmentsWithDetails();

            if (userId.HasValue)
                query = query.Where(x => x.AssignedUserId == userId.Value);

            if (depId.HasValue)
                query = query.Where(x => x.AssignedUser.DepartmentId == depId.Value);

            if (purDate.HasValue)
            {
                var purchaseDate = purDate.Value.Date;
                var nextDay = purchaseDate.AddDays(1);
                query = query.Where(x => x.Device.PurchasedDate >= purchaseDate && x.Device.PurchasedDate < nextDay);
            }

            var dbAssignments = await query.ToListAsync();
            return dbAssignments.ToDomainModelList(x => x.ToDomainModel());
        }

        public async Task Update(Assignment assignment)
        {
            Invent.Assignment? dbAssignment = await _context.InventoryAssignments
                .Include(x => x.AssignmentAccessories)
                .FirstOrDefaultAsync(x => x.Id == assignment.Id);

            if (dbAssignment == null)
                return;

            dbAssignment.DeviceId = assignment.Device.Id;
            dbAssignment.AssignedUserId = assignment.AssignedUser.Id;
            dbAssignment.AssignmentDate = assignment.AssignmentDate;
            dbAssignment.ReturnDate = assignment.ReturnDate;
            dbAssignment.Remarks = assignment.Remarks;

            var accessoryIds = await LoadAccessoryIds(assignment);
            dbAssignment.AssignmentAccessories.RemoveAll(x => !accessoryIds.Contains(x.AccessoryId));
            foreach (var accessoryId in accessoryIds.Where(id => !dbAssignment.AssignmentAccessories.Any(x => x.AccessoryId == id)))
            {
                dbAssignment.AssignmentAccessories.Add(new Invent.AssignmentAccessory { AccessoryId = accessoryId });
            }

            await _context.SaveChangesAsync();
        }

        public async Task Return(long id, DateTime returnDate)
        {
            Invent.Assignment? dbAssignment = await _context.InventoryAssignments
                .Include(x => x.AssignmentAccessories)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dbAssignment == null)
                return;

            dbAssignment.IsReturned = 1;
            dbAssignment.ReturnDate = returnDate;
            dbAssignment.AssignmentAccessories.Clear();

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Assignment>> ListByAccessoryId(Guid accessoryId)
        {
            var dbAssignments = await AssignmentsWithDetails()
                .Where(a => a.AssignmentAccessories.Any(ac => ac.AccessoryId == accessoryId))
                .ToListAsync();
            return dbAssignments.ToDomainModelList(x => x.ToDomainModel());
        }

    }
}
