using Application.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAnalyticsService
    {
        // Dashboard Overview
        Task<DashboardOverviewDto> GetDashboardOverviewAsync(AnalyticsQueryParams queryParams);

        // KPI Metrics
        Task<KpiSummaryDto> GetKpiSummaryAsync(DateTime? startDate, DateTime? endDate);

        // Revenue Analytics
        Task<RevenueTrendDto> GetRevenueTrendAsync(string periodType, DateTime? startDate, DateTime? endDate);
        Task<RevenueBreakdownDto> GetRevenueBreakdownAsync(DateTime? startDate, DateTime? endDate);

        // User Analytics
        Task<UserGrowthDto> GetUserGrowthAsync(DateTime? startDate, DateTime? endDate);
        Task<List<UserGrowthDataPointDto>> GetUserGrowthTrendAsync(string periodType, DateTime? startDate, DateTime? endDate);

        // Service Performance
        Task<List<TopServiceDto>> GetTopServicesAsync(int top, DateTime? startDate, DateTime? endDate);

        // Organization Performance
        Task<List<TopOrganizationDto>> GetTopOrganizationsAsync(int top, DateTime? startDate, DateTime? endDate);

        // Platform Usage
        Task<PlatformUsageSummaryDto> GetPlatformUsageSummaryAsync(DateTime? startDate, DateTime? endDate);
        Task<List<PlatformUsageDetailDto>> GetPlatformUsageDetailAsync(DateTime? startDate, DateTime? endDate);

        // Geographic Analytics
        Task<List<GeographicDataDto>> GetGeographicDistributionAsync(DateTime? startDate, DateTime? endDate);

        // Activity & Alerts
        Task<List<ActivityLogDto>> GetRecentActivitiesAsync(int count);
        Task<List<PerformanceAlertDto>> GetActivePerformanceAlertsAsync();

        // System Statistics
        Task<SystemStatisticsDto> GetSystemStatisticsAsync();

        // Data Generation (for development/testing)
        Task GenerateAnalyticsSnapshotAsync();
    }
}
