using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SupportTicket
{
    // List view DTO
    public class SupportTicketListDto
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Category { get; set; } = null!;
        public List<string> Tags { get; set; } = new();

        // Customer info
        public CustomerInfoDto Customer { get; set; } = null!;

        // Assignment
        public string? AssigneeName { get; set; }
        public string? AssigneeEmail { get; set; }

        // Status
        public string Priority { get; set; } = null!;
        public string Status { get; set; } = null!;

        // Activity
        public DateTime CreatedAt { get; set; }
        public DateTime? LastReplyAt { get; set; }
        public string LastReplyTimeAgo { get; set; } = null!;
        public int MessageCount { get; set; }

        // Satisfaction
        public int? SatisfactionRating { get; set; }
        public bool HasRating => SatisfactionRating.HasValue;
    }

    // Detail view DTO
    public class SupportTicketDetailDto
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public List<string> Tags { get; set; } = new();

        // Customer
        public CustomerInfoDto Customer { get; set; } = null!;

        // Assignment
        public int? AssignedToId { get; set; }
        public string? AssigneeName { get; set; }

        // Status
        public string Priority { get; set; } = null!;
        public string Status { get; set; } = null!;

        // Related Order
        public int? RelatedOrderId { get; set; }
        public string? RelatedOrderNumber { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime? FirstResponseAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? LastReplyAt { get; set; }

        // Metrics
        public int MessageCount { get; set; }
        public TimeSpan? ResponseTime { get; set; }
        public TimeSpan? ResolutionTime { get; set; }

        // Satisfaction
        public int? SatisfactionRating { get; set; }
        public string? SatisfactionComment { get; set; }

        // Internal
        public string? InternalNotes { get; set; }

        // Messages
        public List<TicketMessageDto> Messages { get; set; } = new();
    }

    // Create DTO
    public class CreateSupportTicketDto
    {
        public string Subject { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string? Priority { get; set; }
        public List<string>? Tags { get; set; }
        public int? RelatedOrderId { get; set; }
    }

    // Update DTO
    public class UpdateSupportTicketDto
    {
        public string? Subject { get; set; }
        public string? Priority { get; set; }
        public string? Status { get; set; }
        public string? Category { get; set; }
        public List<string>? Tags { get; set; }
        public int? AssignedToId { get; set; }
        public string? InternalNotes { get; set; }
    }

    // Message DTOs
    public class TicketMessageDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = null!;
        public string SenderEmail { get; set; } = null!;
        public bool IsStaff { get; set; }
        public string Content { get; set; } = null!;
        public List<string> Attachments { get; set; } = new();
        public bool IsInternal { get; set; }
        public DateTime SentAt { get; set; }
        public string TimeAgo { get; set; } = null!;
    }

    public class AddTicketMessageDto
    {
        public string Content { get; set; } = null!;
        public List<string>? Attachments { get; set; }
        public bool IsInternal { get; set; } = false;
    }

    // Satisfaction rating DTO
    public class RateTicketDto
    {
        public int Rating { get; set; } // 1-5
        public string? Comment { get; set; }
    }

    // Customer info
    public class CustomerInfoDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? OrderNumber { get; set; }
    }

    // Statistics DTOs
    public class TicketStatisticsDto
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int EscalatedTickets { get; set; }

        public double AverageResponseTimeHours { get; set; }
        public double AverageResolutionTimeHours { get; set; }
        public double SatisfactionScore { get; set; }
        public double ResolutionRate { get; set; }

        public int TicketsToday { get; set; }
        public int TicketsThisWeek { get; set; }
        public int TicketsThisMonth { get; set; }

        public string TrendIndicator { get; set; } = "stable"; // up, down, stable
        public double TrendPercentage { get; set; }
    }

    public class TeamPerformanceDto
    {
        public int UserId { get; set; }
        public string StaffName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public int AssignedTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public double AverageResponseTimeHours { get; set; }
        public double SatisfactionScore { get; set; }
        public string PerformanceIndicator { get; set; } = "good"; // excellent, good, average, poor
    }

    // Filter DTO
    public class TicketFilterDto
    {
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public string? Category { get; set; }
        public int? AssignedToId { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}