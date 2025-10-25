using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Mentor;
using Domain.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class NotificationReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationReminderService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Check every hour

        public NotificationReminderService(
            IServiceProvider serviceProvider,
            ILogger<NotificationReminderService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Notification Reminder Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SendEventRemindersAsync();
                    await SendWorkshopRemindersAsync();
                    await SendMentoringRemindersAsync();
                    await SendSubscriptionExpiryRemindersAsync();
                    await SendTicketExpiryRemindersAsync();
                    await CleanupExpiredNotificationsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Notification Reminder Service");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Notification Reminder Service stopped");
        }

        private async Task SendEventRemindersAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            try
            {
                var tomorrow = DateTime.UtcNow.AddDays(1);
                var dayAfterTomorrow = DateTime.UtcNow.AddDays(2);

                // Get events happening in the next 24-48 hours
                var upcomingEvents = await unitOfWork.TalkEvent.FindAsync(e =>
                    e.StartDate >= tomorrow &&
                    e.StartDate <= dayAfterTomorrow &&
                    e.Status == TalkEventStatus.Ongoing &&
                    !e.IsDeleted);

                foreach (var evt in upcomingEvents)
                {
                    // Get all users who have tickets for this event
                    var tickets = await unitOfWork.Tickets.FindAsync(t =>
                        t.TicketableId == evt.Id &&
                        t.TicketableType == TicketableTypes.TalkEvent &&
                        t.Status == TicketStatus.Paid &&
                        !t.IsDeleted);

                    var userIds = tickets.Select(t => t.UserId).Distinct();

                    foreach (var userId in userIds)
                    {
                        // Check if reminder was already sent
                        var existingNotification = await unitOfWork.Notifications
                            .GetByRelatedEntityAsync(userId, "TalkEvent", evt.Id);

                        if (existingNotification?.Type == NotificationType.EventReminder)
                            continue; // Already sent

                        await notificationService.SendEventNotificationAsync(
                            userId,
                            evt.Id,
                            NotificationType.EventReminder,
                            $"Reminder: '{evt.Title}' is happening soon on {evt.StartDate:MMM dd, yyyy} at {evt.StartDate:HH:mm}!"
                        );
                    }
                }

                _logger.LogInformation("Sent event reminders for {Count} events", upcomingEvents.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending event reminders");
            }
        }

        private async Task SendWorkshopRemindersAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            try
            {
                var tomorrow = DateTime.UtcNow.AddDays(1);
                var dayAfterTomorrow = DateTime.UtcNow.AddDays(2);

                var upcomingWorkshops = await unitOfWork.Workshops.FindAsync(w =>
                    w.StartDateTime >= tomorrow &&
                    w.StartDateTime <= dayAfterTomorrow &&
                    w.Status == WorkshopStatus.Published &&
                    !w.IsDeleted);

                foreach (var workshop in upcomingWorkshops)
                {
                    var tickets = await unitOfWork.Tickets.FindAsync(t =>
                        t.TicketableId == workshop.Id &&
                        t.TicketableType == TicketableTypes.Workshop &&
                        t.Status == TicketStatus.Paid &&
                        !t.IsDeleted);

                    var userIds = tickets.Select(t => t.UserId).Distinct();

                    foreach (var userId in userIds)
                    {
                        var existingNotification = await unitOfWork.Notifications
                            .GetByRelatedEntityAsync(userId, "Workshop", workshop.Id);

                        if (existingNotification?.Type == NotificationType.WorkshopReminder)
                            continue;

                        await notificationService.SendWorkshopNotificationAsync(
                            userId,
                            workshop.Id,
                            NotificationType.WorkshopReminder,
                            $"Reminder: Workshop '{workshop.Title}' starts on {workshop.StartDateTime:MMM dd, yyyy} at {workshop.StartDateTime:HH:mm}!"
                        );
                    }
                }

                _logger.LogInformation("Sent workshop reminders for {Count} workshops", upcomingWorkshops.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending workshop reminders");
            }
        }

        private async Task SendMentoringRemindersAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            try
            {
                var in2Hours = DateTime.UtcNow.AddHours(2);
                var in4Hours = DateTime.UtcNow.AddHours(4);

                var upcomingSessions = await unitOfWork.MentoringSessions.GetQueryable()
                    .Where(m =>
                        m.SessionDate >= in2Hours &&
                        m.SessionDate <= in4Hours &&
                        m.Status == MentoringSessionStatus.Scheduled &&
                        !m.IsDeleted &&
                        !m.ReminderSent)
                    .ToListAsync();

                foreach (var session in upcomingSessions)
                {
                    // Notify mentor
                    await notificationService.SendMentoringNotificationAsync(
                        session.MentorId,
                        session.Id,
                        NotificationType.MentoringSessionReminder,
                        $"Reminder: Mentoring session starts in {(session.SessionDate - DateTime.UtcNow).TotalHours:F0} hours!"
                    );

                    // Notify mentee if exists
                    if (session.MenteeId.HasValue)
                    {
                        await notificationService.SendMentoringNotificationAsync(
                            session.MenteeId.Value,
                            session.Id,
                            NotificationType.MentoringSessionReminder,
                            $"Reminder: Mentoring session starts in {(session.SessionDate - DateTime.UtcNow).TotalHours:F0} hours!"
                        );
                    }

                    // Mark reminder as sent
                    session.ReminderSent = true;
                    session.ReminderSentAt = DateTime.UtcNow;
                }

                await unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Sent mentoring reminders for {Count} sessions", upcomingSessions.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending mentoring reminders");
            }
        }

        private async Task SendSubscriptionExpiryRemindersAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            try
            {
                var in7Days = DateTime.UtcNow.AddDays(7);
                var in8Days = DateTime.UtcNow.AddDays(8);

                var expiringSubscriptions = await unitOfWork.UserServicePlanSubscription.GetQueryable()//GetQueryable<UserServicePlanSubscription>()
                    .Where(s =>
                        s.EndDate >= in7Days &&
                        s.EndDate <= in8Days &&
                        s.IsActive &&
                        !s.IsDeleted)
                    .ToListAsync();

                foreach (var subscription in expiringSubscriptions)
                {
                    // Check if reminder was already sent
                    var existingNotification = await unitOfWork.Notifications
                        .GetByRelatedEntityAsync(
                            subscription.UserId,
                            "UserServicePlanSubscription",
                            subscription.Id);

                    if (existingNotification?.Type == NotificationType.SubscriptionExpiring)
                        continue;

                    await notificationService.SendContractNotificationAsync(
                        subscription.UserId,
                        subscription.Id,
                        NotificationType.SubscriptionExpiring,
                        $"Your subscription expires in 7 days on {subscription.EndDate:MMM dd, yyyy}. Renew now to continue enjoying premium features!"
                    );
                }

                _logger.LogInformation("Sent subscription expiry reminders for {Count} subscriptions",
                    expiringSubscriptions.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending subscription expiry reminders");
            }
        }

        private async Task SendTicketExpiryRemindersAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            try
            {
                var tomorrow = DateTime.UtcNow.AddDays(1);
                var dayAfterTomorrow = DateTime.UtcNow.AddDays(2);

                var expiringTickets = await unitOfWork.Tickets.FindAsync(t =>
                    t.ValidUntil >= tomorrow &&
                    t.ValidUntil <= dayAfterTomorrow &&
                    t.Status == TicketStatus.Paid &&
                    !t.IsDeleted);

                foreach (var ticket in expiringTickets)
                {
                    var existingNotification = await unitOfWork.Notifications
                        .GetByRelatedEntityAsync(ticket.UserId, "Ticket", ticket.Id);

                    if (existingNotification?.Type == NotificationType.TicketExpiring)
                        continue;

                    await notificationService.SendTicketNotificationAsync(
                        ticket.UserId,
                        ticket.Id,
                        NotificationType.TicketExpiring,
                        $"Your ticket expires soon on {ticket.ValidUntil:MMM dd, yyyy}. Please use it before expiry!"
                    );
                }

                _logger.LogInformation("Sent ticket expiry reminders for {Count} tickets",
                    expiringTickets.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending ticket expiry reminders");
            }
        }

        private async Task CleanupExpiredNotificationsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            try
            {
                var deletedCount = await notificationService.CleanupExpiredNotificationsAsync();

                if (deletedCount > 0)
                {
                    _logger.LogInformation("Cleaned up {Count} expired notifications", deletedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired notifications");
            }
        }
    }
}
  