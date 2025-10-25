using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Notification
{
    // Full notification DTO
    public class NotificationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string? RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }
        public string? ActionUrl { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // List view DTO (optimized)
    public class NotificationListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TimeAgo { get; set; } = string.Empty; // "2 hours ago"
    }

    // Create notification DTO
    public class CreateNotificationDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        [Required]
        public NotificationType Type { get; set; }

        public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

        [MaxLength(50)]
        public string? RelatedEntityType { get; set; }

        public int? RelatedEntityId { get; set; }

        [MaxLength(500)]
        public string? ActionUrl { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(2000)]
        public string? Metadata { get; set; }

        public DateTime? ExpiresAt { get; set; }
    }

    // Bulk create notifications DTO
    public class BulkCreateNotificationDto
    {
        [Required]
        public List<int> UserIds { get; set; } = new();

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        [Required]
        public NotificationType Type { get; set; }

        public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

        [MaxLength(50)]
        public string? RelatedEntityType { get; set; }

        public int? RelatedEntityId { get; set; }

        [MaxLength(500)]
        public string? ActionUrl { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public DateTime? ExpiresAt { get; set; }
    }

    // Mark as read DTO
    public class MarkAsReadDto
    {
        [Required]
        public List<int> NotificationIds { get; set; } = new();
    }

    // Unread count DTO
    public class UnreadCountDto
    {
        public int TotalUnread { get; set; }
        public Dictionary<string, int> UnreadByType { get; set; } = new();
    }

    // Notification filter DTO
    public class NotificationFilterDto
    {
        public bool? IsRead { get; set; }
        public NotificationType? Type { get; set; }
        public NotificationPriority? Priority { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    // Paginated response
    public class PaginatedNotificationsDto
    {
        public List<NotificationListDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    // Notification preferences DTO
    public class NotificationPreferencesDto
    {
        public int UserId { get; set; }
        public bool EnablePushNotifications { get; set; } = true;
        public bool EnableEmailNotifications { get; set; } = true;
        public bool EnableSmsNotifications { get; set; } = false;

        // Type-specific preferences
        public bool NotifyOnOrderUpdates { get; set; } = true;
        public bool NotifyOnTicketPurchases { get; set; } = true;
        public bool NotifyOnEventReminders { get; set; } = true;
        public bool NotifyOnPersonnelRequests { get; set; } = true;
        public bool NotifyOnConsultations { get; set; } = true;
        public bool NotifyOnMentoringSessions { get; set; } = true;
        public bool NotifyOnReviews { get; set; } = true;
        public bool NotifyOnContracts { get; set; } = true;
        public bool NotifyOnAds { get; set; } = true;
        public bool NotifyOnSystemAnnouncements { get; set; } = true;

        // Quiet hours
        public TimeSpan? QuietHoursStart { get; set; }
        public TimeSpan? QuietHoursEnd { get; set; }
    }

    // Notification statistics DTO
    public class NotificationStatisticsDto
    {
        public int TotalNotifications { get; set; }
        public int TotalUnread { get; set; }
        public int TotalRead { get; set; }
        public DateTime? LastNotificationAt { get; set; }
        public DateTime? LastReadAt { get; set; }
        public Dictionary<string, int> NotificationsByType { get; set; } = new();
        public Dictionary<string, int> NotificationsByPriority { get; set; } = new();
    }
}