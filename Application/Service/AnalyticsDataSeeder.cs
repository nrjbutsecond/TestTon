using Domain.Entities.Admin;
using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class AnalyticsDataSeeder
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsDataSeeder(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task SeedAnalyticsDataAsync()
        {
            await SeedKpiSnapshotsAsync();
            await SeedRevenueAnalyticsAsync();
            await SeedUserAnalyticsAsync();
            await SeedServiceAnalyticsAsync();
            await SeedOrganizationPerformanceAsync();
            await SeedPlatformUsageAsync();
            await SeedGeographicAnalyticsAsync();
            await SeedActivityLogsAsync();
            await SeedPerformanceAlertsAsync();
            await SeedSystemStatisticsAsync();

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task SeedKpiSnapshotsAsync()
        {
            var snapshot = new KpiSnapshot
            {
                SnapshotDate = DateTime.UtcNow,
                TotalRevenue = 2_800_000_000, // 2.8B VND
                TotalActiveUsers = 1247,
                TotalOrders = 2840,
                ConversionRate = 24.8m,
                RevenueGrowthPercent = 18.2m,
                UserGrowthPercent = 7.4m,
                OrderGrowthPercent = -3.1m,
                ConversionGrowthPercent = 2.3m,
                Period = "Monthly",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repo<KpiSnapshot>().AddAsync(snapshot);
        }

        private async Task SeedRevenueAnalyticsAsync()
        {
            var revenueData = new List<RevenueAnalytics>();
            var startDate = DateTime.UtcNow.AddDays(-30);

            for (int i = 0; i < 30; i++)
            {
                var date = startDate.AddDays(i);
                var baseAmount = 85_000_000 + (i * 1_500_000); // Trend tăng dần

                revenueData.Add(new RevenueAnalytics
                {
                    Date = date,
                    Amount = baseAmount,
                    Currency = "VND",
                    PeriodType = "Daily",
                    TransactionCount = 80 + (i * 3),
                    AverageOrderValue = baseAmount / (80 + (i * 3)),
                    Category = "Service",
                    OrganizationId = 0,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _unitOfWork.Repo<RevenueAnalytics>().AddRangeAsync(revenueData);
        }

        private async Task SeedUserAnalyticsAsync()
        {
            var userAnalytics = new List<UserAnalytics>();
            var startDate = DateTime.UtcNow.AddDays(-30);

            for (int i = 0; i < 30; i++)
            {
                var date = startDate.AddDays(i);

                userAnalytics.Add(new UserAnalytics
                {
                    Date = date,
                    NewUsers = 4 + (i % 7), // +156 total new users in month
                    ActiveUsers = 1200 + (i * 2),
                    ReturnUsers = 850 + i,
                    ChurnedUsers = 5,
                    RetentionRate = 92.5m,
                    GrowthRate = 12.5m,
                    UserType = "Member",
                    AcquisitionChannel = "Organic",
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _unitOfWork.Repo<UserAnalytics>().AddRangeAsync(userAnalytics);
        }

        private async Task SeedServiceAnalyticsAsync()
        {
            var services = new List<ServiceAnalytics>
            {
                new ServiceAnalytics
                {
                    ServicePlanId = 1,
                    ServiceName = "Basic Package",
                    ServiceType = "Event Planning",
                    OrderCount = 45,
                    TotalRevenue = 450_000_000,
                    AverageRating = 4.5m,
                    PeriodStart = DateTime.UtcNow.AddMonths(-1),
                    PeriodEnd = DateTime.UtcNow,
                    GrowthPercent = 10m,
                    UniqueCustomers = 38,
                    CreatedAt = DateTime.UtcNow
                },
                new ServiceAnalytics
                {
                    ServicePlanId = 2,
                    ServiceName = "Event Experience",
                    ServiceType = "Experience Design",
                    OrderCount = 32,
                    TotalRevenue = 380_000_000,
                    AverageRating = 4.7m,
                    PeriodStart = DateTime.UtcNow.AddMonths(-1),
                    PeriodEnd = DateTime.UtcNow,
                    GrowthPercent = 8m,
                    UniqueCustomers = 28,
                    CreatedAt = DateTime.UtcNow
                },
                new ServiceAnalytics
                {
                    ServicePlanId = 3,
                    ServiceName = "Mentoring (Full)",
                    ServiceType = "Complete Mentoring",
                    OrderCount = 28,
                    TotalRevenue = 320_000_000,
                    AverageRating = 4.8m,
                    PeriodStart = DateTime.UtcNow.AddMonths(-1),
                    PeriodEnd = DateTime.UtcNow,
                    GrowthPercent = -3m,
                    UniqueCustomers = 25,
                    CreatedAt = DateTime.UtcNow
                },
                new ServiceAnalytics
                {
                    ServicePlanId = 4,
                    ServiceName = "WOW Package",
                    ServiceType = "Premium Service",
                    OrderCount = 12,
                    TotalRevenue = 180_000_000,
                    AverageRating = 4.9m,
                    PeriodStart = DateTime.UtcNow.AddMonths(-1),
                    PeriodEnd = DateTime.UtcNow,
                    GrowthPercent = 25m,
                    UniqueCustomers = 12,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await _unitOfWork.Repo<ServiceAnalytics>().AddRangeAsync(services);
        }

        private async Task SeedOrganizationPerformanceAsync()
        {
            var organizations = new List<OrganizationPerformance>
            {
                new OrganizationPerformance
                {
                    OrganizationId = 1,
                    OrganizationName = "TEDxHCMC",
                    TotalRevenue = 450_000_000,
                    EventCount = 15,
                    TicketsSold = 2500,
                    AverageRating = 4.9m,
                    ReviewCount = 450,
                    PeriodStart = DateTime.UtcNow.AddMonths(-1),
                    PeriodEnd = DateTime.UtcNow,
                    Ranking = 1,
                    Region = "Ho Chi Minh City",
                    CreatedAt = DateTime.UtcNow
                },
                new OrganizationPerformance
                {
                    OrganizationId = 2,
                    OrganizationName = "TEDxHanoi",
                    TotalRevenue = 380_000_000,
                    EventCount = 12,
                    TicketsSold = 2100,
                    AverageRating = 4.8m,
                    ReviewCount = 380,
                    PeriodStart = DateTime.UtcNow.AddMonths(-1),
                    PeriodEnd = DateTime.UtcNow,
                    Ranking = 2,
                    Region = "Hanoi",
                    CreatedAt = DateTime.UtcNow
                },
                new OrganizationPerformance
                {
                    OrganizationId = 3,
                    OrganizationName = "TEDxDaNang",
                    TotalRevenue = 180_000_000,
                    EventCount = 8,
                    TicketsSold = 1200,
                    AverageRating = 4.7m,
                    ReviewCount = 200,
                    PeriodStart = DateTime.UtcNow.AddMonths(-1),
                    PeriodEnd = DateTime.UtcNow,
                    Ranking = 3,
                    Region = "Da Nang",
                    CreatedAt = DateTime.UtcNow
                }
            };

            await _unitOfWork.Repo<OrganizationPerformance>().AddRangeAsync(organizations);
        }

        private async Task SeedPlatformUsageAsync()
        {
            var platformUsage = new List<PlatformUsage>
            {
                new PlatformUsage
                {
                    Date = DateTime.UtcNow,
                    DeviceType = "Desktop",
                    SessionCount = 8500,
                    UniqueUsers = 850,
                    UsagePercentage = 68m,
                    AverageSessionDuration = TimeSpan.FromMinutes(12).Add(TimeSpan.FromSeconds(34)),
                    PageViewsPerSession = 8,
                    BounceRate = 32.5m,
                    Browser = "Chrome",
                    OperatingSystem = "Windows",
                    CreatedAt = DateTime.UtcNow
                },
                new PlatformUsage
                {
                    Date = DateTime.UtcNow,
                    DeviceType = "Mobile",
                    SessionCount = 3500,
                    UniqueUsers = 350,
                    UsagePercentage = 28m,
                    AverageSessionDuration = TimeSpan.FromMinutes(8).Add(TimeSpan.FromSeconds(20)),
                    PageViewsPerSession = 5,
                    BounceRate = 45.2m,
                    Browser = "Safari",
                    OperatingSystem = "iOS",
                    CreatedAt = DateTime.UtcNow
                },
                new PlatformUsage
                {
                    Date = DateTime.UtcNow,
                    DeviceType = "Tablet",
                    SessionCount = 500,
                    UniqueUsers = 50,
                    UsagePercentage = 4m,
                    AverageSessionDuration = TimeSpan.FromMinutes(10).Add(TimeSpan.FromSeconds(15)),
                    PageViewsPerSession = 6,
                    BounceRate = 38.7m,
                    Browser = "Safari",
                    OperatingSystem = "iPadOS",
                    CreatedAt = DateTime.UtcNow
                }
            };

            await _unitOfWork.Repo<PlatformUsage>().AddRangeAsync(platformUsage);
        }

        private async Task SeedGeographicAnalyticsAsync()
        {
            var geoData = new List<GeographicAnalytics>
            {
                new GeographicAnalytics
                {
                    City = "Ho Chi Minh City",
                    Province = "Ho Chi Minh",
                    Country = "Vietnam",
                    UserCount = 562,
                    UserPercentage = 45m,
                    Revenue = 1_260_000_000,
                    OrderCount = 1280,
                    PeriodDate = DateTime.UtcNow,
                    GrowthRate = 15.3m,
                    ActiveOrganizations = 8,
                    CreatedAt = DateTime.UtcNow
                },
                new GeographicAnalytics
                {
                    City = "Hanoi",
                    Province = "Hanoi",
                    Country = "Vietnam",
                    UserCount = 349,
                    UserPercentage = 28m,
                    Revenue = 784_000_000,
                    OrderCount = 795,
                    PeriodDate = DateTime.UtcNow,
                    GrowthRate = 12.1m,
                    ActiveOrganizations = 6,
                    CreatedAt = DateTime.UtcNow
                },
                new GeographicAnalytics
                {
                    City = "Da Nang",
                    Province = "Da Nang",
                    Country = "Vietnam",
                    UserCount = 187,
                    UserPercentage = 15m,
                    Revenue = 420_000_000,
                    OrderCount = 426,
                    PeriodDate = DateTime.UtcNow,
                    GrowthRate = 8.7m,
                    ActiveOrganizations = 4,
                    CreatedAt = DateTime.UtcNow
                },
                new GeographicAnalytics
                {
                    City = "Other Cities",
                    Province = "Various",
                    Country = "Vietnam",
                    UserCount = 149,
                    UserPercentage = 12m,
                    Revenue = 336_000_000,
                    OrderCount = 339,
                    PeriodDate = DateTime.UtcNow,
                    GrowthRate = 5.2m,
                    ActiveOrganizations = 3,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await _unitOfWork.Repo<GeographicAnalytics>().AddRangeAsync(geoData);
        }

        private async Task SeedActivityLogsAsync()
        {
            var activities = new List<ActivityLog>
            {
                new ActivityLog
                {
                    ActivityType = "Revenue Spike",
                    Description = "High revenue spike detected - 2.8B VND in last 30 days (+18%)",
                    Severity = "Info",
                    Category = "Revenue",
                    OccurredAt = DateTime.UtcNow.AddHours(-2),
                    UserId = "admin",
                    EntityType = "Revenue",
                    EntityId = null,
                    Metadata = "{\"amount\": 2800000000, \"growth\": 18}",
                    CreatedAt = DateTime.UtcNow.AddHours(-2)
                },
                new ActivityLog
                {
                    ActivityType = "Milestone",
                    Description = "New user milestone reached - 1,247 active users (+12.5%)",
                    Severity = "Info",
                    Category = "User",
                    OccurredAt = DateTime.UtcNow.AddHours(-4),
                    UserId = "system",
                    EntityType = "User",
                    EntityId = null,
                    Metadata = "{\"userCount\": 1247, \"growth\": 12.5}",
                    CreatedAt = DateTime.UtcNow.AddHours(-4)
                },
                new ActivityLog
                {
                    ActivityType = "Alert",
                    Description = "Order completion rate improved - 24.8% conversion rate (+2.3%)",
                    Severity = "Warning",
                    Category = "Order",
                    OccurredAt = DateTime.UtcNow.AddHours(-6),
                    UserId = "system",
                    EntityType = "Order",
                    EntityId = null,
                    Metadata = "{\"conversionRate\": 24.8, \"growth\": 2.3}",
                    CreatedAt = DateTime.UtcNow.AddHours(-6)
                }
            };

            await _unitOfWork.Repo<ActivityLog>().AddRangeAsync(activities);
        }

        private async Task SeedPerformanceAlertsAsync()
        {
            var alerts = new List<PerformanceAlert>
            {
                new PerformanceAlert
                {
                    AlertType = "Revenue Growth",
                    Title = "Revenue Growth",
                    Message = "18% increase in monthly revenue",
                    Severity = "Low",
                    TriggeredAt = DateTime.UtcNow.AddDays(-1),
                    IsResolved = false,
                    ThresholdValue = 10.0m,
                    ActualValue = 18.0m,
                    MetricName = "MonthlyRevenueGrowth",
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new PerformanceAlert
                {
                    AlertType = "User Engagement",
                    Title = "User Engagement",
                    Message = "Average session time increased to 12m 34s",
                    Severity = "Low",
                    TriggeredAt = DateTime.UtcNow.AddHours(-2),
                    IsResolved = false,
                    ThresholdValue = 10.0m,
                    ActualValue = 12.57m,
                    MetricName = "AverageSessionTime",
                    CreatedAt = DateTime.UtcNow.AddHours(-2)
                },
                new PerformanceAlert
                {
                    AlertType = "Order Volume",
                    Title = "Order Volume",
                    Message = "-3% decrease in orders needs attention",
                    Severity = "Medium",
                    TriggeredAt = DateTime.UtcNow.AddHours(-5),
                    IsResolved = false,
                    ThresholdValue = 0.0m,
                    ActualValue = -3.0m,
                    MetricName = "OrderVolumeChange",
                    CreatedAt = DateTime.UtcNow.AddHours(-5)
                }
            };

            await _unitOfWork.Repo<PerformanceAlert>().AddRangeAsync(alerts);
        }

        private async Task SeedSystemStatisticsAsync()
        {
            var stats = new SystemStatistics
            {
                SnapshotTime = DateTime.UtcNow,
                TotalUsers = 1247,
                ActiveOrders = 89,
                MonthlyRevenue = 450_000_000, // 450M VND
                TotalEvents = 45,
                TotalWorkshops = 32,
                TotalMerchandise = 156,
                TotalOrganizations = 28,
                SystemUptime = 99.9m,
                ApiCallCount = 125_000,
                AverageResponseTime = 125.5m,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repo<SystemStatistics>().AddAsync(stats);
        }
    }
}