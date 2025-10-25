using Application.DTOs.Organization;
using Domain.Entities.Organize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helper
{
    public partial class MappingProfile
    {
        private void ConfigureOrganizationMappings()
        {
            // Organization mappings
            CreateMap<OrganizationModel, OrganizationDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.PartnershipTier, opt => opt.MapFrom(src => src.PartnershipTier.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.LastActivityDate, opt => opt.MapFrom(src =>
                    src.Activities.Any()
                        ? src.Activities.OrderByDescending(a => a.ActivityDate).First().ActivityDate
                        : (DateTime?)null))
                .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.Members.Where(m => !m.IsDeleted && m.IsActive)));

            CreateMap<OrganizationModel, OrganizationListDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.PartnershipTier, opt => opt.MapFrom(src => src.PartnershipTier.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.LastActivityDate, opt => opt.MapFrom(src =>
                    src.Activities.Any()
                        ? src.Activities.OrderByDescending(a => a.ActivityDate).First().ActivityDate
                        : (DateTime?)null));

            CreateMap<CreateOrganizationDto, OrganizationModel>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => OrganizationType.Standard))
                .ForMember(dest => dest.PartnershipTier, opt => opt.MapFrom(src => PartnershipTier.None))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => PartnershipStatus.Pending))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.TotalEvents, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.TotalAttendees, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.MonthlyRevenue, opt => opt.MapFrom(src => 0));

            CreateMap<UpdateOrganizationDto, OrganizationModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.PartnershipTier, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.LicenseActiveUntil, opt => opt.Ignore())
                .ForMember(dest => dest.FoundedDate, opt => opt.Ignore())
                .ForMember(dest => dest.LogoUrl, opt => opt.Ignore())
                .ForMember(dest => dest.CoverImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Rating, opt => opt.Ignore())
                .ForMember(dest => dest.TotalEvents, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAttendees, opt => opt.Ignore())
                .ForMember(dest => dest.MonthlyRevenue, opt => opt.Ignore())
                .ForMember(dest => dest.Members, opt => opt.Ignore())
                .ForMember(dest => dest.Events, opt => opt.Ignore())
                .ForMember(dest => dest.Applications, opt => opt.Ignore())
                .ForMember(dest => dest.Activities, opt => opt.Ignore());

            // Organization Member mappings
            CreateMap<OrganizationMember, OrganizationMemberDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : ""))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : ""));

            CreateMap<AddOrganizationMemberDto, OrganizationMember>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Enum.Parse<OrganizationRole>(src.Role)))
                .ForMember(dest => dest.JoinedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Partnership Application mappings
            CreateMap<PartnershipApplication, PartnershipApplicationDto>()
                .ForMember(dest => dest.RequestedTier, opt => opt.MapFrom(src => src.RequestedTier.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.OrganizationName, opt => opt.MapFrom(src => src.Organization != null ? src.Organization.Name : ""))
                .ForMember(dest => dest.ReviewedByName, opt => opt.MapFrom(src => src.ReviewedBy != null ? src.ReviewedBy.FullName : ""));

            CreateMap<CreatePartnershipApplicationDto, PartnershipApplication>()
                .ForMember(dest => dest.RequestedTier, opt => opt.MapFrom(src => Enum.Parse<PartnershipTier>(src.RequestedTier)))
                .ForMember(dest => dest.ApplicationDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ApplicationStatus.Pending));

            // Organization Activity mappings
            CreateMap<OrganizationActivity, OrganizationActivityDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "System"));

            // Organization Statistics mappings
            CreateMap<OrganizationStatistics, OrganizationStatisticsDto>();
        }
    }
}