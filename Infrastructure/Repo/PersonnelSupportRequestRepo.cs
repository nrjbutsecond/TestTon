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
    public class PersonnelSupportRequestRepo : Repo<PersonnelSupportRequest>, IPersonnelSupportRequestRepo
    {
        public PersonnelSupportRequestRepo(AppDbContext context) : base(context)
        {

        }

        public async Task<PersonnelSupportRequest?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.PersonnelSupportRequests
                .Include(r => r.Organizer)
                .Include(r => r.Event)
                .Include(r => r.ApprovedByUser)
                .Include(r => r.Assignments)
                    .ThenInclude(a => a.Personnel)
                        .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<IEnumerable<PersonnelSupportRequest>> GetAllWithDetailsAsync(string? status = null)
        {
            var query = _context.PersonnelSupportRequests
                .Include(r => r.Organizer)
                .Include(r => r.Event)
                .Include(r => r.Assignments)
                .Where(r => !r.IsDeleted);

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Status == status);
            }

            return await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonnelSupportRequest>> GetByOrganizerAsync(int organizerId)
        {
            return await _context.PersonnelSupportRequests
                .Include(r => r.Organizer)
                .Include(r => r.Event)
                .Include(r => r.Assignments)
                .Where(r => r.OrganizerId == organizerId && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonnelSupportRequest>> GetByEventAsync(int eventId)
        {
            return await _context.PersonnelSupportRequests
                .Include(r => r.Organizer)
                .Include(r => r.Event)
                .Include(r => r.Assignments)
                .Where(r => r.EventId == eventId && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetStatisticsAsync()
        {
            var requests = await _context.PersonnelSupportRequests
                .Where(r => !r.IsDeleted)
                .ToListAsync();

            return new Dictionary<string, int>
            {
                { "Total", requests.Count },
                { "Pending", requests.Count(r => r.Status == "Pending") },
                { "Approved", requests.Count(r => r.Status == "Approved") },
                { "Assigned", requests.Count(r => r.Status == "Assigned") },
                { "InProgress", requests.Count(r => r.Status == "InProgress") },
                { "Completed", requests.Count(r => r.Status == "Completed") },
                { "Cancelled", requests.Count(r => r.Status == "Cancelled") }
            };
        }
    }
}
  