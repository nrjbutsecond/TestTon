using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace TON.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly INotificationService _notificationService;

        public NotificationHub(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId > 0)
            {
                // Join user's personal group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");

                // Send current unread count
                var unreadCount = await _notificationService.GetUnreadCountAsync(userId);
                await Clients.Caller.SendAsync("ReceiveUnreadCount", unreadCount);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (userId > 0)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Client calls this to mark notification as read
        /// </summary>
        public async Task MarkAsRead(int notificationId)
        {
            var userId = GetUserId();
            await _notificationService.MarkAsReadAsync(notificationId, userId);
            // Service sẽ tự động gửi unread count update
        }

        /// <summary>
        /// Client calls this to mark all notifications as read
        /// </summary>
        public async Task MarkAllAsRead()
        {
            var userId = GetUserId();
            await _notificationService.MarkAllAsReadAsync(userId);
            // Service sẽ tự động gửi unread count update
        }

        /// <summary>
        /// Client calls this to get latest unread count
        /// </summary>
        public async Task GetUnreadCount()
        {
            var userId = GetUserId();
            var unreadCount = await _notificationService.GetUnreadCountAsync(userId);
            await Clients.Caller.SendAsync("ReceiveUnreadCount", unreadCount);
        }

        private int GetUserId()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }
    }
}