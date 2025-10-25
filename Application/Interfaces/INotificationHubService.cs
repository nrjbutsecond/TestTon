using Application.DTOs.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface INotificationHubService
    {
        Task SendNotificationToUserAsync(int userId, NotificationDto notification);
        Task SendNotificationToUsersAsync(List<int> userIds, NotificationDto notification);
        Task BroadcastNotificationAsync(NotificationDto notification);
        Task SendUnreadCountToUserAsync(int userId, UnreadCountDto unreadCount);
    }
}
