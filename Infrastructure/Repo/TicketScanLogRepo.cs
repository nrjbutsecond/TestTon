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
    public class TicketScanLogRepo : Repo<TicketScanLogModel>, ITicketScanLogRepo
    {
        public TicketScanLogRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<TicketScanLogModel?> GetLastScanAsync(int ticketId)
        {
            return await _context.Set<TicketScanLogModel>()
                .Where(log => log.TicketId == ticketId && !log.IsDeleted)
                .OrderByDescending(log => log.ScannedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TicketScanLogModel>> GetTicketScansAsync(int ticketId)
        {
            return await _context.Set<TicketScanLogModel>()
                .Where(log => log.TicketId == ticketId && !log.IsDeleted)
                .OrderByDescending(log => log.ScannedAt)
                .ToListAsync();
        }
    }
}