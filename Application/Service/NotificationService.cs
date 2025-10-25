using Application.DTOs.Notification;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;
        private readonly INotificationHubService _hubService;

        public NotificationService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<NotificationService> logger,
            INotificationHubService hubService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _hubService = hubService;
        }

        #region Create Notifications

        public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto)
        {
            try
            {
                var notification = _mapper.Map<Notification>(dto);
                notification.CreatedAt = DateTime.UtcNow;

                var created = await _unitOfWork.Notifications.AddAsync(notification);
                await _unitOfWork.SaveChangesAsync();

                var notificationDto = _mapper.Map<NotificationDto>(created);

                // Gửi real-time qua abstraction
                await _hubService.SendNotificationToUserAsync(dto.UserId, notificationDto);

                // Update unread count
                var unreadCount = await GetUnreadCountAsync(dto.UserId);
                await _hubService.SendUnreadCountToUserAsync(dto.UserId, unreadCount);

                _logger.LogInformation("Notification created and sent to user {UserId}: {Title}",
                    dto.UserId, dto.Title);

                return notificationDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification for user {UserId}", dto.UserId);
                throw;
            }
        }

        public async Task<List<NotificationDto>> CreateBulkNotificationsAsync(BulkCreateNotificationDto dto)
        {
            try
            {
                var notifications = new List<Notification>();

                foreach (var userId in dto.UserIds)
                {
                    var notification = _mapper.Map<Notification>(dto);
                    notification.UserId = userId;
                    notification.CreatedAt = DateTime.UtcNow;
                    notifications.Add(notification);
                }

                var created = await _unitOfWork.Notifications.AddRangeAsync(notifications);
                await _unitOfWork.SaveChangesAsync();

                var notificationDtos = _mapper.Map<List<NotificationDto>>(created);

                // Gửi bulk qua abstraction
                await _hubService.SendNotificationToUsersAsync(dto.UserIds, notificationDtos[0]);

                // Update unread count cho từng user
                foreach (var userId in dto.UserIds)
                {
                    var unreadCount = await GetUnreadCountAsync(userId);
                    await _hubService.SendUnreadCountToUserAsync(userId, unreadCount);
                }

                _logger.LogInformation("Bulk notification created for {Count} users: {Title}",
                    dto.UserIds.Count, dto.Title);

                return notificationDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bulk notifications");
                throw;
            }
        }

        #endregion

        #region Read Notifications

        public async Task<NotificationDto?> GetNotificationByIdAsync(int id, int userId)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(id);

            if (notification == null || notification.UserId != userId || notification.IsDeleted)
                return null;

            return _mapper.Map<NotificationDto>(notification);
        }

        public async Task<PaginatedNotificationsDto> GetUserNotificationsAsync(
            int userId,
            NotificationFilterDto filter)
        {
            var query = _unitOfWork.Notifications.GetQueryable()
                .Where(n => n.UserId == userId && !n.IsDeleted);

            // Apply filters
            if (filter.IsRead.HasValue)
                query = query.Where(n => n.IsRead == filter.IsRead.Value);

            if (filter.Type.HasValue)
                query = query.Where(n => n.Type == filter.Type.Value);

            if (filter.Priority.HasValue)
                query = query.Where(n => n.Priority == filter.Priority.Value);

            if (filter.FromDate.HasValue)
                query = query.Where(n => n.CreatedAt >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(n => n.CreatedAt <= filter.ToDate.Value);

            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

            var notifications = query
                .OrderByDescending(n => n.Priority)
                .ThenByDescending(n => n.CreatedAt)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return new PaginatedNotificationsDto
            {
                Items = _mapper.Map<List<NotificationListDto>>(notifications),
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = totalPages,
                HasNextPage = filter.Page < totalPages,
                HasPreviousPage = filter.Page > 1
            };
        }

        public async Task<List<NotificationListDto>> GetUnreadNotificationsAsync(int userId)
        {
            var notifications = await _unitOfWork.Notifications.GetUnreadByUserIdAsync(userId);
            return _mapper.Map<List<NotificationListDto>>(notifications);
        }

        public async Task<UnreadCountDto> GetUnreadCountAsync(int userId)
        {
            var unreadNotifications = await _unitOfWork.Notifications.GetUnreadByUserIdAsync(userId);

            var unreadByType = unreadNotifications
                .GroupBy(n => n.Type.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            return new UnreadCountDto
            {
                TotalUnread = unreadNotifications.Count(),
                UnreadByType = unreadByType
            };
        }

        public async Task<NotificationStatisticsDto> GetNotificationStatisticsAsync(int userId)
        {
            var notifications = _unitOfWork.Notifications.GetQueryable()
                .Where(n => n.UserId == userId && !n.IsDeleted)
                .ToList();

            return new NotificationStatisticsDto
            {
                TotalNotifications = notifications.Count,
                TotalUnread = notifications.Count(n => !n.IsRead),
                TotalRead = notifications.Count(n => n.IsRead),
                LastNotificationAt = notifications.OrderByDescending(n => n.CreatedAt)
                    .FirstOrDefault()?.CreatedAt,
                LastReadAt = notifications.Where(n => n.ReadAt.HasValue)
                    .OrderByDescending(n => n.ReadAt)
                    .FirstOrDefault()?.ReadAt,
                NotificationsByType = notifications.GroupBy(n => n.Type.ToString())
                    .ToDictionary(g => g.Key, g => g.Count()),
                NotificationsByPriority = notifications.GroupBy(n => n.Priority.ToString())
                    .ToDictionary(g => g.Key, g => g.Count())
            };
        }

        #endregion

        #region Update Notifications

        public async Task<bool> MarkAsReadAsync(int notificationId, int userId)
        {
            try
            {
                var result = await _unitOfWork.Notifications.MarkAsReadAsync(notificationId, userId);
                if (result)
                {
                    await _unitOfWork.SaveChangesAsync();

                    // Update unread count qua abstraction
                    var unreadCount = await GetUnreadCountAsync(userId);
                    await _hubService.SendUnreadCountToUserAsync(userId, unreadCount);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification {Id} as read", notificationId);
                return false;
            }
        }

        public async Task<bool> MarkMultipleAsReadAsync(MarkAsReadDto dto, int userId)
        {
            try
            {
                var result = await _unitOfWork.Notifications
                    .MarkMultipleAsReadAsync(dto.NotificationIds, userId);
                if (result)
                {
                    await _unitOfWork.SaveChangesAsync();

                    // Update unread count via SignalR
                    var unreadCount = await GetUnreadCountAsync(userId);
                    await _hubService.SendUnreadCountToUserAsync(userId, unreadCount);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking multiple notifications as read");
                return false;
            }
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            try
            {
                var result = await _unitOfWork.Notifications.MarkAllAsReadAsync(userId);
                if (result)
                {
                    await _unitOfWork.SaveChangesAsync();

                    // Update unread count via SignalR
                    var unreadCount = await GetUnreadCountAsync(userId);
                    await _hubService.SendUnreadCountToUserAsync(userId, unreadCount);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error marking all notifications as read for user {UserId}", userId);
                return false;
            }
        }

        #endregion

        #region Delete Notifications

        public async Task<bool> DeleteNotificationAsync(int notificationId, int userId)
        {
            try
            {
                var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);

                if (notification == null || notification.UserId != userId)
                    return false;

                notification.IsDeleted = true;
                notification.DeletedAt = DateTime.UtcNow;

                _unitOfWork.Notifications.Update(notification);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification {Id}", notificationId);
                return false;
            }
        }

        public async Task<int> DeleteOldNotificationsAsync(int userId, int daysToKeep = 30)
        {
            return await _unitOfWork.Notifications.DeleteOldNotificationsAsync(userId, daysToKeep);
        }

        public async Task<int> CleanupExpiredNotificationsAsync()
        {
            return await _unitOfWork.Notifications.DeleteExpiredNotificationsAsync();
        }

        #endregion

        #region Specific Notification Creators

        public async Task SendOrderNotificationAsync(
            int userId,
            int orderId,
            NotificationType type,
            string customMessage = "")
        {
            var message = string.IsNullOrEmpty(customMessage)
                ? GetDefaultMessage(type)
                : customMessage;

            await CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = userId,
                Title = "Order Update",
                Message = message,
                Type = type,
                Priority = type == NotificationType.OrderDelivered
                    ? NotificationPriority.High
                    : NotificationPriority.Normal,
                RelatedEntityType = "Order",
                RelatedEntityId = orderId,
                ActionUrl = $"/orders/{orderId}"
            });
        }

        public async Task SendTicketNotificationAsync(
            int userId,
            int ticketId,
            NotificationType type,
            string customMessage = "")
        {
            var message = string.IsNullOrEmpty(customMessage)
                ? GetDefaultMessage(type)
                : customMessage;

            await CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = userId,
                Title = "Ticket Update",
                Message = message,
                Type = type,
                Priority = type == NotificationType.TicketExpiring
                    ? NotificationPriority.High
                    : NotificationPriority.Normal,
                RelatedEntityType = "Ticket",
                RelatedEntityId = ticketId,
                ActionUrl = $"/tickets/{ticketId}"
            });
        }

        public async Task SendEventNotificationAsync(
            int userId,
            int eventId,
            NotificationType type,
            string customMessage = "")
        {
            var message = string.IsNullOrEmpty(customMessage)
                ? GetDefaultMessage(type)
                : customMessage;

            await CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = userId,
                Title = "Event Notification",
                Message = message,
                Type = type,
                Priority = type == NotificationType.EventStartingSoon
                    ? NotificationPriority.Urgent
                    : NotificationPriority.Normal,
                RelatedEntityType = "TalkEvent",
                RelatedEntityId = eventId,
                ActionUrl = $"/events/{eventId}"
            });
        }

        public async Task SendWorkshopNotificationAsync(
            int userId,
            int workshopId,
            NotificationType type,
            string customMessage = "")
        {
            var message = string.IsNullOrEmpty(customMessage)
                ? GetDefaultMessage(type)
                : customMessage;

            await CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = userId,
                Title = "Workshop Notification",
                Message = message,
                Type = type,
                Priority = NotificationPriority.Normal,
                RelatedEntityType = "Workshop",
                RelatedEntityId = workshopId,
                ActionUrl = $"/workshops/{workshopId}"
            });
        }

        public async Task SendPersonnelNotificationAsync(
            int userId,
            int requestId,
            NotificationType type,
            string customMessage = "")
        {
            var message = string.IsNullOrEmpty(customMessage)
                ? GetDefaultMessage(type)
                : customMessage;

            await CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = userId,
                Title = "Personnel Support",
                Message = message,
                Type = type,
                Priority = NotificationPriority.High,
                RelatedEntityType = "PersonnelSupportRequest",
                RelatedEntityId = requestId,
                ActionUrl = $"/personnel-support/{requestId}"
            });
        }

        public async Task SendConsultationNotificationAsync(
            int userId,
            int consultationId,
            NotificationType type,
            string customMessage = "")
        {
            var message = string.IsNullOrEmpty(customMessage)
                ? GetDefaultMessage(type)
                : customMessage;

            await CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = userId,
                Title = "Consultation",
                Message = message,
                Type = type,
                Priority = type == NotificationType.ConsultationReminder
                    ? NotificationPriority.High
                    : NotificationPriority.Normal,
                RelatedEntityType = "ConsultationRequest",
                RelatedEntityId = consultationId,
                ActionUrl = $"/consultations/{consultationId}"
            });
        }

        public async Task SendMentoringNotificationAsync(
            int userId,
            int mentoringId,
            NotificationType type,
            string customMessage = "")
        {
            var message = string.IsNullOrEmpty(customMessage)
                ? GetDefaultMessage(type)
                : customMessage;

            await CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = userId,
                Title = "Mentoring Session",
                Message = message,
                Type = type,
                Priority = type == NotificationType.MentoringSessionReminder
                    ? NotificationPriority.High
                    : NotificationPriority.Normal,
                RelatedEntityType = "MentoringRecord",
                RelatedEntityId = mentoringId,
                ActionUrl = $"/mentoring/{mentoringId}"
            });
        }

        public async Task SendReviewNotificationAsync(
            int userId,
            int reviewId,
            NotificationType type,
            string customMessage = "")
        {
            var message = string.IsNullOrEmpty(customMessage)
                ? GetDefaultMessage(type)
                : customMessage;

            await CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = userId,
                Title = "Review Notification",
                Message = message,
                Type = type,
                Priority = NotificationPriority.Low,
                RelatedEntityType = "Review",
                RelatedEntityId = reviewId,
                ActionUrl = $"/reviews/{reviewId}"
            });
        }

        public async Task SendContractNotificationAsync(
            int userId,
            int contractId,
            NotificationType type,
            string customMessage = "")
        {
            var message = string.IsNullOrEmpty(customMessage)
                ? GetDefaultMessage(type)
                : customMessage;

            await CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = userId,
                Title = "Contract & Subscription",
                Message = message,
                Type = type,
                Priority = type == NotificationType.SubscriptionExpiring
                    ? NotificationPriority.Urgent
                    : NotificationPriority.High,
                RelatedEntityType = "ContractNegotiation",
                RelatedEntityId = contractId,
                ActionUrl = $"/contracts/{contractId}"
            });
        }

        public async Task SendAdNotificationAsync(
            int userId,
            int adId,
            NotificationType type,
            string customMessage = "")
        {
            var message = string.IsNullOrEmpty(customMessage)
                ? GetDefaultMessage(type)
                : customMessage;

            await CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = userId,
                Title = "Advertisement",
                Message = message,
                Type = type,
                Priority = type == NotificationType.AdBudgetLow
                    ? NotificationPriority.High
                    : NotificationPriority.Normal,
                RelatedEntityType = "Advertisement",
                RelatedEntityId = adId,
                ActionUrl = $"/advertisements/{adId}"
            });
        }

        public async Task SendSystemNotificationAsync(
            int userId,
            NotificationType type,
            string title,
            string message)
        {
            await CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                Priority = type == NotificationType.SecurityAlert
                    ? NotificationPriority.Urgent
                    : NotificationPriority.Normal
            });
        }

        public async Task SendSystemNotificationToAllAsync(
            NotificationType type,
            string title,
            string message)
        {
            var users = await _unitOfWork.Users.FindAsync(u => u.IsActive && !u.IsDeleted);
            var userIds = users.Select(u => u.Id).ToList();

            await CreateBulkNotificationsAsync(new BulkCreateNotificationDto
            {
                UserIds = userIds,
                Title = title,
                Message = message,
                Type = type,
                Priority = type == NotificationType.SystemMaintenance
                    ? NotificationPriority.High
                    : NotificationPriority.Normal
            });
        }

        #endregion

        #region Helper Methods

        private string GetDefaultMessage(NotificationType type)
        {
            return type switch
            {
                NotificationType.OrderPlaced => "Your order has been placed successfully!",
                NotificationType.OrderProcessing => "Your order is being processed.",
                NotificationType.OrderShipped => "Your order has been shipped!",
                NotificationType.OrderDelivered => "Your order has been delivered.",
                NotificationType.OrderCancelled => "Your order has been cancelled.",
                NotificationType.OrderRefunded => "Your order has been refunded.",
                NotificationType.TicketPurchased => "Your ticket has been purchased successfully!",
                NotificationType.TicketUsed => "Your ticket has been used.",
                NotificationType.TicketExpiring => "Your ticket is expiring soon!",
                NotificationType.TicketCancelled => "Your ticket has been cancelled.",
                _ => "Notification update"
            };
        }

        #endregion
    }
}