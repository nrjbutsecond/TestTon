using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Application.Helper;
using Ticket.Domain.Interface;
using Ticket.Infrastructure.Data;
using Infrastructure.Repo;

namespace Ticket.Infrastructure.Repo
{
    public class TicketRepo : Repo<TicketModel>, ITicketRepo
    {
        public TicketRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<TicketModel?> GetByGuidAsync(Guid guid)
        {
            return await _context.Set<TicketModel>()
                .Include(t => t.TicketType)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Guid == guid && !t.IsDeleted);
        }

        public async Task<TicketModel?> GetByQRCodeAsync(string qrCode)
        {
            return await _context.Set<TicketModel>()
                .Include(t => t.TicketType)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.QRCode == qrCode && !t.IsDeleted);
        }

        public async Task<IEnumerable<TicketModel>> GetTicketsByUserIdAsync(int userId)
        {
            return await _context.Set<TicketModel>()
                .Where(t => t.UserId == userId && !t.IsDeleted)
                .Include(t => t.TicketType)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> IsQRCodeUniqueAsync(string qrCode)
        {
            return !await _context.Set<TicketModel>()
                .AnyAsync(t => t.QRCode == qrCode && !t.IsDeleted);
        }

        public async Task<IEnumerable<TicketModel>> GetExpiredTicketsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Set<TicketModel>()
                .Where(t => t.ValidUntil < now &&
                           (t.Status == TicketStatus.Reserved || t.Status == TicketStatus.Paid) &&
                           !t.IsDeleted)
                .ToListAsync();
        }
    }
}