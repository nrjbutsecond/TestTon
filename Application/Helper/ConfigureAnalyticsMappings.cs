using Application.DTOs.Admin;
using Domain.Entities.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helper
{
    public partial class MappingProfile
    {
        private void ConfigureAnalyticsMappings()
        {
            // Revenue Analytics Mappings
            CreateMap<RevenueAnalytics, RevenueDataPointDto>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.TransactionCount, opt => opt.MapFrom(src => src.TransactionCount));

            CreateMap<RevenueBreakdown, RevenueBreakdownDto>()
                .ForMember(dest => dest.ServiceRevenue, opt => opt.MapFrom(src => src.ServiceRevenue))
                .ForMember(dest => dest.WorkshopRevenue, opt => opt.MapFrom(src => src.WorkshopRevenue))
                .ForMember(dest => dest.EventRevenue, opt => opt.MapFrom(src => src.EventRevenue))
                .ForMember(dest => dest.MerchandiseRevenue, opt => opt.MapFrom(src => src.MerchandiseRevenue))
                .ForMember(dest => dest.ConsultationRevenue, opt => opt.MapFrom(src => src.ConsultationRevenue))
                .ForMember(dest => dest.MentoringRevenue, opt => opt.MapFrom(src => src.MentoringRevenue))
                .ForMember(dest => dest.AdvertisementRevenue, opt => opt.MapFrom(src => src.AdvertisementRevenue))
                .ForMember(dest => dest.TotalRevenue, opt => opt.MapFrom(src => src.TotalRevenue));

            // Service Analytics Mappings
            CreateMap<ServiceAnalytics, TopServiceDto>()
                .ForMember(dest => dest.ServicePlanId, opt => opt.MapFrom(src => src.ServicePlanId))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.ServiceName))
                .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType))
                .ForMember(dest => dest.OrderCount, opt => opt.MapFrom(src => src.OrderCount))
                .ForMember(dest => dest.TotalRevenue, opt => opt.MapFrom(src => src.TotalRevenue))
                .ForMember(dest => dest.GrowthPercent, opt => opt.MapFrom(src => src.GrowthPercent));

            // Organization Performance Mappings
            CreateMap<OrganizationPerformance, TopOrganizationDto>()
                .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom(src => src.OrganizationId))
                .ForMember(dest => dest.OrganizationName, opt => opt.MapFrom(src => src.OrganizationName))
                .ForMember(dest => dest.TotalRevenue, opt => opt.MapFrom(src => src.TotalRevenue))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.AverageRating))
                .ForMember(dest => dest.EventCount, opt => opt.MapFrom(src => src.EventCount));

            // Geographic Analytics Mappings
            CreateMap<GeographicAnalytics, GeographicDataDto>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.Province))
                .ForMember(dest => dest.UserCount, opt => opt.MapFrom(src => src.UserCount))
                .ForMember(dest => dest.UserPercentage, opt => opt.MapFrom(src => src.UserPercentage))
                .ForMember(dest => dest.Revenue, opt => opt.MapFrom(src => src.Revenue));

            // Platform Usage Mappings
            CreateMap<PlatformUsage, PlatformUsageDetailDto>()
                .ForMember(dest => dest.DeviceType, opt => opt.MapFrom(src => src.DeviceType))
                .ForMember(dest => dest.SessionCount, opt => opt.MapFrom(src => src.SessionCount))
                .ForMember(dest => dest.UsagePercentage, opt => opt.MapFrom(src => src.UsagePercentage));

            // Activity Log Mappings
            CreateMap<ActivityLog, ActivityLogDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => src.ActivityType))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Severity, opt => opt.MapFrom(src => src.Severity))
                .ForMember(dest => dest.OccurredAt, opt => opt.MapFrom(src => src.OccurredAt))
                .ForMember(dest => dest.TimeAgo, opt => opt.Ignore()); // Calculated in service

            // Performance Alert Mappings
            CreateMap<PerformanceAlert, PerformanceAlertDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AlertType, opt => opt.MapFrom(src => src.AlertType))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.Severity, opt => opt.MapFrom(src => src.Severity))
                .ForMember(dest => dest.TriggeredAt, opt => opt.MapFrom(src => src.TriggeredAt))
                .ForMember(dest => dest.IsResolved, opt => opt.MapFrom(src => src.IsResolved));

            // System Statistics Mappings
            CreateMap<SystemStatistics, SystemStatisticsDto>()
                .ForMember(dest => dest.TotalUsers, opt => opt.MapFrom(src => src.TotalUsers))
                .ForMember(dest => dest.ActiveOrders, opt => opt.MapFrom(src => src.ActiveOrders))
                .ForMember(dest => dest.MonthlyRevenue, opt => opt.MapFrom(src => src.MonthlyRevenue))
                .ForMember(dest => dest.TotalEvents, opt => opt.MapFrom(src => src.TotalEvents))
                .ForMember(dest => dest.TotalWorkshops, opt => opt.MapFrom(src => src.TotalWorkshops))
                .ForMember(dest => dest.TotalMerchandise, opt => opt.MapFrom(src => src.TotalMerchandise))
                .ForMember(dest => dest.TotalOrganizations, opt => opt.MapFrom(src => src.TotalOrganizations));

            // KPI Snapshot Mappings
            CreateMap<KpiSnapshot, KpiSummaryDto>()
                .ForMember(dest => dest.TotalRevenue, opt => opt.MapFrom(src => src.TotalRevenue))
                .ForMember(dest => dest.ActiveUsers, opt => opt.MapFrom(src => src.TotalActiveUsers))
                .ForMember(dest => dest.TotalOrders, opt => opt.MapFrom(src => src.TotalOrders))
                .ForMember(dest => dest.ConversionRate, opt => opt.MapFrom(src => src.ConversionRate))
                .ForMember(dest => dest.RevenueChange, opt => opt.MapFrom(src => src.RevenueGrowthPercent))
                .ForMember(dest => dest.ActiveUsersChange, opt => opt.MapFrom(src => src.UserGrowthPercent))
                .ForMember(dest => dest.TotalOrdersChange, opt => opt.MapFrom(src => src.OrderGrowthPercent))
                .ForMember(dest => dest.ConversionRateChange, opt => opt.MapFrom(src => src.ConversionGrowthPercent));

            // User Analytics Mappings
            CreateMap<UserAnalytics, UserGrowthDataPointDto>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.NewUsers, opt => opt.MapFrom(src => src.NewUsers))
                .ForMember(dest => dest.ActiveUsers, opt => opt.MapFrom(src => src.ActiveUsers));
        }
    }
}