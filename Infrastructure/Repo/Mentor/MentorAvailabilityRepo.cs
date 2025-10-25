using Domain.Entities.Mentor;
using Domain.Interface.Mentor;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;

namespace Infrastructure.Repo.Mentor
{
    public class MentorAvailabilityRepo : Repo<MentorAvailability>, IMentorAvailabilityRepo
    {
        public MentorAvailabilityRepo(AppDbContext context) : base(context) { }
        public async Task<IEnumerable<MentorAvailability>> GetByMentorIdAsync(int mentorId)
        {
            return await _context.MentorAvailabilities
                .Where(a => a.MentorId == mentorId && !a.IsDeleted)
                .OrderBy(a => a.DayOfWeek)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<MentorAvailability?> GetByIdAndMentorAsync(int availabilityId, int mentorId)
        {
            return await _context.MentorAvailabilities
                .FirstOrDefaultAsync(a => a.Id == availabilityId
                    && a.MentorId == mentorId
                    && !a.IsDeleted);
        }

        public async Task<bool> HasOverlapAsync(
            int mentorId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime, int? excludeId = null)
        {
            var query = _context.MentorAvailabilities
                .Where(a => a.MentorId == mentorId
                    && a.DayOfWeek == dayOfWeek
                    && !a.IsDeleted
                    && ((a.StartTime <= startTime && a.EndTime > startTime)
                        || (a.StartTime < endTime && a.EndTime >= endTime)
                        || (a.StartTime >= startTime && a.EndTime <= endTime)));

            if (excludeId.HasValue)
            {
                query = query.Where(a => a.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}