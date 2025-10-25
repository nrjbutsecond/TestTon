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
    public class OrganizationActivityRepo : Repo<OrganizationActivity>, IOrganizationActivityRepo
    {
        public OrganizationActivityRepo(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<OrganizationActivity>> GetActivitiesByOrganizationAsync(int organizationId, int take = 10)
        {
            return await _context.OrganizationActivities
                .Include(a => a.User)
                .Where(a => a.OrganizationId == organizationId && !a.IsDeleted)
                .OrderByDescending(a => a.ActivityDate)
                .Take(take)
                .ToListAsync();
        }

        public async Task LogActivityAsync(int organizationId, ActivityType type, string description, int? userId = null, string details = null)
        {
            var activity = new OrganizationActivity
            {
                OrganizationId = organizationId,
                UserId = userId,
                Type = type,
                Description = description,
                Details = details,
                ActivityDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await AddAsync(activity);
            await _context.SaveChangesAsync();
        }

        public async Task LogEventActivityAsync(int eventId, int userId, ActivityType type, string description)
        {
            // Find organization through member relationship
            var orgMember = await (
                from m in _context.OrganizationMembers
                where m.UserId == userId && !m.IsDeleted && m.IsActive
                select m
            ).FirstOrDefaultAsync();

            if (orgMember != null)
            {
                await LogActivityAsync(orgMember.OrganizationId, type, description, userId);
            }
        }

        public async Task LogMerchandiseActivityAsync(int merchandiseId, int userId, ActivityType type, string description)
        {
            // Find organization through member relationship
            var orgMember = await (
                from m in _context.OrganizationMembers
                where m.UserId == userId && !m.IsDeleted && m.IsActive
                select m
            ).FirstOrDefaultAsync();

            if (orgMember != null)
            {
                await LogActivityAsync(orgMember.OrganizationId, type, description, userId);
            }
        }
        public async Task<IEnumerable<OrganizationActivity>> GetActivitiesByDateRangeAsync(
           int organizationId, DateTime startDate, DateTime endDate)
        {
            return await _context.OrganizationActivities
                .Include(a => a.User)
                .Where(a => a.OrganizationId == organizationId
                    && a.ActivityDate >= startDate
                    && a.ActivityDate <= endDate
                    && !a.IsDeleted)
                .OrderByDescending(a => a.ActivityDate)
                .ToListAsync();
        }
    }
}
