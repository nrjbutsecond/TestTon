using Application.DTOs.Notification;
using Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using TON.Hubs;

namespace TON.Services
{
    public class SignalRNotificationHubService : INotificationHubService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotificationHubService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationToUserAsync(int userId, NotificationDto notification)
        {
            await _hubContext.Clients
                .Group($"user_{userId}")
                .SendAsync("ReceiveNotification", notification);
        }

        public async Task SendNotificationToUsersAsync(List<int> userIds, NotificationDto notification)
        {
            var groups = userIds.Select(id => $"user_{id}").ToList();
            await _hubContext.Clients
                .Groups(groups)
                .SendAsync("ReceiveNotification", notification);
        }

        public async Task BroadcastNotificationAsync(NotificationDto notification)
        {
            await _hubContext.Clients
                .All
                .SendAsync("ReceiveNotification", notification);
        }

        public async Task SendUnreadCountToUserAsync(int userId, UnreadCountDto unreadCount)
        {
            await _hubContext.Clients
                .Group($"user_{userId}")
                .SendAsync("ReceiveUnreadCount", unreadCount);
        }
    }
}
