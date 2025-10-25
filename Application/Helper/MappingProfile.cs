using Application.DTOs.Activity;
using Application.DTOs.UserDtos;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helper
{
    public partial class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Call configuration methods from partial classes
            ConfigureArticleMappings();
            ConfigureWorkshopMappings();
            ConfigureTalkEventMappings();
            ConfigureTicketTypeMappings();
            ConfigureTicketMappings();
            ConfigureUserMappings();
            //ConfigureServicePlanMappings();
            ConfigureMerchandiseMappings();
            ConfigureShippingConfigMappings();
            ConfigureOrderMappings();
            ConfigureReviewMappings();
            ConfigureSponsorMappings();
            ConfigureAdvertisementMappings();
            ConfigureOrganizationMappings();
            ConfigureActivityMappings();
            ConfigureAnalyticsMappings();
            ConfigureCartMappings();
            ConfigurePersonnelMappings();
            ConfigureConsultationRequestMappings();
            ConfigureSupportTicketMappings();
            ConfigureMentoringMappings();
            ConfigureNotificationMappings();

        }
    }
}