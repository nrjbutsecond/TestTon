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
    public class MentoringParticipantRepo : Repo<MentoringSessionParticipant>, IMentoringParticipantRepo
    {
        public MentoringParticipantRepo(AppDbContext context) : base(context) { }

        public async Task<MentoringSessionParticipant?> GetBySessionAndUserAsync(int sessionId, int userId)
        {
            return await _context.MentoringSessionParticipants
                .FirstOrDefaultAsync(p => p.MentoringRecordId == sessionId
                    && p.UserId == userId
                    && !p.IsDeleted);
        }

        public async Task<IEnumerable<MentoringSessionParticipant>> GetBySessionIdAsync(int sessionId)
        {
            return await _context.MentoringSessionParticipants
                .Include(p => p.User)
                .Where(p => p.MentoringRecordId == sessionId && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<int> GetParticipantCountAsync(int sessionId)
        {
            return await _context.MentoringSessionParticipants
                .CountAsync(p => p.MentoringRecordId == sessionId && !p.IsDeleted);
        }
    }
}

