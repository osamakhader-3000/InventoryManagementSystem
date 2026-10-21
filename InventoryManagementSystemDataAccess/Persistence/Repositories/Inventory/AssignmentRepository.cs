using InventoryManagementSystemDataAccess.Mappers.Inventory;
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

        public async Task<Assignment?> GetById(long id)
        {
            Invent.Assignment? dbAssignment = await AssignmentsWithDetails().FirstOrDefaultAsync(x => x.Id == id);

            if (dbAssignment == null)
                return null;

            return dbAssignment.ToDomainModel();
        }

        public async Task<List<Assignment>> ListAll(Guid? userId, Guid? depId, Guid? locationId, DateTime? purDate, string? model, string? serialNumber, bool? isReturned)
        {
            var query = AssignmentsWithDetails();

            if (userId.HasValue)
                query = query.Where(x => x.AssignedUserId == userId.Value);

            if (depId.HasValue)
                query = query.Where(x => x.AssignedUser.DepartmentId == depId.Value);

            if (locationId.HasValue)
                query = query.Where(x => x.AssignedUser.LocationId == locationId.Value);

            if (purDate.HasValue)
            {
                var purchaseDate = DateTime.SpecifyKind(purDate.Value.Date, DateTimeKind.Utc);
                var nextDay = purchaseDate.AddDays(1);
                query = query.Where(x => x.Device.PurchasedDate >= purchaseDate && x.Device.PurchasedDate < nextDay);
            }

            if (!string.IsNullOrEmpty(model))
                query = query.WhereContainsIgnoreCase(x => x.Device.Model, model);

            if (!string.IsNullOrEmpty(serialNumber))
                query = query.Where(x => x.Device.SerialNumber == serialNumber);

            if (isReturned.HasValue)
            {
                byte returnedFlag = (byte)(isReturned.Value ? 1 : 0);
                query = query.Where(x => x.IsReturned == returnedFlag);
            }

            var dbAssignments = await query.ToListAsync();
            return dbAssignments.ToDomainModelList(x => x.ToDomainModel());
        }

        // Returns false when the assignment no longer exists (e.g. returned, and so deleted, meanwhile)
        public async Task<bool> Update(Assignment assignment)
        {
            Invent.Assignment? dbAssignment = await _context.InventoryAssignments
                .Include(x => x.AssignmentAccessories)
                .FirstOrDefaultAsync(x => x.Id == assignment.Id);

            if (dbAssignment == null)
                return false;

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
            return true;
        }

        // Accessory links are removed explicitly so this doesn't depend on the database FK cascading.
        // Returns false when no row was deleted; a concurrent delete of the same row blocks on its lock and then gets false.
        public async Task<bool> Delete(long id)
        {
            await _context.InventoryAssignmentAccessories
                .Where(x => x.AssignmentId == id)
                .ExecuteDeleteAsync();

            int affectedRows = await _context.InventoryAssignments
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync();
            return affectedRows == 1;
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
