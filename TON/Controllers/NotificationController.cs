using Application.DTOs.Notification;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TON.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        /// <summary>
        /// Get paginated notifications for current user
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] NotificationFilterDto filter)
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.GetUserNotificationsAsync(userId, filter);
            return Ok(result);
        }

        /// <summary>
        /// Get unread notifications
        /// </summary>
        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadNotifications()
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.GetUnreadNotificationsAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Get unread notification count
        /// </summary>
        [HttpGet("unread/count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Get notification statistics
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.GetNotificationStatisticsAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Get notification by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotificationById(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.GetNotificationByIdAsync(id, userId);

            if (result == null)
                return NotFound(new { message = "Notification not found" });

            return Ok(result);
        }

        /// <summary>
        /// Mark notification as read
        /// </summary>
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.MarkAsReadAsync(id, userId);

            if (!result)
                return NotFound(new { message = "Notification not found or already read" });

            return Ok(new { message = "Notification marked as read" });
        }

        /// <summary>
        /// Mark multiple notifications as read
        /// </summary>
        [HttpPut("read/multiple")]
        public async Task<IActionResult> MarkMultipleAsRead([FromBody] MarkAsReadDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.MarkMultipleAsReadAsync(dto, userId);

            if (!result)
                return BadRequest(new { message = "No notifications were marked as read" });

            return Ok(new { message = "Notifications marked as read" });
        }

        /// <summary>
        /// Mark all notifications as read
        /// </summary>
        [HttpPut("read/all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.MarkAllAsReadAsync(userId);

            if (!result)
                return BadRequest(new { message = "No unread notifications found" });

            return Ok(new { message = "All notifications marked as read" });
        }

        /// <summary>
        /// Delete notification
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.DeleteNotificationAsync(id, userId);

            if (!result)
                return NotFound(new { message = "Notification not found" });

            return Ok(new { message = "Notification deleted successfully" });
        }

        /// <summary>
        /// Delete old notifications (older than specified days)
        /// </summary>
        [HttpDelete("cleanup")]
        public async Task<IActionResult> CleanupOldNotifications([FromQuery] int daysToKeep = 30)
        {
            var userId = GetCurrentUserId();
            var deletedCount = await _notificationService.DeleteOldNotificationsAsync(userId, daysToKeep);

            return Ok(new
            {
                message = "Old notifications cleaned up",
                deletedCount
            });
        }

        /// <summary>
        /// Admin only: Send system notification to all users
        /// </summary>
        [HttpPost("system/broadcast")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BroadcastSystemNotification([FromBody] BroadcastNotificationDto dto)
        {
            await _notificationService.SendSystemNotificationToAllAsync(
                dto.Type,
                dto.Title,
                dto.Message
            );

            return Ok(new { message = "System notification broadcast successfully" });
        }

        /// <summary>
        /// Admin only: Cleanup expired notifications
        /// </summary>
        [HttpPost("cleanup/expired")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CleanupExpiredNotifications()
        {
            var deletedCount = await _notificationService.CleanupExpiredNotificationsAsync();

            return Ok(new
            {
                message = "Expired notifications cleaned up",
                deletedCount
            });
        }
    }

    // DTO for broadcasting
    public class BroadcastNotificationDto
    {
        public Domain.Entities.NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}

