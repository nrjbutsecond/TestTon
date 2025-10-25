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
    public class WorkshopRepo : Repo<WorkshopModel>, IWorkshopRepo
    {
        public WorkshopRepo(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<WorkshopModel>> GetUpcomingWorkshopsAsync(int pageNumber, int pageSize)
        {
            return await _dbSet
                .Where(w => w.StartDateTime > DateTime.UtcNow &&
                       w.Status == WorkshopStatus.InProgress && !w.IsDeleted)
                .OrderBy(w => w.StartDateTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(w => w.Organizer)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkshopModel>> GetOfficialWorkshopsAsync()
        {
            return await _dbSet
                .Where(w => w.IsOfficial && w.Status == WorkshopStatus.Published && !w.IsDeleted)
                .OrderBy(w => w.StartDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkshopModel>> GetWorkshopsByOrganizerAsync(int organizerId)
        {
            return await _dbSet
                .Where(w => w.OrganizerId == organizerId && !w.IsDeleted)
                .OrderByDescending(w => w.CreatedAt)
                .Include(w => w.Organizer)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkshopModel>> GetAvailableWorkshopsAsync()
        {
            return await _dbSet
                .Where(w => w.CurrentParticipants < w.MaxParticipants &&
                       w.RegistrationDeadline > DateTime.UtcNow &&
                       w.Status == WorkshopStatus.Published && !w.IsDeleted)
                .OrderBy(w => w.StartDateTime)
                .Include(w => w.Organizer)
                .ToListAsync();
        }

        public async Task<bool> IsUserRegisteredAsync(int workshopId, int userId)
        {
            return await _context.Tickets
                .AnyAsync(t => t.Id == workshopId &&
                      //    t. == userId && //model ticket undone
                          t.Status != TicketStatus.Cancelled);
        }

        public async Task<int> GetRegisteredCountAsync(int workshopId)
        {
            return await _context.Tickets
                .CountAsync(t => t.Id == workshopId &&
                            t.Status != TicketStatus.Cancelled);
        }

        public async Task UpdateParticipantCountAsync(int workshopId, int count)
        {
            var workshop = await _dbSet.FindAsync(workshopId);
            if (workshop != null)
            {
                workshop.CurrentParticipants = count;
                await _context.SaveChangesAsync();
            }
        }
    }
}