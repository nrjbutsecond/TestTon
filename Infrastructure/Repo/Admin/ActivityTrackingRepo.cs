using Domain.Entities.Admin;
using Domain.Interface.Admin;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;

namespace Infrastructure.Repo.Admin
{
    public class ActivityTrackingRepo : Repo<ActivityLog>, IActivityTrackingRepo
    {
        public ActivityTrackingRepo(AppDbContext context) : base(context) { }

        // ========== Activity Logs ==========
        public async Task<List<ActivityLog>> GetRecentActivitiesAsync(int count)
        {
            return await _context.Set<ActivityLog>()
                .Where(a => !a.IsDeleted)
                .OrderByDescending(a => a.OccurredAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<ActivityLog>> GetActivitiesBySeverityAsync(string severity, int count)
        {
            return await _context.Set<ActivityLog>()
                .Where(a => !a.IsDeleted && a.Severity == severity)
                .OrderByDescending(a => a.OccurredAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<ActivityLog>> GetActivitiesByCategoryAsync(string category, DateTime startDate, DateTime endDate)
        {
            return await _context.Set<ActivityLog>()
                .Where(a => !a.IsDeleted &&
                           a.Category == category &&
                           a.OccurredAt >= startDate &&
                           a.OccurredAt <= endDate)
                .OrderByDescending(a => a.OccurredAt)
                .ToListAsync();
        }

        public async Task LogActivityAsync(string activityType, string description, string severity, string category, string userId = null)
        {
            var log = new ActivityLog
            {
                ActivityType = activityType,
                Description = description,
                Severity = severity,
                Category = category,
                UserId = userId,
                OccurredAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await AddAsync(log);
        }

        // ========== Performance Alerts ==========
        public async Task<List<PerformanceAlert>> GetActiveAlertsAsync()
        {
            return await _context.Set<PerformanceAlert>()
                .Where(a => !a.IsDeleted && !a.IsResolved)
                .OrderByDescending(a => a.TriggeredAt)
                .ToListAsync();
        }

        public async Task<List<PerformanceAlert>> GetAlertsByTypeAsync(string alertType)
        {
            return await _context.Set<PerformanceAlert>()
                .Where(a => !a.IsDeleted && a.AlertType == alertType)
                .OrderByDescending(a => a.TriggeredAt)
                .ToListAsync();
        }

        public async Task<bool> ResolveAlertAsync(int alertId, string resolvedBy)
        {
            var alert = await _context.Set<PerformanceAlert>().FindAsync(alertId);
            if (alert == null || alert.IsDeleted)
                return false;

            alert.IsResolved = true;
            alert.ResolvedAt = DateTime.UtcNow;
            alert.ResolvedBy = resolvedBy;
            alert.UpdatedAt = DateTime.UtcNow;

            _context.Set<PerformanceAlert>().Update(alert);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task CreateAlertAsync(string alertType, string title, string message, string severity, decimal thresholdValue, decimal actualValue)
        {
            var alert = new PerformanceAlert
            {
                AlertType = alertType,
                Title = title,
                Message = message,
                Severity = severity,
                TriggeredAt = DateTime.UtcNow,
                IsResolved = false,
                ThresholdValue = thresholdValue,
                ActualValue = actualValue,
                MetricName = alertType,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Set<PerformanceAlert>().AddAsync(alert);
            await _context.SaveChangesAsync();
        }
    }
}
