using Application.DTOs.Mentoring;
using Domain.Entities.Mentor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Helper
{
    public partial class MappingProfile
    {
        private void ConfigureMentoringMappings()
        {
            // MentoringRecord mappings
            CreateMap<MentoringRecord, MentoringSessionListDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.SessionType, opt => opt.MapFrom(src => src.SessionType.ToString()))
                .ForMember(dest => dest.MenteeName, opt => opt.MapFrom(src => src.Mentee != null ? src.Mentee.FullName : null))
                .ForMember(dest => dest.BackgroundColor, opt => opt.MapFrom(src => GetStatusColor(src.Status)))
                .ForMember(dest => dest.AllDay, opt => opt.MapFrom(src => false));

            CreateMap<MentoringRecord, MentoringSessionDetailDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.SessionType, opt => opt.MapFrom(src => src.SessionType.ToString()))
                .ForMember(dest => dest.MentorName, opt => opt.MapFrom(src => src.Mentor.FullName))
                .ForMember(dest => dest.MenteeName, opt => opt.MapFrom(src => src.Mentee != null ? src.Mentee.FullName : null))
                .ForMember(dest => dest.ActionItems, opt => opt.MapFrom(src => DeserializeActionItems(src.ActionItems)))
                .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants))
                .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments));

            CreateMap<CreateMentoringSessionDto, MentoringRecord>()
                .ForMember(dest => dest.SessionType, opt => opt.MapFrom(src => Enum.Parse<SessionType>(src.SessionType)))
                .ForMember(dest => dest.SessionEndDate, opt => opt.MapFrom(src => src.SessionDate.AddMinutes(src.Duration)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MentoringSessionStatus.Scheduled))
                .ForMember(dest => dest.CurrentParticipants, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.ActionItems, opt => opt.Ignore())
                .ForMember(dest => dest.Participants, opt => opt.Ignore())
                .ForMember(dest => dest.Attachments, opt => opt.Ignore());

            CreateMap<UpdateMentoringSessionDto, MentoringRecord>()
                .ForMember(dest => dest.SessionEndDate, opt => opt.MapFrom(src => src.SessionDate.AddMinutes(src.Duration)))
                .ForMember(dest => dest.ActionItems, opt => opt.MapFrom(src => SerializeActionItems(src.ActionItems)))
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.MentorId, opt => opt.Ignore())
                .ForMember(dest => dest.MenteeId, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentParticipants, opt => opt.Ignore());

            // Participant mappings
            CreateMap<MentoringSessionParticipant, SessionParticipantDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email));

            // Attachment mappings
            CreateMap<MentoringSessionAttachment, SessionAttachmentDto>();

            // Calendar event mapping
            CreateMap<MentoringRecord, CalendarEventDto>()
                .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.SessionDate))
                .ForMember(dest => dest.End, opt => opt.MapFrom(src => src.SessionEndDate))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "session"))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => GetStatusColor(src.Status)))
                .ForMember(dest => dest.AllDay, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.ExtendedProps, opt => opt.MapFrom(src => new
                {
                    sessionType = src.SessionType.ToString(),
                    mentorId = src.MentorId,
                    menteeId = src.MenteeId,
                    isOnline = src.IsOnline,
                    meetingLink = src.MeetingLink,
                    currentParticipants = src.CurrentParticipants,
                    maxParticipants = src.MaxParticipants
                }));

            // Availability mappings
            CreateMap<MentorAvailability, MentorAvailabilityDto>()
                .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.DayOfWeek.ToString()))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ToString(@"hh\:mm")))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToString(@"hh\:mm")));

            CreateMap<CreateAvailabilityDto, MentorAvailability>()
                .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => Enum.Parse<DayOfWeek>(src.DayOfWeek)))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => TimeSpan.Parse(src.StartTime)))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => TimeSpan.Parse(src.EndTime)));

            // Blocked time mappings
            CreateMap<MentorBlockedTime, MentorBlockedTimeDto>();
            CreateMap<CreateBlockedTimeDto, MentorBlockedTime>();

            CreateMap<MentorBlockedTime, CalendarEventDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Reason ?? "Busy"))
                .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.StartDateTime))
                .ForMember(dest => dest.End, opt => opt.MapFrom(src => src.EndDateTime))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "blocked"))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => "#EF4444"))
                .ForMember(dest => dest.AllDay, opt => opt.MapFrom(src => src.IsAllDay))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "blocked"));
        }

        private static string GetStatusColor(MentoringSessionStatus status)
        {
            return status switch
            {
                MentoringSessionStatus.Scheduled => "#3B82F6",    // Blue
                MentoringSessionStatus.InProgress => "#10B981",   // Green
                MentoringSessionStatus.Completed => "#6B7280",    // Gray
                MentoringSessionStatus.Cancelled => "#EF4444",    // Red
                MentoringSessionStatus.NoShow => "#F59E0B",       // Orange
                _ => "#3B82F6"
            };
        }

        private static List<string>? DeserializeActionItems(string? json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            try
            {
                return JsonSerializer.Deserialize<List<string>>(json);
            }
            catch
            {
                return null;
            }
        }

        private static string? SerializeActionItems(List<string>? actionItems)
        {
            if (actionItems == null || actionItems.Count == 0)
                return null;

            return JsonSerializer.Serialize(actionItems);
        }
    }
}