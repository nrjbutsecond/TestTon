using Domain.Entities;
using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repo
{
    public class TalkEventRepo : Repo<TalkEventModel>, ITalkEventRepo
    {
        public TalkEventRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TalkEventModel>> GetUpcomingEventsAsync(int limit = 10)
        {
            return await _context.TalkEvents
                .Where(e => !e.IsDeleted &&
                           e.Status == TalkEventStatus.Published &&
                           e.StartDate > DateTime.UtcNow)
                .OrderBy(e => e.StartDate)
                .Take(limit)
                .Include(e => e.Organizer)
                .ToListAsync();
        }

        public async Task<IEnumerable<TalkEventModel>> GetEventsByOrganizerAsync(int organizerId)
        {
            return await _context.TalkEvents
                .Where(e => !e.IsDeleted && e.OrganizerId == organizerId)
                .OrderByDescending(e => e.StartDate)
                .Include(e => e.Organizer)
                .ToListAsync();
        }

        public async Task<IEnumerable<TalkEventModel>> GetPartneredEventsAsync()
        {
            return await _context.TalkEvents
                .Where(e => !e.IsDeleted &&
                           e.Status == TalkEventStatus.Published &&
                           e.Organizer.IsPartneredOrganizer)
                .OrderByDescending(e => e.StartDate)
                .Include(e => e.Organizer)
                .ToListAsync();
        }

        public async Task<(IEnumerable<TalkEventModel> Items, int TotalCount)> GetPagedEventsAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<TalkEventModel, bool>>? filter = null,
            string? orderBy = null)
        {
            var query = _context.TalkEvents.Where(e => !e.IsDeleted);

            if (filter != null)
                query = query.Where(filter);

            var totalCount = await query.CountAsync();

            query = orderBy?.ToLower() switch
            {
                "date" => query.OrderBy(e => e.StartDate),
                "date_desc" => query.OrderByDescending(e => e.StartDate),
                "title" => query.OrderBy(e => e.Title),
                _ => query.OrderByDescending(e => e.CreatedAt)
            };

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(e => e.Organizer)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<TalkEventModel?> GetEventWithTicketsAsync(int id)
        {
            return await _context.TalkEvents
                .Include(e => e.Organizer)
                .Include(e => e.TicketTypes) // ticket and ticketType haven't done
                .Include(e => e.Tickets)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public async Task<TalkEventModel?> GetDeletedByIdAsync(int id)
        {
            return await _context.TalkEvents
                .IgnoreQueryFilters()
                .Include(e => e.Organizer)
                .Include(e => e.Tickets)
                    .ThenInclude(t => t.TicketType)
                .Include(e => e.TicketTypes)
                .FirstOrDefaultAsync(e => e.Id == id && e.IsDeleted);
        }

        public async Task<IEnumerable<TalkEventModel>> GetDeletedEventsAsync(
            Expression<Func<TalkEventModel, bool>>? filter = null,
            string? orderBy = null)
        {
            // Base query
            var baseQuery = _context.TalkEvents
                .IgnoreQueryFilters()
                .Where(e => e.IsDeleted);

            // Apply filter if exists
            if (filter != null)
                baseQuery = baseQuery.Where(filter);

            // Apply ordering
            baseQuery = orderBy?.ToLower() switch
            {
                "date" => baseQuery.OrderBy(e => e.StartDate),
                "deleted" => baseQuery.OrderByDescending(e => e.DeletedAt),
                "title" => baseQuery.OrderBy(e => e.Title),
                _ => baseQuery.OrderByDescending(e => e.DeletedAt ?? DateTime.MinValue)
            };

            // Apply includes last
            var finalQuery = baseQuery
                .Include(e => e.Organizer)
                .Include(e => e.Tickets);

            return await finalQuery.ToListAsync();
        }

        public async Task<int> CountDeletedEventsAsync(Expression<Func<TalkEventModel, bool>>? filter = null)
        {
            var query = _context.TalkEvents
                .IgnoreQueryFilters()
                .Where(e => e.IsDeleted);

            if (filter != null)
                query = query.Where(filter);

            return await query.CountAsync();
        }

    }
}