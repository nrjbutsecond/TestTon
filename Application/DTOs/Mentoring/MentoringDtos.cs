using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Mentoring
{
    // List view for calendar
    public class MentoringSessionListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public DateTime SessionEndDate { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; } = string.Empty;
        public string SessionType { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public string? MeetingLink { get; set; }
        public string? Location { get; set; }

        // Participants info
        public string? MenteeName { get; set; }
        public int CurrentParticipants { get; set; }
        public int MaxParticipants { get; set; }

        // Calendar display
        public string BackgroundColor { get; set; } = "#3B82F6";  // Default blue
        public bool AllDay { get; set; } = false;
    }

    // Detailed view
    public class MentoringSessionDetailDto
    {
        public int Id { get; set; }
        public int MentorId { get; set; }
        public string MentorName { get; set; } = string.Empty;
        public int? MenteeId { get; set; }
        public string? MenteeName { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SessionType { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public DateTime SessionEndDate { get; set; }
        public int Duration { get; set; }

        public string? MeetingLink { get; set; }
        public string? MeetingPassword { get; set; }
        public string? Location { get; set; }
        public bool IsOnline { get; set; }

        public string? Topic { get; set; }
        public string? SessionNotes { get; set; }
        public List<string>? ActionItems { get; set; }
        public string? MenteeProgress { get; set; }
        public string? PrepMaterials { get; set; }

        public string Status { get; set; } = string.Empty;
        public int? Rating { get; set; }
        public string? MenteeFeedback { get; set; }

        public DateTime? NextSessionDate { get; set; }
        public int? NextSessionId { get; set; }

        public int MaxParticipants { get; set; }
        public int CurrentParticipants { get; set; }
        public List<SessionParticipantDto>? Participants { get; set; }
        public List<SessionAttachmentDto>? Attachments { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class SessionParticipantDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserEmail { get; set; }
        public bool HasJoined { get; set; }
        public DateTime? JoinedAt { get; set; }
        public int? Rating { get; set; }
        public string? Feedback { get; set; }
    }

    public class SessionAttachmentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string? FileType { get; set; }
        public long FileSize { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Create/Update DTOs
    public class CreateMentoringSessionDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required]
        public string SessionType { get; set; } = "OneOnOne";

        [Required]
        public DateTime SessionDate { get; set; }

        [Required]
        [Range(15, 480)]  // 15 minutes to 8 hours
        public int Duration { get; set; }

        public int? MenteeId { get; set; }
        public List<int>? ParticipantIds { get; set; }

        [Url]
        public string? MeetingLink { get; set; }

        [StringLength(100)]
        public string? MeetingPassword { get; set; }

        [StringLength(500)]
        public string? Location { get; set; }

        public bool IsOnline { get; set; } = true;

        [StringLength(200)]
        public string? Topic { get; set; }

        public string? PrepMaterials { get; set; }

        [Range(1, 50)]
        public int MaxParticipants { get; set; } = 1;

        public int? ConsultationRequestId { get; set; }
    }

    public class UpdateMentoringSessionDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required]
        public DateTime SessionDate { get; set; }

        [Required]
        [Range(15, 480)]
        public int Duration { get; set; }

        [Url]
        public string? MeetingLink { get; set; }

        [StringLength(100)]
        public string? MeetingPassword { get; set; }

        [StringLength(500)]
        public string? Location { get; set; }

        public bool IsOnline { get; set; }

        [StringLength(200)]
        public string? Topic { get; set; }

        public string? PrepMaterials { get; set; }

        public string? SessionNotes { get; set; }
        public List<string>? ActionItems { get; set; }
        public string? MenteeProgress { get; set; }
    }

    public class CompleteMentoringSessionDto
    {
        [Required]
        public string SessionNotes { get; set; } = string.Empty;

        public List<string>? ActionItems { get; set; }
        public string? MenteeProgress { get; set; }
        public DateTime? NextSessionDate { get; set; }
    }

    public class CancelMentoringSessionDto
    {
        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
    }

    public class RateMentoringSessionDto
    {
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Feedback { get; set; }
    }

    // Mentor Availability DTOs
    public class MentorAvailabilityDto
    {
        public int Id { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public bool IsRecurring { get; set; }
        public DateTime? RecurringEndDate { get; set; }
        public DateTime? SpecificDate { get; set; }
    }

    public class CreateAvailabilityDto
    {
        [Required]
        public string DayOfWeek { get; set; } = string.Empty;

        [Required]
        public string StartTime { get; set; } = string.Empty;  // Format: "HH:mm"

        [Required]
        public string EndTime { get; set; } = string.Empty;

        public bool IsRecurring { get; set; } = true;
        public DateTime? RecurringEndDate { get; set; }
        public DateTime? SpecificDate { get; set; }
    }

    public class MentorBlockedTimeDto
    {
        public int Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? Reason { get; set; }
        public bool IsAllDay { get; set; }
    }

    public class CreateBlockedTimeDto
    {
        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public DateTime EndDateTime { get; set; }

        [StringLength(200)]
        public string? Reason { get; set; }

        public bool IsAllDay { get; set; } = false;
    }

    // Calendar view DTOs
    public class CalendarEventDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Type { get; set; } = "session";  // session, blocked, availability
        public string Status { get; set; } = string.Empty;
        public string? Color { get; set; }
        public bool AllDay { get; set; }
        public object? ExtendedProps { get; set; }  // Additional data for calendar
    }

    public class AvailableTimeSlotsDto
    {
        public DateTime Date { get; set; }
        public List<TimeSlotDto> TimeSlots { get; set; } = new();
    }

    public class TimeSlotDto
    {
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public string? Reason { get; set; }  // Why not available
    }

    // Query parameters
    public class MentoringSessionQueryDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public string? SessionType { get; set; }
        public int? MentorId { get; set; }
        public int? MenteeId { get; set; }
        public bool IncludeCompleted { get; set; } = false;
    }

    public class MentorStatisticsDto
    {
        public int TotalSessions { get; set; }
        public int CompletedSessions { get; set; }
        public int UpcomingSessions { get; set; }
        public double AverageRating { get; set; }
        public int TotalMentees { get; set; }
    }
}