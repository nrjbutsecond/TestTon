using Application.DTOs.Ticket;
using Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helper
{
    public partial class MappingProfile
    {
        private void ConfigureTicketTypeMappings()
        {
            CreateMap<TicketTypeModel, TicketTypeDto>()
               .ForMember(dest => dest.Benefits, opt => opt.MapFrom(src =>
                   string.IsNullOrEmpty(src.Benefits)
                       ? null
                       : JsonConvert.DeserializeObject<List<string>>(src.Benefits)))
               .ForMember(dest => dest.EventTitle, opt => opt.MapFrom(src =>
                   src.TalkEvent != null ? src.TalkEvent.Title :
                   src.Workshop != null ? src.Workshop.Title : null))
               .ForMember(dest => dest.EventType, opt => opt.MapFrom(src =>
                   src.TalkEventId.HasValue ? "TalkEvent" : "Workshop"))
               .ForMember(dest => dest.EventStartDate, opt => opt.MapFrom(src =>
                   src.TalkEvent != null ? src.TalkEvent.StartDate :
                   src.Workshop != null ? src.Workshop.StartDateTime : (DateTime?)null))
               .ForMember(dest => dest.EventEndDate, opt => opt.MapFrom(src =>
                   src.TalkEvent != null ? src.TalkEvent.EndDate :
                   src.Workshop != null ? src.Workshop.EndDateTime : (DateTime?)null))
               .ForMember(dest => dest.EventLocation, opt => opt.MapFrom(src =>
                   src.TalkEvent != null ? src.TalkEvent.Location :
                   src.Workshop != null ? src.Workshop.Location : null));

            // TicketTypeModel -> TicketTypeListDto (Simplified for lists)
            CreateMap<TicketTypeModel, TicketTypeListDto>()
                .ForMember(dest => dest.EventTitle, opt => opt.MapFrom(src =>
                    src.TalkEvent != null ? src.TalkEvent.Title :
                    src.Workshop != null ? src.Workshop.Title : null))
                .ForMember(dest => dest.EventType, opt => opt.MapFrom(src =>
                    src.TalkEventId.HasValue ? "TalkEvent" : "Workshop"));

            // CreateTicketTypeForTalkEventDto -> TicketTypeModel
            CreateMap<CreateTicketTypeForTalkEventDto, TicketTypeModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TalkEventId, opt => opt.MapFrom(src => src.TalkEventId))
                .ForMember(dest => dest.WorkshopId, opt => opt.MapFrom(src => (int?)null))
                .ForMember(dest => dest.Benefits, opt => opt.MapFrom(src =>
                    src.Benefits != null && src.Benefits.Count > 0
                        ? JsonConvert.SerializeObject(src.Benefits)
                        : null))
                .ForMember(dest => dest.SoldQuantity, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.ReasonDelete, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.TalkEvent, opt => opt.Ignore())
                .ForMember(dest => dest.Workshop, opt => opt.Ignore())
                .ForMember(dest => dest.Tickets, opt => opt.Ignore());

            // CreateTicketTypeForWorkshopDto -> TicketTypeModel
            CreateMap<CreateTicketTypeForWorkshopDto, TicketTypeModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TalkEventId, opt => opt.MapFrom(src => (int?)null))
                .ForMember(dest => dest.WorkshopId, opt => opt.MapFrom(src => src.WorkshopId))
                .ForMember(dest => dest.Benefits, opt => opt.MapFrom(src =>
                    src.Benefits != null && src.Benefits.Count > 0
                        ? JsonConvert.SerializeObject(src.Benefits)
                        : null))
                .ForMember(dest => dest.SoldQuantity, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.ReasonDelete, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.TalkEvent, opt => opt.Ignore())
                .ForMember(dest => dest.Workshop, opt => opt.Ignore())
                .ForMember(dest => dest.Tickets, opt => opt.Ignore());

            // UpdateTicketTypeDto -> TicketTypeModel (For partial updates)
            CreateMap<UpdateTicketTypeDto, TicketTypeModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TalkEventId, opt => opt.Ignore()) // Don't change event association
                .ForMember(dest => dest.WorkshopId, opt => opt.Ignore()) // Don't change event association
                .ForMember(dest => dest.Benefits, opt => opt.MapFrom(src =>
                    src.Benefits != null && src.Benefits.Count > 0
                        ? JsonConvert.SerializeObject(src.Benefits)
                        : null))
                .ForMember(dest => dest.SoldQuantity, opt => opt.Ignore()) // Don't change via update
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.ReasonDelete, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.TalkEvent, opt => opt.Ignore())
                .ForMember(dest => dest.Workshop, opt => opt.Ignore())
                .ForMember(dest => dest.Tickets, opt => opt.Ignore());
        }
    }
}
