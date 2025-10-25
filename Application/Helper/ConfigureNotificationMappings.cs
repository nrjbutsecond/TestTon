using Application.DTOs.Notification;
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
        private void ConfigureNotificationMappings()
        {
            // Notification to NotificationDto
            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()));

            // Notification to NotificationListDto
            CreateMap<Notification, NotificationListDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
                .ForMember(dest => dest.TimeAgo, opt => opt.MapFrom(src => CalculateTimeAgo(src.CreatedAt)));

            // CreateNotificationDto to Notification
            CreateMap<CreateNotificationDto, Notification>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.ReadAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

            // BulkCreateNotificationDto to Notification (will be used per user)
            CreateMap<BulkCreateNotificationDto, Notification>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Will be set manually
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.ReadAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Metadata, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());
        }

        private static string CalculateTimeAgo(DateTime createdAt)
        {
            var timeSpan = DateTime.UtcNow - createdAt;

            if (timeSpan.TotalSeconds < 60)
                return "Just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes}m ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours}h ago";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays}d ago";
            if (timeSpan.TotalDays < 30)
                return $"{(int)(timeSpan.TotalDays / 7)}w ago";
            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)}mo ago";

            return $"{(int)(timeSpan.TotalDays / 365)}y ago";
        }
    }
}