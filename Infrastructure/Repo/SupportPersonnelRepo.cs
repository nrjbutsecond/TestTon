using Domain.Entities;
using Domain.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;

namespace Infrastructure.Repo
{
    public class SupportPersonnelRepo : Repo<SupportPersonnel>, ISupportPersonnelRepo
    {
        public SupportPersonnelRepo(AppDbContext context) : base(context)
        {

        }
        public async Task<SupportPersonnel?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.SupportPersonnel
                .Include(sp => sp.User)
                .Include(sp => sp.RegisteredByUser)
                .Include(sp => sp.Assignments)
                .FirstOrDefaultAsync(sp => sp.Id == id && !sp.IsDeleted);
        }

        public async Task<IEnumerable<SupportPersonnel>> GetAllWithDetailsAsync(bool? isActive = null)
        {
            var query = _context.SupportPersonnel
                .Include(sp => sp.User)
                .Include(sp => sp.Assignments)
                .Where(sp => !sp.IsDeleted);

            if (isActive.HasValue)
            {
                query = query.Where(sp => sp.IsActive == isActive.Value);
            }

            return await query
                .OrderByDescending(sp => sp.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SupportPersonnel>> GetByOrganizerAsync(int organizerId)
        {
            return await _context.SupportPersonnel
                .Include(sp => sp.User)
                .Include(sp => sp.Assignments)
                .Where(sp => sp.RegisteredBy == organizerId && !sp.IsDeleted)
                .OrderByDescending(sp => sp.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SupportPersonnel>> GetAvailablePersonnelAsync(DateTime startDate, DateTime endDate)
        {
            // Get all active personnel
            var allPersonnel = await _context.SupportPersonnel
                .Include(sp => sp.User)
                .Include(sp => sp.Assignments)
                    .ThenInclude(a => a.Request)
                .Where(sp => sp.IsActive && !sp.IsDeleted)
                .ToListAsync();

            // Filter out personnel with conflicting assignments
            var availablePersonnel = allPersonnel.Where(p =>
            {
                var hasConflict = p.Assignments?.Any(a =>
                    !a.IsDeleted &&
                    a.Request != null &&
                    (a.Status == "Assigned" || a.Status == "InProgress") &&
                    a.Request.StartDate < endDate &&
                    a.Request.EndDate > startDate) ?? false;

                return !hasConflict;
            }).ToList();

            return availablePersonnel;
        }

        public async Task<bool> ExistsByUserIdAsync(int userId)
        {
            return await _context.SupportPersonnel
                .AnyAsync(sp => sp.UserId == userId && !sp.IsDeleted);
        }

        public async Task<bool> HasActiveAssignmentsAsync(int personnelId)
        {
            return await _context.PersonnelSupportAssignments
                .AnyAsync(a => a.SupportPersonnelId == personnelId &&
                              a.Status != "Completed" &&
                              a.Status != "Cancelled" &&
                              !a.IsDeleted);
        }
    }
}
    