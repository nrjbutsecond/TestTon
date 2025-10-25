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
    public class MentoringSessionRepo : Repo<MentoringRecord>, IMentoringSessionRepo
    {
        public MentoringSessionRepo(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<MentoringRecord>> GetSessionsByMentorAndDateRangeAsync(
           int mentorId, DateTime startDate, DateTime endDate)
        {
            return await _context.MentoringRecords
                .Include(m => m.Mentee)
                .Include(m => m.Participants)
                    .ThenInclude(p => p.User)
                .Include(m => m.Attachments)
                .Where(m => m.MentorId == mentorId
                    && m.SessionDate >= startDate
                    && m.SessionDate <= endDate
                    && !m.IsDeleted)
                .OrderBy(m => m.SessionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MentoringRecord>> GetSessionsByMenteeAndDateRangeAsync(
            int menteeId, DateTime startDate, DateTime endDate)
        {
            return await _context.MentoringRecords
                .Include(m => m.Mentor)
                .Include(m => m.Attachments)
                .Where(m => (m.MenteeId == menteeId || m.Participants.Any(p => p.UserId == menteeId))
                    && m.SessionDate >= startDate
                    && m.SessionDate <= endDate
                    && !m.IsDeleted)
                .OrderBy(m => m.SessionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MentoringRecord>> GetUpcomingSessionsAsync(int userId)
        {
            var now = DateTime.UtcNow;
            return await _context.MentoringRecords
                .Include(m => m.Mentor)
                .Include(m => m.Mentee)
                .Where(m => (m.MentorId == userId || m.MenteeId == userId || m.Participants.Any(p => p.UserId == userId))
                    && m.SessionDate >= now
                    && m.Status == MentoringSessionStatus.Scheduled
                    && !m.IsDeleted)
                .OrderBy(m => m.SessionDate)
                .Take(10)
                .ToListAsync();
        }

        public async Task<MentoringRecord?> GetSessionWithDetailsAsync(int sessionId)
        {
            return await _context.MentoringRecords
                .Include(m => m.Mentor)
                .Include(m => m.Mentee)
                .Include(m => m.Participants)
                    .ThenInclude(p => p.User)
                .Include(m => m.Attachments)
                .Include(m => m.ConsultationRequest)
                .FirstOrDefaultAsync(m => m.Id == sessionId && !m.IsDeleted);
        }

        public async Task<bool> HasConflictingSessionAsync(
            int mentorId, DateTime startTime, DateTime endTime, int? excludeSessionId = null)
        {
            var query = _context.MentoringRecords
                .Where(m => m.MentorId == mentorId
                    && !m.IsDeleted
                    && m.Status != MentoringSessionStatus.Cancelled
                    && m.Status != MentoringSessionStatus.Completed
                    && ((m.SessionDate <= startTime && m.SessionEndDate > startTime)
                        || (m.SessionDate < endTime && m.SessionEndDate >= endTime)
                        || (m.SessionDate >= startTime && m.SessionEndDate <= endTime)));

            if (excludeSessionId.HasValue)
            {
                query = query.Where(m => m.Id != excludeSessionId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<MentoringRecord>> GetMentorSessionsAsync(
            int mentorId, DateTime? startDate = null, DateTime? endDate = null,
            MentoringSessionStatus? status = null)
        {
            var query = _context.MentoringRecords
                .Include(m => m.Mentee)
                .Include(m => m.Participants)
                .Where(m => m.MentorId == mentorId && !m.IsDeleted);

            if (startDate.HasValue)
                query = query.Where(m => m.SessionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(m => m.SessionDate <= endDate.Value);

            if (status.HasValue)
                query = query.Where(m => m.Status == status.Value);

            return await query
                .OrderByDescending(m => m.SessionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MentoringRecord>> GetMenteeSessionsAsync(
            int menteeId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.MentoringRecords
                .Include(m => m.Mentor)
                .Where(m => (m.MenteeId == menteeId || m.Participants.Any(p => p.UserId == menteeId))
                    && !m.IsDeleted);

            if (startDate.HasValue)
                query = query.Where(m => m.SessionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(m => m.SessionDate <= endDate.Value);

            return await query
                .OrderByDescending(m => m.SessionDate)
                .ToListAsync();
        }

        public async Task<int> GetTotalSessionCountAsync(int mentorId)
        {
            return await _context.MentoringRecords
                .CountAsync(m => m.MentorId == mentorId && !m.IsDeleted);
        }

        public async Task<int> GetCompletedSessionCountAsync(int mentorId)
        {
            return await _context.MentoringRecords
                .CountAsync(m => m.MentorId == mentorId
                    && m.Status == MentoringSessionStatus.Completed
                    && !m.IsDeleted);
        }

        public async Task<int> GetUpcomingSessionCountAsync(int mentorId)
        {
            return await _context.MentoringRecords
                .CountAsync(m => m.MentorId == mentorId
                    && m.SessionDate > DateTime.UtcNow
                    && m.Status == MentoringSessionStatus.Scheduled
                    && !m.IsDeleted);
        }

        public async Task<double> GetAverageRatingAsync(int mentorId)
        {
            var ratings = await _context.MentoringRecords
                .Where(m => m.MentorId == mentorId
                    && m.Rating.HasValue
                    && !m.IsDeleted)
                .Select(m => m.Rating!.Value)
                .ToListAsync();

            return ratings.Any() ? ratings.Average() : 0;
        }

        public async Task<int> GetTotalMenteesCountAsync(int mentorId)
        {
            return await _context.MentoringRecords
                .Where(m => m.MentorId == mentorId && !m.IsDeleted && m.MenteeId.HasValue)
                .Select(m => m.MenteeId!.Value)
                .Distinct()
                .CountAsync();
        }
    }
}

