using Domain.Entities;
using Domain.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;

namespace Infrastructure.Repo
{
    public class ActivityLogRepo : Repo<UserActivityLog>, IActivityLogRepo
    {
        public ActivityLogRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<UserActivityLog?> GetLastActivityByUserIdAsync(int userId)
        {
            return await _context.Set<UserActivityLog>()
                .Where(log => log.UserId == userId && !log.IsDeleted)
                .OrderByDescending(log => log.Timestamp)
                .FirstOrDefaultAsync();
        }

        public async Task<List<UserActivityLog>> GetUserActivitiesAsync(int userId, int take = 20)
        {
            return await _context.Set<UserActivityLog>()
                .Where(log => log.UserId == userId && !log.IsDeleted)
                .OrderByDescending(log => log.Timestamp)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<UserActivityLog>> GetUserActivitiesByDateRangeAsync(int userId, DateTime from, DateTime to)
        {
            return await _context.Set<UserActivityLog>()
                .Where(log => log.UserId == userId
                    && !log.IsDeleted
                    && log.Timestamp >= from
                    && log.Timestamp <= to)
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();
        }

        public async Task<int> GetTotalActivitiesCountAsync(int userId)
        {
            return await _context.Set<UserActivityLog>()
                .CountAsync(log => log.UserId == userId && !log.IsDeleted);
        }

        public async Task DeleteOldLogsAsync(DateTime beforeDate)
        {
            var oldLogs = await _context.Set<UserActivityLog>()
                .Where(log => log.Timestamp < beforeDate)
                .ToListAsync();

            foreach (var log in oldLogs)
            {
                log.IsDeleted = true;
                log.DeletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}
