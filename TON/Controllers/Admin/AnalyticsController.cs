using Application.DTOs.Admin;
using Application.Interfaces;
using Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TON.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        /// <summary>
        /// Get comprehensive dashboard overview with all analytics data
        /// </summary>
        /// <param name="startDate">Optional start date (default: 30 days ago)</param>
        /// <param name="endDate">Optional end date (default: today)</param>
        /// <param name="periodType">Period type: Daily, Monthly, Yearly (default: Monthly)</param>
        /// <param name="top">Number of top items to return (default: 10)</param>
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardOverviewDto>> GetDashboardOverview(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string periodType = "Monthly",
            [FromQuery] int top = 10)
        {
            try
            {
                var queryParams = new AnalyticsQueryParams
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    PeriodType = periodType,
                    Top = top
                };

                var result = await _analyticsService.GetDashboardOverviewAsync(queryParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving dashboard data", error = ex.Message });
            }
        }

        /// <summary>
        /// Get KPI summary (Total Revenue, Active Users, Orders, Conversion Rate)
        /// </summary>
        [HttpGet("kpi-summary")]
        public async Task<ActionResult<KpiSummaryDto>> GetKpiSummary(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _analyticsService.GetKpiSummaryAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving KPI summary", error = ex.Message });
            }
        }

        /// <summary>
        /// Get revenue trend data
        /// </summary>
        [HttpGet("revenue-trend")]
        public async Task<ActionResult<RevenueTrendDto>> GetRevenueTrend(
            [FromQuery] string periodType = "Monthly",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var result = await _analyticsService.GetRevenueTrendAsync(periodType, startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving revenue trend", error = ex.Message });
            }
        }

        /// <summary>
        /// Get revenue breakdown by category
        /// </summary>
        [HttpGet("revenue-breakdown")]
        public async Task<ActionResult<RevenueBreakdownDto>> GetRevenueBreakdown(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _analyticsService.GetRevenueBreakdownAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving revenue breakdown", error = ex.Message });
            }
        }

        /// <summary>
        /// Get user growth statistics
        /// </summary>
        [HttpGet("user-growth")]
        public async Task<ActionResult<UserGrowthDto>> GetUserGrowth(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _analyticsService.GetUserGrowthAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving user growth", error = ex.Message });
            }
        }

        /// <summary>
        /// Get top performing services
        /// </summary>
        [HttpGet("top-services")]
        public async Task<ActionResult<TopServiceDto[]>> GetTopServices(
            [FromQuery] int top = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var result = await _analyticsService.GetTopServicesAsync(top, startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving top services", error = ex.Message });
            }
        }

        /// <summary>
        /// Get top performing organizations
        /// </summary>
        [HttpGet("top-organizations")]
        public async Task<ActionResult<TopOrganizationDto[]>> GetTopOrganizations(
            [FromQuery] int top = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var result = await _analyticsService.GetTopOrganizationsAsync(top, startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving top organizations", error = ex.Message });
            }
        }

        /// <summary>
        /// Get platform usage summary
        /// </summary>
        [HttpGet("platform-usage")]
        public async Task<ActionResult<PlatformUsageSummaryDto>> GetPlatformUsage(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _analyticsService.GetPlatformUsageSummaryAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving platform usage", error = ex.Message });
            }
        }

        /// <summary>
        /// Get detailed platform usage breakdown
        /// </summary>
        [HttpGet("platform-usage/detail")]
        public async Task<ActionResult<PlatformUsageDetailDto[]>> GetPlatformUsageDetail(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _analyticsService.GetPlatformUsageDetailAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving platform usage detail", error = ex.Message });
            }
        }

        /// <summary>
        /// Get geographic distribution of users
        /// </summary>
        [HttpGet("geographic-distribution")]
        public async Task<ActionResult<GeographicDataDto[]>> GetGeographicDistribution(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _analyticsService.GetGeographicDistributionAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving geographic distribution", error = ex.Message });
            }
        }

        /// <summary>
        /// Get recent activity logs
        /// </summary>
        [HttpGet("recent-activities")]
        public async Task<ActionResult<ActivityLogDto[]>> GetRecentActivities(
            [FromQuery] int count = 10)
        {
            try
            {
                var result = await _analyticsService.GetRecentActivitiesAsync(count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving recent activities", error = ex.Message });
            }
        }

        /// <summary>
        /// Get active performance alerts
        /// </summary>
        [HttpGet("performance-alerts")]
        public async Task<ActionResult<PerformanceAlertDto[]>> GetPerformanceAlerts()
        {
            try
            {
                var result = await _analyticsService.GetActivePerformanceAlertsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving performance alerts", error = ex.Message });
            }
        }

        /// <summary>
        /// Get system statistics
        /// </summary>
        [HttpGet("system-statistics")]
        public async Task<ActionResult<SystemStatisticsDto>> GetSystemStatistics()
        {
            try
            {
                var result = await _analyticsService.GetSystemStatisticsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving system statistics", error = ex.Message });
            }
        }

        /// <summary>
        /// Generate analytics snapshot (admin only - for scheduled jobs)
        /// </summary>
        [HttpPost("generate-snapshot")]
        public async Task<ActionResult> GenerateAnalyticsSnapshot()
        {
            try
            {
                await _analyticsService.GenerateAnalyticsSnapshotAsync();
                return Ok(new { message = "Analytics snapshot generated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating analytics snapshot", error = ex.Message });
            }
        }

        /// <summary>
        /// Seed sample analytics data (Development/Testing only)
        /// </summary>
        [HttpPost("seed-sample-data")]
        public async Task<ActionResult> SeedSampleData([FromServices] AnalyticsDataSeeder seeder)
        {
            try
            {
                await seeder.SeedAnalyticsDataAsync();
                return Ok(new
                {
                    message = "Sample analytics data seeded successfully",
                    note = "This endpoint should only be used in development/testing"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error seeding sample data", error = ex.Message });
            }
        }
    }
}