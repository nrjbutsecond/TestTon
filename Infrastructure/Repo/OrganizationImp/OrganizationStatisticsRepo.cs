using Domain.Entities;
using Domain.Entities.Organize;
using Domain.Interface.OrganizationRepoFolder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;

namespace Infrastructure.Repo.OrganizationImp
{
    public class OrganizationStatisticsRepo :Repo<OrganizationStatistics>, IOrganizationStatisticsRepo
    {
        public OrganizationStatisticsRepo(AppDbContext context) : base(context) { }

        public async Task<OrganizationStatistics> GetStatisticsByPeriodAsync(int organizationId, int year, int month)
        {
            return await _context.OrganizationStatistics
                .FirstOrDefaultAsync(s => s.OrganizationId == organizationId
                    && s.Year == year
                    && s.Month == month
                    && !s.IsDeleted);
        }

        public async Task<IEnumerable<OrganizationStatistics>> GetYearlyStatisticsAsync(int organizationId, int year)
        {
            return await _context.OrganizationStatistics
                .Where(s => s.OrganizationId == organizationId
                    && s.Year == year
                    && !s.IsDeleted)
                .OrderBy(s => s.Month)
                .ToListAsync();
        }

        public async Task UpdateOrCreateStatisticsAsync(int organizationId, int year, int month)
        {
            var stats = await GetStatisticsByPeriodAsync(organizationId, year, month);

            if (stats == null)
            {
                stats = new OrganizationStatistics
                {
                    OrganizationId = organizationId,
                    Year = year,
                    Month = month,
                    CreatedAt = DateTime.UtcNow
                };
                await AddAsync(stats);
            }
            else
            {
                stats.UpdatedAt = DateTime.UtcNow;
            }

            // Calculate statistics for the period
            stats = await CalculateStatisticsForPeriod(stats, organizationId, year, month);

            await _context.SaveChangesAsync();
        }

        public async Task<OrganizationStatistics> CalculateCurrentStatisticsAsync(int organizationId)
        {
            var now = DateTime.UtcNow;
            return await CalculateStatisticsForPeriod(new OrganizationStatistics(), organizationId, now.Year, now.Month);
        }

        private async Task<OrganizationStatistics> CalculateStatisticsForPeriod(OrganizationStatistics stats, int organizationId, int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // Calculate events
            stats.TotalEvents = await _context.TalkEvents
                .CountAsync(e => e.OrganizerId == organizationId
                    && e.StartDate >= startDate
                    && e.StartDate <= endDate
                    && !e.IsDeleted);

            // Calculate attendees
            stats.TotalAttendees = await _context.TalkEvents
                .Where(e => e.OrganizerId == organizationId
                    && e.StartDate >= startDate
                    && e.StartDate <= endDate
                    && !e.IsDeleted)
                .SelectMany(e => e.Tickets)
                .CountAsync(t => t.Status == TicketStatus.Used);

            // Calculate revenue
            stats.Revenue = await _context.Orders
                .Where(o => o.CreatedAt >= startDate
                    && o.CreatedAt <= endDate
                    && !o.IsDeleted)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

            // Calculate new members
            stats.NewMembers = await _context.OrganizationMembers
                .CountAsync(m => m.OrganizationId == organizationId
                    && m.JoinedDate >= startDate
                    && m.JoinedDate <= endDate
                    && !m.IsDeleted);

            return stats;
        }
    }

}

