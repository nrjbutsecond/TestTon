using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface INotificationRepo : IRepo<Notification>
    {
        Task<IEnumerable<Notification>> GetByUserIdAsync(int userId, int page = 1, int pageSize = 20);
        Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> MarkAsReadAsync(int notificationId, int userId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<bool> MarkMultipleAsReadAsync(List<int> notificationIds, int userId);
        Task<int> DeleteExpiredNotificationsAsync();
        Task<int> DeleteOldNotificationsAsync(int userId, int daysToKeep = 30);
        Task<IEnumerable<Notification>> GetByTypeAsync(int userId, NotificationType type, int page = 1, int pageSize = 20);
        Task<Notification?> GetByRelatedEntityAsync(int userId, string entityType, int entityId);
        Task<bool> HasUnreadNotificationsAsync(int userId);
    }
}
