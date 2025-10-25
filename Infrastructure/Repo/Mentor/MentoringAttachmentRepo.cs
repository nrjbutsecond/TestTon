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
    public class MentoringAttachmentRepo : Repo<MentoringSessionAttachment>, IMentoringAttachmentRepo
    {
        public MentoringAttachmentRepo(AppDbContext context) : base(context) { }
        public async Task<IEnumerable<MentoringSessionAttachment>> GetBySessionIdAsync(int sessionId)
        {
            return await _context.MentoringSessionAttachments
                .Where(a => a.MentoringRecordId == sessionId && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<MentoringSessionAttachment?> GetByIdWithSessionAsync(int attachmentId)
        {
            return await _context.MentoringSessionAttachments
                .Include(a => a.MentoringRecord)
                .FirstOrDefaultAsync(a => a.Id == attachmentId && !a.IsDeleted);
        }
    }
}

