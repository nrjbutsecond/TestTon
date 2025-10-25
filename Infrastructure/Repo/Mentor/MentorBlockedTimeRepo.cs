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
    public class MentorBlockedTimeRepo : Repo<MentorBlockedTime>, IMentorBlockedTimeRepo
    {
        public MentorBlockedTimeRepo(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<MentorBlockedTime>> GetByMentorAndDateRangeAsync(
           int mentorId, DateTime startDate, DateTime endDate)
        {
            return await _context.MentorBlockedTimes
                .Where(b => b.MentorId == mentorId
                    && !b.IsDeleted
                    && b.StartDateTime <= endDate
                    && b.EndDateTime >= startDate)
                .OrderBy(b => b.StartDateTime)
                .ToListAsync();
        }

        public async Task<MentorBlockedTime?> GetByIdAndMentorAsync(int blockedTimeId, int mentorId)
        {
            return await _context.MentorBlockedTimes
                .FirstOrDefaultAsync(b => b.Id == blockedTimeId
                    && b.MentorId == mentorId
                    && !b.IsDeleted);
        }

        public async Task<bool> HasOverlapAsync(
            int mentorId, DateTime startTime, DateTime endTime, int? excludeId = null)
        {
            var query = _context.MentorBlockedTimes
                .Where(b => b.MentorId == mentorId
                    && !b.IsDeleted
                    && ((b.StartDateTime <= startTime && b.EndDateTime > startTime)
                        || (b.StartDateTime < endTime && b.EndDateTime >= endTime)
                        || (b.StartDateTime >= startTime && b.EndDateTime <= endTime)));

            if (excludeId.HasValue)
            {
                query = query.Where(b => b.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}

