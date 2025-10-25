using Domain.Entities.Admin;
using Domain.Interface.Admin;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;

namespace Infrastructure.Repo.Admin
{
    public class PerformanceMonitoringRepo : Repo<PlatformUsage>, IPerformanceMonitoringRepo
    {
        public PerformanceMonitoringRepo(AppDbContext context) : base(context) { }

        // ========== Platform Usage ==========
        public async Task<List<PlatformUsage>> GetPlatformUsageByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Set<PlatformUsage>()
                .Where(p => !p.IsDeleted &&
                           p.Date >= startDate &&
                           p.Date <= endDate)
                .ToListAsync();
        }

        public async Task<Dictionary<string, decimal>> GetUsagePercentagesAsync(DateTime startDate, DateTime endDate)
        {
            var usages = await GetPlatformUsageByPeriodAsync(startDate, endDate);

            return usages
                .GroupBy(u => u.DeviceType)
                .ToDictionary(
                    g => g.Key,
                    g => g.Average(u => u.UsagePercentage)
                );
        }

        // ========== Geographic Analytics ==========
        public async Task<List<GeographicAnalytics>> GetGeographicDataAsync(DateTime startDate, DateTime endDate)
        {
            var analytics = await _context.Set<GeographicAnalytics>()
                .Where(g => !g.IsDeleted &&
                           g.PeriodDate >= startDate &&
                           g.PeriodDate <= endDate)
                .ToListAsync();

            return analytics
                .GroupBy(g => new { g.City, g.Province })
                .Select(group => new GeographicAnalytics
                {
                    City = group.Key.City,
                    Province = group.Key.Province,
                    Country = "Vietnam",
                    UserCount = group.Sum(g => g.UserCount),
                    UserPercentage = group.Average(g => g.UserPercentage),
                    Revenue = group.Sum(g => g.Revenue),
                    OrderCount = group.Sum(g => g.OrderCount),
                    PeriodDate = endDate,
                    GrowthRate = group.Average(g => g.GrowthRate),
                    ActiveOrganizations = group.Max(g => g.ActiveOrganizations)
                })
                .OrderByDescending(g => g.UserPercentage)
                .ToList();
        }

        public async Task<List<GeographicAnalytics>> GetTopCitiesAsync(int top, DateTime startDate, DateTime endDate)
        {
            var allData = await GetGeographicDataAsync(startDate, endDate);
            return allData
                .OrderByDescending(g => g.UserPercentage)
                .Take(top)
                .ToList();
        }

        // ========== System Statistics ==========
        public async Task<SystemStatistics> GetLatestSystemStatsAsync()
        {
            return await _context.Set<SystemStatistics>()
                .Where(s => !s.IsDeleted)
                .OrderByDescending(s => s.SnapshotTime)
                .FirstOrDefaultAsync();
        }

        public async Task<List<SystemStatistics>> GetSystemStatsHistoryAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Set<SystemStatistics>()
                .Where(s => !s.IsDeleted &&
                           s.SnapshotTime >= startDate &&
                           s.SnapshotTime <= endDate)
                .OrderBy(s => s.SnapshotTime)
                .ToListAsync();
        }
    }
}
