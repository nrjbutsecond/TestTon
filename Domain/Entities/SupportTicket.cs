using Domain.common;
using Domain.Entities.MerchandiseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SupportTicket : BaseEntity
    {
        public string TicketNumber { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Description { get; set; } = null!;

        // Customer
        public int CustomerId { get; set; }
        public UserModel Customer { get; set; } = null!;

        // Assignment
        public int? AssignedToId { get; set; }
        public UserModel? AssignedTo { get; set; }

        // Classification
        public TicketPriority Priority { get; set; } = TicketPriority.Medium;
        public SupportTicketStatus Status { get; set; } = SupportTicketStatus.Open;
        public TicketCategory Category { get; set; }

        // Tags (stored as comma-separated string)
        public string? Tags { get; set; }

        // Order reference (if ticket is related to an order)
        public int? RelatedOrderId { get; set; }
        public OrderModel? RelatedOrder { get; set; }

        // Metrics
        public int MessageCount { get; set; } = 0;
        public DateTime? FirstResponseAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? LastReplyAt { get; set; }

        // Satisfaction
        public int? SatisfactionRating { get; set; } // 1-5
        public string? SatisfactionComment { get; set; }

        // Internal notes (not visible to customer)
        public string? InternalNotes { get; set; }

        // Navigation
        public ICollection<SupportTicketMessage> Messages { get; set; } = new List<SupportTicketMessage>();
    }
    public enum TicketPriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Urgent = 4
    }

    public enum SupportTicketStatus
    {
        Open = 1,
        InProgress = 2,
        Pending = 3,
        Resolved = 4,
        Escalated = 5,
        Closed = 6
    }

    public enum TicketCategory
    {
        Payment = 1,
        Technical = 2,
        Billing = 3,
        General = 4,
        FeatureRequest = 5,
        OrderIssue = 6,
        Other = 7
    }

}