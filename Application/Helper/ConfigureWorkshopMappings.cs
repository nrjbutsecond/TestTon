using Application.DTOs.Activity;
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
        private void ConfigureWorkshopMappings()
        {
            CreateMap<WorkshopModel, WorkshopDto>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.AvailableSlots,
                    opt => opt.MapFrom(src => src.MaxParticipants - src.CurrentParticipants))
                .ForMember(dest => dest.CanRegister,
                    opt => opt.MapFrom(src => (src.MaxParticipants - src.CurrentParticipants) > 0 &&
                                             src.RegistrationDeadline > DateTime.UtcNow &&
                                             src.Status == WorkshopStatus.Published));

            CreateMap<WorkshopModel, WorkshopListDto>()
                .ForMember(dest => dest.OrganizerName,
                    opt => opt.MapFrom(src => src.IsOfficial ? "Official" :
                        (src.Organizer != null ? src.Organizer.FullName : "Unknown")))
                .ForMember(dest => dest.AvailableSlots,
                    opt => opt.MapFrom(src => src.MaxParticipants - src.CurrentParticipants));

            CreateMap<CreateWorkshopDto, WorkshopModel>()
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentParticipants, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.IsOfficial, opt => opt.MapFrom(src => false));

            CreateMap<UpdateWorkshopDto, WorkshopModel>()
                .ForMember(dest => dest.CurrentParticipants, opt => opt.Ignore())
                .ForMember(dest => dest.IsOfficial, opt => opt.Ignore())
                .ForMember(dest => dest.OrganizerId, opt => opt.Ignore());
        }
    }
}