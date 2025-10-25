using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Admin
{
    // Dashboard Overview DTO
    public class DashboardOverviewDto
    {
        public KpiSummaryDto KpiSummary { get; set; }
        public List<RevenueDataPointDto> RevenueTrend { get; set; }
        public UserGrowthDto UserGrowth { get; set; }
        public List<TopServiceDto> TopServices { get; set; }
        public List<TopOrganizationDto> TopOrganizations { get; set; }
        public PlatformUsageSummaryDto PlatformUsage { get; set; }
        public List<GeographicDataDto> GeographicDistribution { get; set; }
        public List<ActivityLogDto> RecentActivities { get; set; }
        public List<PerformanceAlertDto> PerformanceAlerts { get; set; }
    }

    // KPI Summary
    public class KpiSummaryDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal RevenueChange { get; set; } // percentage
        public int ActiveUsers { get; set; }
        public decimal ActiveUsersChange { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalOrdersChange { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal ConversionRateChange { get; set; }
    }

    // Revenue Trend
    public class RevenueDataPointDto
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int TransactionCount { get; set; }
    }

    public class RevenueTrendDto
    {
        public decimal TotalRevenue { get; set; }
        public List<RevenueDataPointDto> DataPoints { get; set; }
        public string PeriodType { get; set; }
    }

    // User Growth
    public class UserGrowthDto
    {
        public int NewUsers { get; set; }
        public decimal GrowthPercentage { get; set; }
        public List<UserGrowthDataPointDto> DataPoints { get; set; }
    }

    public class UserGrowthDataPointDto
    {
        public DateTime Date { get; set; }
        public int NewUsers { get; set; }
        public int ActiveUsers { get; set; }
    }

    // Top Services
    public class TopServiceDto
    {
        public int ServicePlanId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceType { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal GrowthPercent { get; set; }
    }

    // Top Organizations
    public class TopOrganizationDto
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageRating { get; set; }
        public int EventCount { get; set; }
    }

    // Platform Usage
    public class PlatformUsageSummaryDto
    {
        public decimal DesktopPercentage { get; set; }
        public decimal MobilePercentage { get; set; }
        public decimal TabletPercentage { get; set; }
        public string AverageSessionDuration { get; set; }
    }

    public class PlatformUsageDetailDto
    {
        public string DeviceType { get; set; }
        public int SessionCount { get; set; }
        public decimal UsagePercentage { get; set; }
    }

    // Geographic Distribution
    public class GeographicDataDto
    {
        public string City { get; set; }
        public string Province { get; set; }
        public int UserCount { get; set; }
        public decimal UserPercentage { get; set; }
        public decimal Revenue { get; set; }
    }

    // Activity Log
    public class ActivityLogDto
    {
        public int Id { get; set; }
        public string ActivityType { get; set; }
        public string Description { get; set; }
        public string Severity { get; set; }
        public DateTime OccurredAt { get; set; }
        public string TimeAgo { get; set; }
    }

    // Performance Alert
    public class PerformanceAlertDto
    {
        public int Id { get; set; }
        public string AlertType { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Severity { get; set; }
        public DateTime TriggeredAt { get; set; }
        public bool IsResolved { get; set; }
    }

    // Revenue Breakdown
    public class RevenueBreakdownDto
    {
        public decimal ServiceRevenue { get; set; }
        public decimal WorkshopRevenue { get; set; }
        public decimal EventRevenue { get; set; }
        public decimal MerchandiseRevenue { get; set; }
        public decimal ConsultationRevenue { get; set; }
        public decimal MentoringRevenue { get; set; }
        public decimal AdvertisementRevenue { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    // System Statistics
    public class SystemStatisticsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveOrders { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int TotalEvents { get; set; }
        public int TotalWorkshops { get; set; }
        public int TotalMerchandise { get; set; }
        public int TotalOrganizations { get; set; }
    }

    // Query Parameters
    public class AnalyticsQueryParams
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string PeriodType { get; set; } = "Monthly"; // Daily, Monthly, Yearly
        public int Top { get; set; } = 10;
    }
}
