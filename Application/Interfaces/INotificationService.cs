using Application.DTOs.Notification;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface INotificationService
    {
        // Create notifications
        Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto);
        Task<List<NotificationDto>> CreateBulkNotificationsAsync(BulkCreateNotificationDto dto);

        // Read notifications
        Task<NotificationDto?> GetNotificationByIdAsync(int id, int userId);
        Task<PaginatedNotificationsDto> GetUserNotificationsAsync(int userId, NotificationFilterDto filter);
        Task<List<NotificationListDto>> GetUnreadNotificationsAsync(int userId);
        Task<UnreadCountDto> GetUnreadCountAsync(int userId);
        Task<NotificationStatisticsDto> GetNotificationStatisticsAsync(int userId);

        // Update notifications
        Task<bool> MarkAsReadAsync(int notificationId, int userId);
        Task<bool> MarkMultipleAsReadAsync(MarkAsReadDto dto, int userId);
        Task<bool> MarkAllAsReadAsync(int userId);

        // Delete notifications
        Task<bool> DeleteNotificationAsync(int notificationId, int userId);
        Task<int> DeleteOldNotificationsAsync(int userId, int daysToKeep = 30);
        Task<int> CleanupExpiredNotificationsAsync();

        // Specific notification creators (helper methods)
        Task SendOrderNotificationAsync(int userId, int orderId, NotificationType type, string customMessage = "");
        Task SendTicketNotificationAsync(int userId, int ticketId, NotificationType type, string customMessage = "");
        Task SendEventNotificationAsync(int userId, int eventId, NotificationType type, string customMessage = "");
        Task SendWorkshopNotificationAsync(int userId, int workshopId, NotificationType type, string customMessage = "");
        Task SendPersonnelNotificationAsync(int userId, int requestId, NotificationType type, string customMessage = "");
        Task SendConsultationNotificationAsync(int userId, int consultationId, NotificationType type, string customMessage = "");
        Task SendMentoringNotificationAsync(int userId, int mentoringId, NotificationType type, string customMessage = "");
        Task SendReviewNotificationAsync(int userId, int reviewId, NotificationType type, string customMessage = "");
        Task SendContractNotificationAsync(int userId, int contractId, NotificationType type, string customMessage = "");
        Task SendAdNotificationAsync(int userId, int adId, NotificationType type, string customMessage = "");
        Task SendSystemNotificationAsync(int userId, NotificationType type, string title, string message);
        Task SendSystemNotificationToAllAsync(NotificationType type, string title, string message);
    }
}

