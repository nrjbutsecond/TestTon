using Application.DTOs.Admin;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities.Admin;
using Domain.Interface.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IAnalyticsDataRepo _analyticsDataRepo;
        private readonly IPerformanceMonitoringRepo _performanceRepo;
        private readonly IActivityTrackingRepo _activityRepo;
        private readonly IMapper _mapper;

        public AnalyticsService(
            IAnalyticsDataRepo analyticsDataRepo,
            IPerformanceMonitoringRepo performanceRepo,
            IActivityTrackingRepo activityRepo,
            IMapper mapper)
        {
            _analyticsDataRepo = analyticsDataRepo;
            _performanceRepo = performanceRepo;
            _activityRepo = activityRepo;
            _mapper = mapper;
        }

        public async Task<DashboardOverviewDto> GetDashboardOverviewAsync(AnalyticsQueryParams queryParams)
        {
            var endDate = queryParams.EndDate ?? DateTime.UtcNow;
            var startDate = queryParams.StartDate ?? endDate.AddDays(-30);

            return new DashboardOverviewDto
            {
                KpiSummary = await GetKpiSummaryAsync(startDate, endDate),
                RevenueTrend = (await GetRevenueTrendAsync(queryParams.PeriodType, startDate, endDate)).DataPoints,
                UserGrowth = await GetUserGrowthAsync(startDate, endDate),
                TopServices = await GetTopServicesAsync(queryParams.Top, startDate, endDate),
                TopOrganizations = await GetTopOrganizationsAsync(queryParams.Top, startDate, endDate),
                PlatformUsage = await GetPlatformUsageSummaryAsync(startDate, endDate),
                GeographicDistribution = await GetGeographicDistributionAsync(startDate, endDate),
                RecentActivities = await GetRecentActivitiesAsync(10),
                PerformanceAlerts = await GetActivePerformanceAlertsAsync()
            };
        }

        public async Task<KpiSummaryDto> GetKpiSummaryAsync(DateTime? startDate, DateTime? endDate)
        {
            var end = endDate ?? DateTime.UtcNow;
            var start = startDate ?? end.AddDays(-30);
            var previousEnd = start.AddDays(-1);
            var previousStart = previousEnd.AddDays(-(end - start).Days);

            // Current period metrics
            var currentRevenue = await _analyticsDataRepo.CalculateTotalRevenueAsync(start, end);
            var currentUsers = await _analyticsDataRepo.GetActiveUsersCountAsync(end);
            var currentOrders = await _analyticsDataRepo.GetTotalOrdersAsync(start, end);
            var currentConversion = await _analyticsDataRepo.CalculateConversionRateAsync(start, end);

            // Previous period metrics
            var previousRevenue = await _analyticsDataRepo.CalculateTotalRevenueAsync(previousStart, previousEnd);
            var previousUsers = await _analyticsDataRepo.GetActiveUsersCountAsync(previousEnd);
            var previousOrders = await _analyticsDataRepo.GetTotalOrdersAsync(previousStart, previousEnd);
            var previousConversion = await _analyticsDataRepo.CalculateConversionRateAsync(previousStart, previousEnd);

            // Calculate growth percentages
            var revenueChange = await _analyticsDataRepo.CalculateGrowthPercentageAsync(currentRevenue, previousRevenue);
            var usersChange = await _analyticsDataRepo.CalculateGrowthPercentageAsync(currentUsers, previousUsers);
            var ordersChange = await _analyticsDataRepo.CalculateGrowthPercentageAsync(currentOrders, previousOrders);
            var conversionChange = await _analyticsDataRepo.CalculateGrowthPercentageAsync(currentConversion, previousConversion);

            return new KpiSummaryDto
            {
                TotalRevenue = currentRevenue,
                RevenueChange = revenueChange,
                ActiveUsers = currentUsers,
                ActiveUsersChange = usersChange,
                TotalOrders = currentOrders,
                TotalOrdersChange = ordersChange,
                ConversionRate = currentConversion,
                ConversionRateChange = conversionChange
            };
        }

        public async Task<RevenueTrendDto> GetRevenueTrendAsync(string periodType, DateTime? startDate, DateTime? endDate)
        {
            var end = endDate ?? DateTime.UtcNow;
            var start = startDate ?? end.AddDays(-30);

            var revenueData = await _analyticsDataRepo.GetRevenueByPeriodAsync(start, end, periodType);

            var dataPoints = revenueData
                .Select(r => _mapper.Map<RevenueDataPointDto>(r))
                .ToList();

            return new RevenueTrendDto
            {
                TotalRevenue = dataPoints.Sum(d => d.Amount),
                DataPoints = dataPoints,
                PeriodType = periodType
            };
        }

        public async Task<RevenueBreakdownDto> GetRevenueBreakdownAsync(DateTime? startDate, DateTime? endDate)
        {
            var end = endDate ?? DateTime.UtcNow;
            var start = startDate ?? end.AddMonths(-1);

            var breakdown = await _analyticsDataRepo.GetRevenueBreakdownAsync(start, end);

            if (breakdown == null)
            {
                return new RevenueBreakdownDto();
            }

            return _mapper.Map<RevenueBreakdownDto>(breakdown);
        }

        public async Task<UserGrowthDto> GetUserGrowthAsync(DateTime? startDate, DateTime? endDate)
        {
            var end = endDate ?? DateTime.UtcNow;
            var start = startDate ?? end.AddDays(-30);

            var userAnalytics = await _analyticsDataRepo.GetUserAnalyticsByPeriodAsync(start, end);

            var dataPoints = userAnalytics
                .Select(u => _mapper.Map<UserGrowthDataPointDto>(u))
                .ToList();

            var totalNewUsers = dataPoints.Sum(d => d.NewUsers);

            // Previous period for comparison
            var previousEnd = start.AddDays(-1);
            var previousStart = previousEnd.AddDays(-(end - start).Days);
            var previousPeriodUsers = await _analyticsDataRepo.GetUserAnalyticsByPeriodAsync(previousStart, previousEnd);
            var previousTotal = previousPeriodUsers.Sum(u => u.NewUsers);

            var growthPercentage = await _analyticsDataRepo.CalculateGrowthPercentageAsync(totalNewUsers, previousTotal);

            return new UserGrowthDto
            {
                NewUsers = totalNewUsers,
                GrowthPercentage = growthPercentage,
                DataPoints = dataPoints
            };
        }

        public async Task<List<UserGrowthDataPointDto>> GetUserGrowthTrendAsync(string periodType, DateTime? startDate, DateTime? endDate)
        {
            var end = endDate ?? DateTime.UtcNow;
            var start = startDate ?? end.AddDays(-30);

            var userAnalytics = await _analyticsDataRepo.GetUserAnalyticsByPeriodAsync(start, end);

            return userAnalytics
                .Select(u => _mapper.Map<UserGrowthDataPointDto>(u))
                .ToList();
        }

        public async Task<List<TopServiceDto>> GetTopServicesAsync(int top, DateTime? startDate, DateTime? endDate)
        {
            var end = endDate ?? DateTime.UtcNow;
            var start = startDate ?? end.AddMonths(-1);

            var services = await _analyticsDataRepo.GetTopServicesAsync(top, start, end);

            return services
                .Select(s => _mapper.Map<TopServiceDto>(s))
                .ToList();
        }

        public async Task<List<TopOrganizationDto>> GetTopOrganizationsAsync(int top, DateTime? startDate, DateTime? endDate)
        {
            var end = endDate ?? DateTime.UtcNow;
            var start = startDate ?? end.AddMonths(-1);

            var organizations = await _analyticsDataRepo.GetTopOrganizationsAsync(top, start, end);

            return organizations
                .Select(o => _mapper.Map<TopOrganizationDto>(o))
                .ToList();
        }

        public async Task<PlatformUsageSummaryDto> GetPlatformUsageSummaryAsync(DateTime? startDate, DateTime? endDate)
        {
            var end = endDate ?? DateTime.UtcNow;
            var start = startDate ?? end.AddDays(-30);

            var usagePercentages = await _performanceRepo.GetUsagePercentagesAsync(start, end);
            var platformUsages = await _performanceRepo.GetPlatformUsageByPeriodAsync(start, end);

            var avgDurationSeconds = platformUsages.Any()
                ? platformUsages.Average(p => p.AverageSessionDuration.TotalSeconds)
                : 0;
            var avgDuration = TimeSpan.FromSeconds(avgDurationSeconds);

            return new PlatformUsageSummaryDto
            {
                DesktopPercentage = usagePercentages.GetValueOrDefault("Desktop", 0),
                MobilePercentage = usagePercentages.GetValueOrDefault("Mobile", 0),
                TabletPercentage = usagePercentages.GetValueOrDefault("Tablet", 0),
                AverageSessionDuration = $"{(int)avgDuration.TotalMinutes}m {avgDuration.Seconds}s"
            };
        }

        public async Task<List<PlatformUsageDetailDto>> GetPlatformUsageDetailAsync(DateTime? startDate, DateTime? endDate)
        {
            var end = endDate ?? DateTime.UtcNow;
            var start = startDate ?? end.AddDays(-30);

            var usage = await _performanceRepo.GetPlatformUsageByPeriodAsync(start, end);

            return usage.GroupBy(u => u.DeviceType)
                .Select(g => new PlatformUsageDetailDto
                {
                    DeviceType = g.Key,
                    SessionCount = g.Sum(u => u.SessionCount),
                    UsagePercentage = g.Average(u => u.UsagePercentage)
                })
                .OrderByDescending(p => p.UsagePercentage)
                .ToList();
        }

        public async Task<List<GeographicDataDto>> GetGeographicDistributionAsync(DateTime? startDate, DateTime? endDate)
        {
            var end = endDate ?? DateTime.UtcNow;
            var start = startDate ?? end.AddDays(-30);

            var geoData = await _performanceRepo.GetGeographicDataAsync(start, end);

            return geoData
                .Select(g => _mapper.Map<GeographicDataDto>(g))
                .ToList();
        }

        public async Task<List<ActivityLogDto>> GetRecentActivitiesAsync(int count)
        {
            var activities = await _activityRepo.GetRecentActivitiesAsync(count);

            return activities.Select(a => new ActivityLogDto
            {
                Id = a.Id,
                ActivityType = a.ActivityType,
                Description = a.Description,
                Severity = a.Severity,
                OccurredAt = a.OccurredAt,
                TimeAgo = GetTimeAgo(a.OccurredAt)
            }).ToList();
        }

        public async Task<List<PerformanceAlertDto>> GetActivePerformanceAlertsAsync()
        {
            var alerts = await _activityRepo.GetActiveAlertsAsync();

            return alerts
                .Select(a => _mapper.Map<PerformanceAlertDto>(a))
                .ToList();
        }

        public async Task<SystemStatisticsDto> GetSystemStatisticsAsync()
        {
            var latest = await _performanceRepo.GetLatestSystemStatsAsync();

            if (latest == null)
            {
                return new SystemStatisticsDto();
            }

            return _mapper.Map<SystemStatisticsDto>(latest);
        }

        public async Task GenerateAnalyticsSnapshotAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = now;

            // Calculate current period metrics
            var totalRevenue = await _analyticsDataRepo.CalculateTotalRevenueAsync(startOfMonth, endOfMonth);
            var activeUsers = await _analyticsDataRepo.GetActiveUsersCountAsync(endOfMonth);
            var totalOrders = await _analyticsDataRepo.GetTotalOrdersAsync(startOfMonth, endOfMonth);
            var conversionRate = await _analyticsDataRepo.CalculateConversionRateAsync(startOfMonth, endOfMonth);

            // Calculate previous period for growth
            var previousMonthStart = startOfMonth.AddMonths(-1);
            var previousMonthEnd = startOfMonth.AddDays(-1);

            var previousRevenue = await _analyticsDataRepo.CalculateTotalRevenueAsync(previousMonthStart, previousMonthEnd);
            var previousUsers = await _analyticsDataRepo.GetActiveUsersCountAsync(previousMonthEnd);
            var previousOrders = await _analyticsDataRepo.GetTotalOrdersAsync(previousMonthStart, previousMonthEnd);
            var previousConversion = await _analyticsDataRepo.CalculateConversionRateAsync(previousMonthStart, previousMonthEnd);

            // Calculate growth percentages
            var revenueGrowth = await _analyticsDataRepo.CalculateGrowthPercentageAsync(totalRevenue, previousRevenue);
            var userGrowth = await _analyticsDataRepo.CalculateGrowthPercentageAsync(activeUsers, previousUsers);
            var orderGrowth = await _analyticsDataRepo.CalculateGrowthPercentageAsync(totalOrders, previousOrders);
            var conversionGrowth = await _analyticsDataRepo.CalculateGrowthPercentageAsync(conversionRate, previousConversion);

            // Create KPI snapshot
            var kpiSnapshot = new KpiSnapshot
            {
                SnapshotDate = now,
                TotalRevenue = totalRevenue,
                TotalActiveUsers = activeUsers,
                TotalOrders = totalOrders,
                ConversionRate = conversionRate,
                RevenueGrowthPercent = revenueGrowth,
                UserGrowthPercent = userGrowth,
                OrderGrowthPercent = orderGrowth,
                ConversionGrowthPercent = conversionGrowth,
                Period = "Monthly",
                CreatedAt = now
            };

            await _analyticsDataRepo.AddAsync(kpiSnapshot);
        }

        // Helper Methods
        private string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;

            if (timeSpan.TotalMinutes < 1) return "just now";
            if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalDays < 30) return $"{(int)timeSpan.TotalDays} days ago";
            if (timeSpan.TotalDays < 365) return $"{(int)(timeSpan.TotalDays / 30)} months ago";
            return $"{(int)(timeSpan.TotalDays / 365)} years ago";
        }
    }
}