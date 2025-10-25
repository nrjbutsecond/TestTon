using Application.DTOs.Mentoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMentoringService
    {
        // Session CRUD
        Task<MentoringSessionDetailDto> CreateSessionAsync(int mentorId, CreateMentoringSessionDto dto);
        Task<MentoringSessionDetailDto> UpdateSessionAsync(int sessionId, int mentorId, UpdateMentoringSessionDto dto);
        Task<bool> DeleteSessionAsync(int sessionId, int mentorId);
        Task<MentoringSessionDetailDto?> GetSessionByIdAsync(int sessionId);

        // Calendar views
        Task<IEnumerable<CalendarEventDto>> GetMentorCalendarAsync(
            int mentorId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<CalendarEventDto>> GetMenteeCalendarAsync(
            int menteeId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<MentoringSessionListDto>> GetUpcomingSessionsAsync(int userId);

        // Session management
        Task<bool> StartSessionAsync(int sessionId, int mentorId);
        Task<bool> CompleteSessionAsync(int sessionId, int mentorId, CompleteMentoringSessionDto dto);
        Task<bool> CancelSessionAsync(int sessionId, int userId, CancelMentoringSessionDto dto);
        Task<bool> RateSessionAsync(int sessionId, int menteeId, RateMentoringSessionDto dto);

        // Participant management
        Task<bool> AddParticipantAsync(int sessionId, int mentorId, int userId);
        Task<bool> RemoveParticipantAsync(int sessionId, int mentorId, int userId);
        Task<bool> JoinSessionAsync(int sessionId, int userId);

        // Availability management
        Task<IEnumerable<MentorAvailabilityDto>> GetMentorAvailabilityAsync(int mentorId);
        Task<MentorAvailabilityDto> AddAvailabilityAsync(int mentorId, CreateAvailabilityDto dto);
        Task<bool> UpdateAvailabilityAsync(int availabilityId, int mentorId, CreateAvailabilityDto dto);
        Task<bool> DeleteAvailabilityAsync(int availabilityId, int mentorId);

        // Blocked time management
        Task<IEnumerable<MentorBlockedTimeDto>> GetBlockedTimesAsync(
            int mentorId, DateTime startDate, DateTime endDate);
        Task<MentorBlockedTimeDto> AddBlockedTimeAsync(int mentorId, CreateBlockedTimeDto dto);
        Task<bool> UpdateBlockedTimeAsync(int blockedTimeId, int mentorId, CreateBlockedTimeDto dto);
        Task<bool> DeleteBlockedTimeAsync(int blockedTimeId, int mentorId);

        // Available time slots
        Task<IEnumerable<AvailableTimeSlotsDto>> GetAvailableTimeSlotsAsync(
            int mentorId, DateTime startDate, int daysCount, int sessionDuration);

        // Statistics
        Task<MentorStatisticsDto> GetMentorStatisticsAsync(int mentorId);

        // Attachment management
        Task<bool> AddAttachmentAsync(int sessionId, int userId, string fileName, string fileUrl, long fileSize);
        Task<bool> RemoveAttachmentAsync(int attachmentId, int userId);
    }
}