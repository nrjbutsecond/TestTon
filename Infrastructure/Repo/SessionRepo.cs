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
    public class SessionRepo : Repo<UserSession>, ISessionRepo
    {
        public SessionRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<UserSession?> GetActiveSessionByUserIdAsync(int userId)
        {
            return await _context.Set<UserSession>()
                .Where(s => s.UserId == userId && s.IsActive && !s.IsDeleted)
                .OrderByDescending(s => s.LastPingAt)
                .FirstOrDefaultAsync();
        }

        public async Task<List<UserSession>> GetActiveSessionsByUserIdsAsync(List<int> userIds)
        {
            return await _context.Set<UserSession>()
                .Where(s => userIds.Contains(s.UserId) && s.IsActive && !s.IsDeleted)
                .ToListAsync();
        }

        public async Task<int> GetOnlineUsersCountAsync(int thresholdMinutes = 5)
        {
            var cutoffTime = DateTime.UtcNow.AddMinutes(-thresholdMinutes);

            return await _context.Set<UserSession>()
                .Where(s => s.IsActive
                    && !s.IsDeleted
                    && s.LastPingAt >= cutoffTime)
                .Select(s => s.UserId)
                .Distinct()
                .CountAsync();
        }

        public async Task<List<UserSession>> GetExpiredSessionsAsync(int thresholdMinutes = 5)
        {
            var cutoffTime = DateTime.UtcNow.AddMinutes(-thresholdMinutes);

            return await _context.Set<UserSession>()
                .Where(s => s.IsActive
                    && !s.IsDeleted
                    && s.LastPingAt < cutoffTime)
                .ToListAsync();
        }

        public async Task EndAllUserSessionsAsync(int userId)
        {
            var sessions = await _context.Set<UserSession>()
                .Where(s => s.UserId == userId && s.IsActive && !s.IsDeleted)
                .ToListAsync();

            foreach (var session in sessions)
            {
                session.IsActive = false;
                session.EndedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}
