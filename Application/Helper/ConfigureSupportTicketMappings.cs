using Application.DTOs.SupportTicket;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
namespace Application.Helper
{
    public partial class MappingProfile
    {
        private void ConfigureSupportTicketMappings()
        {
            // SupportTicket -> SupportTicketListDto
            CreateMap<SupportTicket, SupportTicketListDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
                    !string.IsNullOrEmpty(src.Tags)
                        ? src.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                        : new System.Collections.Generic.List<string>()))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => new CustomerInfoDto
                {
                    Id = src.Customer.Id,
                    FullName = src.Customer.FullName,
                    Email = src.Customer.Email,
                    Phone = src.Customer.Phone,
                    OrderNumber = src.RelatedOrder != null ? src.RelatedOrder.OrderNumber : null
                }))
                .ForMember(dest => dest.AssigneeName, opt => opt.MapFrom(src =>
                    src.AssignedTo != null ? src.AssignedTo.FullName : null))
                .ForMember(dest => dest.AssigneeEmail, opt => opt.MapFrom(src =>
                    src.AssignedTo != null ? src.AssignedTo.Email : null))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.LastReplyTimeAgo, opt => opt.MapFrom(src =>
                    src.LastReplyAt.HasValue
                        ? GetTimeAgo(src.LastReplyAt.Value)
                        : "No replies yet"))
                .ForMember(dest => dest.MessageCount, opt => opt.MapFrom(src => src.MessageCount));

            // SupportTicket -> SupportTicketDetailDto
            CreateMap<SupportTicket, SupportTicketDetailDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
                    !string.IsNullOrEmpty(src.Tags)
                        ? src.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                        : new System.Collections.Generic.List<string>()))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => new CustomerInfoDto
                {
                    Id = src.Customer.Id,
                    FullName = src.Customer.FullName,
                    Email = src.Customer.Email,
                    Phone = src.Customer.Phone,
                    OrderNumber = src.RelatedOrder != null ? src.RelatedOrder.OrderNumber : null
                }))
                .ForMember(dest => dest.AssigneeName, opt => opt.MapFrom(src =>
                    src.AssignedTo != null ? src.AssignedTo.FullName : null))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.RelatedOrderNumber, opt => opt.MapFrom(src =>
                    src.RelatedOrder != null ? src.RelatedOrder.OrderNumber : null))
                .ForMember(dest => dest.ResponseTime, opt => opt.MapFrom(src =>
                    src.FirstResponseAt.HasValue
                        ? src.FirstResponseAt.Value - src.CreatedAt
                        : (TimeSpan?)null))
                .ForMember(dest => dest.ResolutionTime, opt => opt.MapFrom(src =>
                    src.ResolvedAt.HasValue
                        ? src.ResolvedAt.Value - src.CreatedAt
                        : (TimeSpan?)null))
                .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages));

            // SupportTicketMessage -> TicketMessageDto
            CreateMap<SupportTicketMessage, TicketMessageDto>()
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.FullName))
                .ForMember(dest => dest.SenderEmail, opt => opt.MapFrom(src => src.Sender.Email))
                .ForMember(dest => dest.IsStaff, opt => opt.MapFrom(src =>
                    src.Sender.Role == UserRoles.Admin ||
                    src.Sender.Role == UserRoles.CommunityStaff ||
                    src.Sender.Role == UserRoles.SalesStaff ||
                    src.Sender.Role == UserRoles.MentoringStaff))
                .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => DeserializeAttachments(src.Attachments)))
                .ForMember(dest => dest.TimeAgo, opt => opt.MapFrom(src => GetTimeAgo(src.SentAt)));
        }

        private List<string>? DeserializeAttachments(string? attachmentsJson)
        {
            if (string.IsNullOrEmpty(attachmentsJson))
                return new List<string>();

            try
            {
                return JsonSerializer.Deserialize<List<string>>(attachmentsJson);
            }
            catch
            {
                return new List<string>();
            }
        }
        private static string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes > 1 ? "s" : "")} ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours > 1 ? "s" : "")} ago";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays > 1 ? "s" : "")} ago";
            if (timeSpan.TotalDays < 30)
                return $"{(int)(timeSpan.TotalDays / 7)} week{((int)(timeSpan.TotalDays / 7) > 1 ? "s" : "")} ago";
            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} month{((int)(timeSpan.TotalDays / 30) > 1 ? "s" : "")} ago";

            return $"{(int)(timeSpan.TotalDays / 365)} year{((int)(timeSpan.TotalDays / 365) > 1 ? "s" : "")} ago";
        }
    }
}