using Application.DTOs.Ticket;
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
        private void ConfigureTicketMappings()
        {
            // TicketModel -> TicketDto (Full details)
            CreateMap<TicketModel, TicketDto>()
                .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.TicketableId))
                .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => src.TicketableType))
                .ForMember(dest => dest.EventName, opt => opt.MapFrom(src =>
                    src.TicketableType == TicketableTypes.TalkEvent
                        ? (src.TicketType.TalkEvent != null ? src.TicketType.TalkEvent.Title : "")
                        : (src.TicketType.Workshop != null ? src.TicketType.Workshop.Title : "")))
                .ForMember(dest => dest.EventDate, opt => opt.MapFrom(src =>
                    src.TicketableType == TicketableTypes.TalkEvent
                        ? (src.TicketType.TalkEvent != null ? src.TicketType.TalkEvent.StartDate : DateTime.MinValue)
                        : (src.TicketType.Workshop != null ? src.TicketType.Workshop.StartDateTime : DateTime.MinValue)))
                .ForMember(dest => dest.EventLocation, opt => opt.MapFrom(src =>
                    src.TicketableType == TicketableTypes.TalkEvent
                        ? (src.TicketType.TalkEvent != null ? src.TicketType.TalkEvent.Location : "")
                        : (src.TicketType.Workshop != null ? src.TicketType.Workshop.Location : "")))
                .ForMember(dest => dest.TicketTypeName, opt => opt.MapFrom(src =>
                    src.TicketType != null ? src.TicketType.Name : ""))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src =>
                    src.TicketType != null ? src.TicketType.Price : 0))
                .ForMember(dest => dest.Benefits, opt => opt.MapFrom(src =>
                    src.TicketType != null ? src.TicketType.Benefits : ""))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src =>
                    src.User != null ? src.User.FullName : ""))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src =>
                    src.User != null ? src.User.Email : ""));

            // CreateTicketDto -> TicketModel
            CreateMap<CreateTicketDto, TicketModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TicketableId, opt => opt.MapFrom(src => src.EventId))
                .ForMember(dest => dest.TicketableType, opt => opt.MapFrom(src =>
                    Enum.Parse<TicketableTypes>(src.EventType)))
                .ForMember(dest => dest.Guid, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.QRCode, opt => opt.Ignore()) // Generated in service
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TicketStatus.Reserved))
                .ForMember(dest => dest.PurchaseDate, opt => opt.Ignore())
                .ForMember(dest => dest.ValidFrom, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.ValidUntil, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.ReasonDelete, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.TicketType, opt => opt.Ignore());

            // ReserveTicketDto -> TicketModel
            CreateMap<ReserveTicketDto, TicketModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TicketableId, opt => opt.Ignore()) // Get from TicketType
                .ForMember(dest => dest.TicketableType, opt => opt.Ignore()) // Get from TicketType
                .ForMember(dest => dest.Guid, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.QRCode, opt => opt.Ignore()) // Generated in service
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TicketStatus.Reserved))
                .ForMember(dest => dest.PurchaseDate, opt => opt.Ignore())
                .ForMember(dest => dest.ValidFrom, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.ValidUntil, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.ReasonDelete, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.TicketType, opt => opt.Ignore());
        }
    }
}