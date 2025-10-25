using Application.DTOs.ServicePlan;
using Domain.Entities.ServicePlan;
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
        private void ConfigureServicePlanMappings()
        {
            // Service Plan Mappings
            CreateMap<ServicePlanModel, ServicePlanDto>()
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Features)
                        ? new List<string>()
                        : JsonConvert.DeserializeObject<List<string>>(src.Features)));

            CreateMap<ServicePlanModel, ServicePlanListDto>()
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Description)
                        ? ""
                        : src.Description.Length > 150
                            ? src.Description.Substring(0, 150) + "..."
                            : src.Description));

            CreateMap<CreateServicePlanDto, ServicePlanModel>()
                .ForMember(dest => dest.Features, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<UpdateServicePlanDto, ServicePlanModel>()
                .ForMember(dest => dest.Features, opt => opt.Ignore())
                .ForMember(dest => dest.Code, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Subscription Mappings
            CreateMap<UserServicePlanSubscriptionModel, UserServicePlanSubscriptionDto>()
                .ForMember(dest => dest.ServicePlanName, opt => opt.MapFrom(src =>
                    src.ServicePlan != null ? src.ServicePlan.Name : ""))
                .ForMember(dest => dest.ServicePlanCode, opt => opt.MapFrom(src =>
                    src.ServicePlan != null ? src.ServicePlan.Code : ""))
                .ForMember(dest => dest.DaysRemaining, opt => opt.MapFrom(src =>
                    (src.EndDate - DateTime.UtcNow).Days))
                .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src =>
                    src.EndDate < DateTime.UtcNow));

            CreateMap<CreateSubscriptionDto, UserServicePlanSubscriptionModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.StartDate, opt => opt.Ignore())
                .ForMember(dest => dest.EndDate, opt => opt.Ignore())
                .ForMember(dest => dest.PaidAmount, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentStatus, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore());

            // Contract Negotiation Mappings
            CreateMap<ContractNegotiationModel, ContractNegotiationDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src =>
                    src.User != null ? src.User.FullName : ""))
                .ForMember(dest => dest.ServicePlanName, opt => opt.MapFrom(src =>
                    src.ServicePlan != null ? src.ServicePlan.Name : ""));

            CreateMap<CreateContractNegotiationDto, ContractNegotiationModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentStatus, opt => opt.Ignore())
                .ForMember(dest => dest.RequestDate, opt => opt.Ignore())
                .ForMember(dest => dest.ResponseDate, opt => opt.Ignore())
                .ForMember(dest => dest.HandledBy, opt => opt.Ignore());

            CreateMap<UpdateContractNegotiationDto, ContractNegotiationModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.ServicePlanId, opt => opt.Ignore())
                .ForMember(dest => dest.RequestType, opt => opt.Ignore())
                .ForMember(dest => dest.RequestDate, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}