using Application.DTOs.UserLogTracking;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helper
{
    public partial class MappingProfile
    {
        private void ConfigureActivityMappings()
        {
            // UserActivityLog -> ActivityLogDto
            CreateMap<UserActivityLog, ActivityLogDto>()
                .ForMember(dest => dest.TimeAgo, opt => opt.MapFrom(src =>
                    TimeHelper.GetRelativeTime(src.Timestamp)));

            // UserSession -> UserSessionDto
            CreateMap<UserSession, UserSessionDto>()
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src =>
                    CalculateSessionDuration(src.StartedAt, src.EndedAt ?? DateTime.UtcNow)));

            // UserModel -> UserActivityStatusDto
            CreateMap<UserModel, UserActivityStatusDto>()
                .ForMember(dest => dest.JoinedDate, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.JoinedFormatted, opt => opt.MapFrom(src =>
                    $"Joined {TimeHelper.FormatDateTime(src.CreatedAt, "dd/MM/yyyy")}")) // ✅ FIX: Thêm explicit parameter
                .ForMember(dest => dest.LastActivityAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastActivityFormatted, opt => opt.Ignore())
                .ForMember(dest => dest.IsOnline, opt => opt.Ignore())
                .ForMember(dest => dest.OnlineStatus, opt => opt.Ignore());
        }

        private static string CalculateSessionDuration(DateTime start, DateTime end)
        {
            var duration = end - start;

            if (duration.TotalMinutes < 60)
                return $"{(int)duration.TotalMinutes}m";

            if (duration.TotalHours < 24)
                return $"{(int)duration.TotalHours}h {duration.Minutes}m";

            return $"{(int)duration.TotalDays}d {duration.Hours}h";
        }
    }
}
