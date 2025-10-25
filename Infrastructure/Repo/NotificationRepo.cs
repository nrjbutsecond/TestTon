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
    public class NotificationRepo : Repo<Notification>, INotificationRepo
    {
        public NotificationRepo(AppDbContext context) : base(context) { }
      

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId, int page = 1, int pageSize = 20)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsDeleted)
                .OrderByDescending(n => n.Priority)
                .ThenByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted)
                .OrderByDescending(n => n.Priority)
                .ThenByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead && !n.IsDeleted);
        }

        public async Task<bool> MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId && !n.IsDeleted);

            if (notification == null || notification.IsRead)
                return false;

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            notification.UpdatedAt = DateTime.UtcNow;

            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted)
                .ToListAsync();

            if (!unreadNotifications.Any())
                return false;

            var now = DateTime.UtcNow;
            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = now;
                notification.UpdatedAt = now;
            }

            return true;
        }

        public async Task<bool> MarkMultipleAsReadAsync(List<int> notificationIds, int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => notificationIds.Contains(n.Id) && n.UserId == userId && !n.IsRead && !n.IsDeleted)
                .ToListAsync();

            if (!notifications.Any())
                return false;

            var now = DateTime.UtcNow;
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = now;
                notification.UpdatedAt = now;
            }

            return true;
        }

        public async Task<int> DeleteExpiredNotificationsAsync()
        {
            var expiredNotifications = await _context.Notifications
                .Where(n => n.ExpiresAt.HasValue && n.ExpiresAt.Value < DateTime.UtcNow && !n.IsDeleted)
                .ToListAsync();

            if (!expiredNotifications.Any())
                return 0;

            var now = DateTime.UtcNow;
            foreach (var notification in expiredNotifications)
            {
                notification.IsDeleted = true;
                notification.DeletedAt = now;
            }

            await _context.SaveChangesAsync();
            return expiredNotifications.Count;
        }

        public async Task<int> DeleteOldNotificationsAsync(int userId, int daysToKeep = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);

            var oldNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && n.CreatedAt < cutoffDate && !n.IsDeleted)
                .ToListAsync();

            if (!oldNotifications.Any())
                return 0;

            var now = DateTime.UtcNow;
            foreach (var notification in oldNotifications)
            {
                notification.IsDeleted = true;
                notification.DeletedAt = now;
            }

            await _context.SaveChangesAsync();
            return oldNotifications.Count;
        }

        public async Task<IEnumerable<Notification>> GetByTypeAsync(int userId, NotificationType type, int page = 1, int pageSize = 20)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && n.Type == type && !n.IsDeleted)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Notification?> GetByRelatedEntityAsync(int userId, string entityType, int entityId)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(n => n.UserId == userId
                    && n.RelatedEntityType == entityType
                    && n.RelatedEntityId == entityId
                    && !n.IsDeleted);
        }

        public async Task<bool> HasUnreadNotificationsAsync(int userId)
        {
            return await _context.Notifications
                .AnyAsync(n => n.UserId == userId && !n.IsRead && !n.IsDeleted);
        }
    }
}
 
