using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Notification : BaseEntity
    {
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

        // Polymorphic reference to related entity
        public string? RelatedEntityType { get; set; } // "Order", "Ticket", "TalkEvent", etc.
        public int? RelatedEntityId { get; set; }

        public string? ActionUrl { get; set; } // Deep link for mobile app
        public string? ImageUrl { get; set; }
        public string? Metadata { get; set; } // JSON for additional data

        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
        public DateTime? ExpiresAt { get; set; }

        // Navigation property
        public virtual UserModel User { get; set; } = null!;
    }
    public enum NotificationType
    {
        // Order notifications
        OrderPlaced = 1,
        OrderProcessing = 2,
        OrderShipped = 3,
        OrderDelivered = 4,
        OrderCancelled = 5,
        OrderRefunded = 6,

        // Ticket notifications
        TicketPurchased = 10,
        TicketUsed = 11,
        TicketExpiring = 12,
        TicketCancelled = 13,

        // Event notifications
        EventCreated = 20,
        EventUpdated = 21,
        EventCancelled = 22,
        EventReminder = 23,
        EventStartingSoon = 24,

        // Workshop notifications
        WorkshopRegistered = 30,
        WorkshopReminder = 31,
        WorkshopCancelled = 32,
        WorkshopCompleted = 33,

        // Personnel Support notifications
        PersonnelRequestReceived = 40,
        PersonnelRequestApproved = 41,
        PersonnelRequestRejected = 42,
        PersonnelAssigned = 43,
        PersonnelRequestCompleted = 44,

        // Consultation notifications
        ConsultationRequested = 50,
        ConsultationScheduled = 51,
        ConsultationReminder = 52,
        ConsultationCompleted = 53,
        ConsultationCancelled = 54,

        // Mentoring notifications
        MentoringSessionScheduled = 60,
        MentoringSessionReminder = 61,
        MentoringSessionCompleted = 62,
        MentoringSessionCancelled = 63,
        MentoringFeedbackReceived = 64,

        // Review notifications
        ReviewReceived = 70,
        ReviewResponseReceived = 71,
        ReviewHelpfulVote = 72,

        // Contract & Subscription notifications
        ContractRequestReceived = 80,
        ContractApproved = 81,
        ContractRejected = 82,
        SubscriptionExpiring = 83,
        SubscriptionExpired = 84,
        SubscriptionRenewed = 85,

        // Advertisement notifications
        AdCreated = 90,
        AdApproved = 91,
        AdRejected = 92,
        AdExpiring = 93,
        AdBudgetLow = 94,

        // Support Ticket notifications
        SupportTicketCreated = 100,
        SupportTicketAssigned = 101,
        SupportTicketUpdated = 102,
        SupportTicketResolved = 103,
        SupportTicketClosed = 104,
        SupportTicketMessageReceived = 105,

        // Partnership notifications
        PartnershipApplicationSubmitted = 110,
        PartnershipApplicationApproved = 111,
        PartnershipApplicationRejected = 112,

        // Organization notifications
        OrganizationMemberAdded = 120,
        OrganizationMemberRemoved = 121,
        OrganizationRoleChanged = 122,

        // System notifications
        SystemAnnouncement = 200,
        SystemMaintenance = 201,
        AccountVerified = 202,
        PasswordChanged = 203,
        SecurityAlert = 204
    }

    public enum NotificationPriority
    {
        Low = 0,
        Normal = 1,
        High = 2,
        Urgent = 3
    }
}

