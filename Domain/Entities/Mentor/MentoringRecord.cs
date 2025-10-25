using Domain.common;
using Domain.Entities.ServicePlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Mentor
{
    public class MentoringRecord : BaseEntity
        {
        // Core Information
        public int MentorId { get; set; }
        public int? MenteeId { get; set; }  // Nullable for group sessions
        public int? ConsultationRequestId { get; set; }

        // Session Details
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public SessionType SessionType { get; set; } = SessionType.OneOnOne;
        public DateTime SessionDate { get; set; }
        public DateTime SessionEndDate { get; set; }  // For calendar blocking
        public int Duration { get; set; }  // in minutes

        // Meeting Information
        public string? MeetingLink { get; set; }  // Google Meet, Zoom, etc.
        public string? MeetingPassword { get; set; }
        public string? Location { get; set; }  // For in-person sessions
        public bool IsOnline { get; set; } = true;

        // Content
        public string? Topic { get; set; }
        public string? SessionNotes { get; set; }  // Mentor's notes during session
        public string? ActionItems { get; set; }  // JSON array of action items
        public string? MenteeProgress { get; set; }
        public string? PrepMaterials { get; set; }  // Materials to prepare before session

        // Status & Rating
        public MentoringSessionStatus Status { get; set; } = MentoringSessionStatus.Scheduled;
        public int? Rating { get; set; }  // 1-5, filled by mentee after session
        public string? MenteeFeedback { get; set; }

        // Follow-up
        public DateTime? NextSessionDate { get; set; }
        public int? NextSessionId { get; set; }  // Link to next session in series

        // Reminders
        public bool ReminderSent { get; set; } = false;
        public DateTime? ReminderSentAt { get; set; }

        // Group Session Support
        public int MaxParticipants { get; set; } = 1;
        public int CurrentParticipants { get; set; } = 0;

        // Cancellation
        public string? CancellationReason { get; set; }
        public DateTime? CancelledAt { get; set; }
        public int? CancelledBy { get; set; }  // UserId who cancelled

        // Navigation Properties
        public virtual UserModel Mentor { get; set; } = null!;
        public virtual UserModel? Mentee { get; set; }
        public virtual ConsultationRequest? ConsultationRequest { get; set; }
        public virtual ICollection<MentoringSessionParticipant> Participants { get; set; } = new List<MentoringSessionParticipant>();
        public virtual ICollection<MentoringSessionAttachment> Attachments { get; set; } = new List<MentoringSessionAttachment>();
    }

    public enum MentoringSessionStatus
    {
        Scheduled,      
        InProgress,     
        Completed,      
        Cancelled,      
        NoShow          // Học viên không tham gia ???
    }

    public enum SessionType
    {
        OneOnOne,       // 1-1
        Group,          // Group
        Workshop        // Workshop 
    }
}

