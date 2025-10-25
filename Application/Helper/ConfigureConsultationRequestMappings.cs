using Application.DTOs.ConsultationRequest;
using Domain.Entities.ServicePlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helper
{
    public partial class MappingProfile
    {
        private void ConfigureConsultationRequestMappings()
        {
            // Entity to List DTO
            CreateMap<ConsultationRequest, ConsultationRequestListDto>()
                .ForMember(dest => dest.OrganizerName,
                    opt => opt.MapFrom(src => src.Organizer != null ? src.Organizer.FullName : "Unknown"))
                .ForMember(dest => dest.ServicePlanName,
                    opt => opt.MapFrom(src => src.ServicePlan != null ? src.ServicePlan.Name : "Unknown"))
                .ForMember(dest => dest.ConsultationType,
                    opt => opt.MapFrom(src => src.ConsultationType.ToString()))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.AssignedStaffName,
                    opt => opt.MapFrom(src => src.AssignedStaff != null ? src.AssignedStaff.FullName : null));

            // Entity to Detail DTO
            CreateMap<ConsultationRequest, ConsultationRequestDto>()
                .ForMember(dest => dest.OrganizerName,
                    opt => opt.MapFrom(src => src.Organizer != null ? src.Organizer.FullName : "Unknown"))
                .ForMember(dest => dest.OrganizerEmail,
                    opt => opt.MapFrom(src => src.Organizer != null ? src.Organizer.Email : "Unknown"))
                .ForMember(dest => dest.ServicePlanName,
                    opt => opt.MapFrom(src => src.ServicePlan != null ? src.ServicePlan.Name : "Unknown"))
                .ForMember(dest => dest.ConsultationType,
                    opt => opt.MapFrom(src => src.ConsultationType.ToString()))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.AssignedStaffName,
                    opt => opt.MapFrom(src => src.AssignedStaff != null ? src.AssignedStaff.FullName : null))
                .ForMember(dest => dest.CanEdit, opt => opt.Ignore())
                .ForMember(dest => dest.CanCancel, opt => opt.Ignore());

            // Create DTO to Entity
            CreateMap<CreateConsultationRequestDto, ConsultationRequest>()
                .ForMember(dest => dest.ConsultationType,
                    opt => opt.MapFrom(src => Enum.Parse<ConsultationType>(src.ConsultationType, true)))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrganizerId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedStaffId, opt => opt.Ignore())
                .ForMember(dest => dest.Notes, opt => opt.Ignore())
                .ForMember(dest => dest.ScheduledDate, opt => opt.Ignore())
                .ForMember(dest => dest.CompletedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Organizer, opt => opt.Ignore())
                .ForMember(dest => dest.ServicePlan, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedStaff, opt => opt.Ignore());
                //.ForMember(dest => dest.MentoringRecords, opt => opt.Ignore());

            // Update DTO to Entity (for partial updates)
            CreateMap<UpdateConsultationRequestDto, ConsultationRequest>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrganizerId, opt => opt.Ignore())
                .ForMember(dest => dest.ServicePlanId, opt => opt.Ignore())
                .ForMember(dest => dest.ConsultationType, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedStaffId, opt => opt.Ignore())
                .ForMember(dest => dest.Notes, opt => opt.Ignore())
                .ForMember(dest => dest.ScheduledDate, opt => opt.Ignore())
                .ForMember(dest => dest.CompletedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Organizer, opt => opt.Ignore())
                .ForMember(dest => dest.ServicePlan, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedStaff, opt => opt.Ignore());
              //  .ForMember(dest => dest.MentoringRecords, opt => opt.Ignore());
        }
    }
}
