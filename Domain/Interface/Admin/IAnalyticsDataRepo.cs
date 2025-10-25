using Domain.Entities.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.Admin
{
    public interface IAnalyticsDataRepo : IRepo<KpiSnapshot>
    {
        // KPI Snapshots
        Task<KpiSnapshot> GetLatestKpiSnapshotAsync(string period = "Monthly");
        Task<List<KpiSnapshot>> GetKpiSnapshotsByPeriodAsync(DateTime startDate, DateTime endDate, string period);

        // Revenue Analytics
        Task<List<RevenueAnalytics>> GetRevenueByPeriodAsync(DateTime startDate, DateTime endDate, string periodType);
        Task<decimal> CalculateTotalRevenueAsync(DateTime startDate, DateTime endDate);
        Task<RevenueBreakdown> GetRevenueBreakdownAsync(DateTime startDate, DateTime endDate);

        // User Analytics
        Task<List<UserAnalytics>> GetUserAnalyticsByPeriodAsync(DateTime startDate, DateTime endDate);
        Task<int> GetActiveUsersCountAsync(DateTime endDate);
        Task<int> GetNewUsersCountAsync(DateTime startDate, DateTime endDate);

        // Service Analytics
        Task<List<ServiceAnalytics>> GetTopServicesAsync(int top, DateTime startDate, DateTime endDate);

        // Organization Performance
        Task<List<OrganizationPerformance>> GetTopOrganizationsAsync(int top, DateTime startDate, DateTime endDate);

        // Calculations
        Task<int> GetTotalOrdersAsync(DateTime startDate, DateTime endDate);
        Task<decimal> CalculateConversionRateAsync(DateTime startDate, DateTime endDate);
        Task<decimal> CalculateGrowthPercentageAsync(decimal currentValue, decimal previousValue);
    }
}
