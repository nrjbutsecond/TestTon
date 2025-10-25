using Application.DTOs.TalkEvent;
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
    private void ConfigureTalkEventMappings()
        {
            // CreateTalkEventDto -> TalkEventModel
            CreateMap<CreateTalkEventDto, TalkEventModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrganizerId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TalkEventStatus.Draft))
                .ForMember(dest => dest.CancellationReason, opt => opt.Ignore())
                .ForMember(dest => dest.Organizer, opt => opt.Ignore())
                .ForMember(dest => dest.TicketTypes, opt => opt.Ignore())
                .ForMember(dest => dest.Tickets, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

            // UpdateTalkEventDto -> TalkEventModel
            CreateMap<UpdateTalkEventDto, TalkEventModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrganizerId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CancellationReason, opt => opt.Ignore())
                .ForMember(dest => dest.Organizer, opt => opt.Ignore())
                .ForMember(dest => dest.TicketTypes, opt => opt.Ignore())
                .ForMember(dest => dest.Tickets, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

            // TalkEventModel -> TalkEventResponseDto
            CreateMap<TalkEventModel, TalkEventResponseDto>()
                .ForMember(dest => dest.OrganizerName,
                    opt => opt.MapFrom(src => src.Organizer != null ? src.Organizer.FullName : string.Empty))
                .ForMember(dest => dest.IsPartneredOrganizer,
                    opt => opt.MapFrom(src => src.Organizer != null && src.Organizer.IsPartneredOrganizer))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CurrentAttendees,
                    opt => opt.MapFrom(src => src.Tickets.Count(t => t.Status == TicketStatus.Paid)));

            // TalkEventModel -> TalkEventListDto
            CreateMap<TalkEventModel, TalkEventListDto>()
                .ForMember(dest => dest.OrganizerName,
                    opt => opt.MapFrom(src => src.Organizer != null ? src.Organizer.FullName : string.Empty))
                .ForMember(dest => dest.IsPartneredOrganizer,
                    opt => opt.MapFrom(src => src.Organizer != null && src.Organizer.IsPartneredOrganizer))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CurrentAttendees,
                    opt => opt.MapFrom(src => src.Tickets.Count(t => t.Status == TicketStatus.Paid)));

            CreateMap<TalkEventModel, DeletedTalkEventDto>()
                    .ForMember(dest => dest.OrganizerName,
                        opt => opt.MapFrom(src => src.Organizer != null ? src.Organizer.FullName : string.Empty))
                    .ForMember(dest => dest.IsPartneredOrganizer,
                        opt => opt.MapFrom(src => src.Organizer != null && src.Organizer.IsPartneredOrganizer))
                    .ForMember(dest => dest.Status,
                        opt => opt.MapFrom(src => src.Status.ToString()))
                    .ForMember(dest => dest.CurrentAttendees,
                        opt => opt.MapFrom(src => src.Tickets.Count(t => t.Status == TicketStatus.Paid)))
                    .ForMember(dest => dest.DeletedBy,
                        opt => opt.MapFrom(src => src.UpdatedBy)) // Assuming UpdatedBy contains who deleted it
                    .ForMember(dest => dest.CanRestore, opt => opt.Ignore())
                    .ForMember(dest => dest.RestoreBlockReason, opt => opt.Ignore())
                    .ForMember(dest => dest.TicketsSold, opt => opt.Ignore())
                    .ForMember(dest => dest.TotalRevenue, opt => opt.Ignore());




        }

}
}
