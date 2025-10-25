using Domain.Entities.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.Admin
{
    public interface IPerformanceMonitoringRepo : IRepo<PlatformUsage>
    {
        // Platform Usage
        Task<List<PlatformUsage>> GetPlatformUsageByPeriodAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<string, decimal>> GetUsagePercentagesAsync(DateTime startDate, DateTime endDate);

        // Geographic Analytics
        Task<List<GeographicAnalytics>> GetGeographicDataAsync(DateTime startDate, DateTime endDate);
        Task<List<GeographicAnalytics>> GetTopCitiesAsync(int top, DateTime startDate, DateTime endDate);

        // System Statistics
        Task<SystemStatistics> GetLatestSystemStatsAsync();
        Task<List<SystemStatistics>> GetSystemStatsHistoryAsync(DateTime startDate, DateTime endDate);
    }
}
