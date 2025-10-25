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
    public class SupportTicketRepo : Repo<SupportTicket>, ISupportTicketRepo
    {
        public SupportTicketRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<SupportTicket?> GetTicketWithDetailsAsync(int id)
        {
            return await _context.SupportTickets
                .Include(t => t.Customer)
                .Include(t => t.AssignedTo)
                .Include(t => t.RelatedOrder)
                .Include(t => t.Messages)
                    .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        }

        public async Task<SupportTicket?> GetTicketByNumberAsync(string ticketNumber)
        {
            return await _context.SupportTickets
                .Include(t => t.Customer)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber && !t.IsDeleted);
        }

        public async Task<IEnumerable<SupportTicket>> GetTicketsByCustomerAsync(int customerId)
        {
            return await _context.SupportTickets
                .Include(t => t.Customer)
                .Include(t => t.AssignedTo)
                .Where(t => t.CustomerId == customerId && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SupportTicket>> GetTicketsByAssigneeAsync(int assigneeId)
        {
            return await _context.SupportTickets
                .Include(t => t.Customer)
                .Include(t => t.AssignedTo)
                .Where(t => t.AssignedToId == assigneeId && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SupportTicket>> GetTicketsByStatusAsync(SupportTicketStatus status)
        {
            return await _context.SupportTickets
                .Include(t => t.Customer)
                .Include(t => t.AssignedTo)
                .Where(t => t.Status == status && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SupportTicket>> GetTicketsByPriorityAsync(TicketPriority priority)
        {
            return await _context.SupportTickets
                .Include(t => t.Customer)
                .Include(t => t.AssignedTo)
                .Where(t => t.Priority == priority && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SupportTicket>> GetOpenTicketsAsync()
        {
            return await _context.SupportTickets
                .Include(t => t.Customer)
                .Include(t => t.AssignedTo)
                .Where(t => t.Status == SupportTicketStatus.Open && !t.IsDeleted)
                .OrderByDescending(t => t.Priority)
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SupportTicket>> GetUnassignedTicketsAsync()
        {
            return await _context.SupportTickets
                .Include(t => t.Customer)
                .Where(t => t.AssignedToId == null &&
                           t.Status != SupportTicketStatus.Closed &&
                           !t.IsDeleted)
                .OrderByDescending(t => t.Priority)
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetOpenTicketCountAsync()
        {
            return await _context.SupportTickets
                .CountAsync(t => t.Status == SupportTicketStatus.Open && !t.IsDeleted);
        }

        public async Task<double> GetAverageResponseTimeAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.SupportTickets
                .Where(t => t.FirstResponseAt != null && !t.IsDeleted);

            if (fromDate.HasValue)
                query = query.Where(t => t.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(t => t.CreatedAt <= toDate.Value);

            var tickets = await query.ToListAsync();

            if (!tickets.Any())
                return 0;

            var totalHours = tickets
                .Where(t => t.FirstResponseAt.HasValue)
                .Select(t => (t.FirstResponseAt!.Value - t.CreatedAt).TotalHours)
                .Sum();

            return totalHours / tickets.Count;
        }

        public async Task<double> GetAverageResolutionTimeAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.SupportTickets
                .Where(t => t.ResolvedAt != null && !t.IsDeleted);

            if (fromDate.HasValue)
                query = query.Where(t => t.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(t => t.CreatedAt <= toDate.Value);

            var tickets = await query.ToListAsync();

            if (!tickets.Any())
                return 0;

            var totalHours = tickets
                .Where(t => t.ResolvedAt.HasValue)
                .Select(t => (t.ResolvedAt!.Value - t.CreatedAt).TotalHours)
                .Sum();

            return totalHours / tickets.Count;
        }

        public async Task<double> GetSatisfactionScoreAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.SupportTickets
                .Where(t => t.SatisfactionRating != null && !t.IsDeleted);

            if (fromDate.HasValue)
                query = query.Where(t => t.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(t => t.CreatedAt <= toDate.Value);

            var ratings = await query
                .Select(t => t.SatisfactionRating!.Value)
                .ToListAsync();

            return ratings.Any() ? ratings.Average() : 0;
        }

        public async Task<Dictionary<SupportTicketStatus, int>> GetTicketCountByStatusAsync()
        {
            return await _context.SupportTickets
                .Where(t => !t.IsDeleted)
                .GroupBy(t => t.Status)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<Dictionary<TicketPriority, int>> GetTicketCountByPriorityAsync()
        {
            return await _context.SupportTickets
                .Where(t => !t.IsDeleted)
                .GroupBy(t => t.Priority)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<IEnumerable<SupportTicket>> SearchTicketsAsync(string searchTerm)
        {
            return await _context.SupportTickets
                .Include(t => t.Customer)
                .Include(t => t.AssignedTo)
                .Where(t => !t.IsDeleted &&
                           (t.TicketNumber.Contains(searchTerm) ||
                            t.Subject.Contains(searchTerm) ||
                            t.Description.Contains(searchTerm) ||
                            t.Customer.FullName.Contains(searchTerm) ||
                            t.Customer.Email.Contains(searchTerm)))
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}