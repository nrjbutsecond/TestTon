using Domain.Entities;
using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Repo
{
    public class SupportTicketMessageRepo : Repo<SupportTicketMessage>, ISupportTicketMessageRepo
    {
        public SupportTicketMessageRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SupportTicketMessage>> GetMessagesByTicketAsync(int ticketId)
        {
            return await _context.SupportTicketMessages
                .Include(m => m.Sender)
                .Where(m => m.SupportTicketId == ticketId && !m.IsDeleted)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<SupportTicketMessage?> GetLatestMessageAsync(int ticketId)
        {
            return await _context.SupportTicketMessages
                .Include(m => m.Sender)
                .Where(m => m.SupportTicketId == ticketId && !m.IsDeleted)
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetMessageCountAsync(int ticketId)
        {
            return await _context.SupportTicketMessages
                .CountAsync(m => m.SupportTicketId == ticketId && !m.IsDeleted);
        }
    }
}