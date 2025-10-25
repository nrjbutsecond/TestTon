using Domain.Entities.ServicePlan;
using Domain.Interface.ServicePlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repo.ServicePlan
{
    public class ConsultationRequestRepo : Repo<ConsultationRequest>, IConsultationRequestRepo
    {
        public ConsultationRequestRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ConsultationRequest>> GetByOrganizerIdAsync(int organizerId, bool includeDeleted = false)
        {
            var query = _context.ConsultationRequests
                .Include(c => c.Organizer)
                .Include(c => c.ServicePlan)
                .Include(c => c.AssignedStaff)
                .Where(c => c.OrganizerId == organizerId);

            if (!includeDeleted)
            {
                query = query.Where(c => !c.IsDeleted);
            }

            return await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ConsultationRequest>> GetByStatusAsync(ConsultationStatus status, bool includeDeleted = false)
        {
            var query = _context.ConsultationRequests
                .Include(c => c.Organizer)
                .Include(c => c.ServicePlan)
                .Include(c => c.AssignedStaff)
                .Where(c => c.Status == status);

            if (!includeDeleted)
            {
                query = query.Where(c => !c.IsDeleted);
            }

            return await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ConsultationRequest>> GetByAssignedStaffIdAsync(int staffId, bool includeDeleted = false)
        {
            var query = _context.ConsultationRequests
                .Include(c => c.Organizer)
                .Include(c => c.ServicePlan)
                .Where(c => c.AssignedStaffId == staffId);

            if (!includeDeleted)
            {
                query = query.Where(c => !c.IsDeleted);
            }

            return await query
                .OrderByDescending(c => c.ScheduledDate ?? c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ConsultationRequest>> GetPendingRequestsAsync()
        {
            return await _context.ConsultationRequests
                .Include(c => c.Organizer)
                .Include(c => c.ServicePlan)
                .Where(c => !c.IsDeleted && c.Status == ConsultationStatus.Pending)
                .OrderBy(c => c.PreferredDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ConsultationRequest>> GetUpcomingScheduledAsync(DateTime fromDate)
        {
            return await _context.ConsultationRequests
                .Include(c => c.Organizer)
                .Include(c => c.ServicePlan)
                .Include(c => c.AssignedStaff)
                .Where(c => !c.IsDeleted
                    && c.Status == ConsultationStatus.Scheduled
                    && c.ScheduledDate.HasValue
                    && c.ScheduledDate.Value >= fromDate)
                .OrderBy(c => c.ScheduledDate)
                .ToListAsync();
        }

        public async Task<ConsultationRequest?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.ConsultationRequests
                .Include(c => c.Organizer)
                .Include(c => c.ServicePlan)
                .Include(c => c.AssignedStaff)
              //  .Include(c => c.MentoringRecords)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<bool> HasActiveConsultationAsync(int organizerId, int servicePlanId)
        {
            return await _context.ConsultationRequests
                .AnyAsync(c => !c.IsDeleted
                    && c.OrganizerId == organizerId
                    && c.ServicePlanId == servicePlanId
                    && (c.Status == ConsultationStatus.Pending
                        || c.Status == ConsultationStatus.Scheduled
                        || c.Status == ConsultationStatus.InProgress));
        }

        public async Task<Dictionary<string, int>> GetRequestCountByTypeAsync()
        {
            return await _context.ConsultationRequests
                .Where(c => !c.IsDeleted)
                .GroupBy(c => c.ConsultationType)
                .Select(g => new { Type = g.Key.ToString(), Count = g.Count() })
                .ToDictionaryAsync(x => x.Type, x => x.Count);
        }

        public async Task<int> GetPendingCountAsync()
        {
            return await _context.ConsultationRequests
                .CountAsync(c => !c.IsDeleted && c.Status == ConsultationStatus.Pending);
        }
    }
}

