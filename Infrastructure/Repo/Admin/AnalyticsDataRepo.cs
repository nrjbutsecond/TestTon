using Domain.Entities.Admin;
using Domain.Interface.Admin;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;
using Domain.Entities.MerchandiseEntity;

namespace Infrastructure.Repo.Admin
{
    public class AnalyticsDataRepo : Repo<KpiSnapshot>, IAnalyticsDataRepo
    {
        public AnalyticsDataRepo(AppDbContext context) : base(context) { }

        // ========== KPI Snapshots ==========
        public async Task<KpiSnapshot> GetLatestKpiSnapshotAsync(string period = "Monthly")
        {
            return await _context.Set<KpiSnapshot>()
                .Where(k => !k.IsDeleted && k.Period == period)
                .OrderByDescending(k => k.SnapshotDate)
                .FirstOrDefaultAsync();
        }

        public async Task<List<KpiSnapshot>> GetKpiSnapshotsByPeriodAsync(DateTime startDate, DateTime endDate, string period)
        {
            return await _context.Set<KpiSnapshot>()
                .Where(k => !k.IsDeleted &&
                           k.Period == period &&
                           k.SnapshotDate >= startDate &&
                           k.SnapshotDate <= endDate)
                .OrderByDescending(k => k.SnapshotDate)
                .ToListAsync();
        }

        // ========== Revenue Analytics ==========
        public async Task<List<RevenueAnalytics>> GetRevenueByPeriodAsync(DateTime startDate, DateTime endDate, string periodType)
        {
            return await _context.Set<RevenueAnalytics>()
                .Where(r => !r.IsDeleted &&
                           r.Date >= startDate &&
                           r.Date <= endDate &&
                           r.PeriodType == periodType)
                .OrderBy(r => r.Date)
                .ToListAsync();
        }

        public async Task<decimal> CalculateTotalRevenueAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _context.Orders
                .Where(o => !o.IsDeleted &&
                           o.OrderDate >= startDate &&
                           o.OrderDate <= endDate &&
                           (o.Status == OrderStatus.Delivered || o.Status == OrderStatus.Shipped))
                .ToListAsync();

            return orders.Sum(o => o.TotalAmount);
        }

        public async Task<RevenueBreakdown> GetRevenueBreakdownAsync(DateTime startDate, DateTime endDate)
        {
            var breakdowns = await _context.Set<RevenueBreakdown>()
                .Where(r => !r.IsDeleted &&
                           r.PeriodDate >= startDate &&
                           r.PeriodDate <= endDate)
                .ToListAsync();

            if (!breakdowns.Any())
                return null;

            return new RevenueBreakdown
            {
                PeriodDate = endDate,
                ServiceRevenue = breakdowns.Sum(b => b.ServiceRevenue),
                WorkshopRevenue = breakdowns.Sum(b => b.WorkshopRevenue),
                EventRevenue = breakdowns.Sum(b => b.EventRevenue),
                MerchandiseRevenue = breakdowns.Sum(b => b.MerchandiseRevenue),
                ConsultationRevenue = breakdowns.Sum(b => b.ConsultationRevenue),
                MentoringRevenue = breakdowns.Sum(b => b.MentoringRevenue),
                AdvertisementRevenue = breakdowns.Sum(b => b.AdvertisementRevenue),
                TotalRevenue = breakdowns.Sum(b => b.TotalRevenue),
                PeriodType = "Aggregated"
            };
        }

        // ========== User Analytics ==========
        public async Task<List<UserAnalytics>> GetUserAnalyticsByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Set<UserAnalytics>()
                .Where(u => !u.IsDeleted &&
                           u.Date >= startDate &&
                           u.Date <= endDate)
                .OrderBy(u => u.Date)
                .ToListAsync();
        }

        public async Task<int> GetActiveUsersCountAsync(DateTime endDate)
        {
            return await _context.Users
               .Where(u => !u.IsDeleted &&
               u.IsActive &&
               u.CreatedAt <= endDate) 
                .CountAsync();
        }

        public async Task<int> GetNewUsersCountAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Users
                .Where(u => !u.IsDeleted &&
                           u.CreatedAt >= startDate &&
                           u.CreatedAt <= endDate)
                .CountAsync();
        }

        // ========== Service Analytics ==========
        public async Task<List<ServiceAnalytics>> GetTopServicesAsync(int top, DateTime startDate, DateTime endDate)
        {
            return await _context.Set<ServiceAnalytics>()
                .Where(s => !s.IsDeleted &&
                           s.PeriodStart >= startDate &&
                           s.PeriodEnd <= endDate)
                .OrderByDescending(s => s.TotalRevenue)
                .Take(top)
                .ToListAsync();
        }

        // ========== Organization Performance ==========
        public async Task<List<OrganizationPerformance>> GetTopOrganizationsAsync(int top, DateTime startDate, DateTime endDate)
        {
            return await _context.Set<OrganizationPerformance>()
                .Where(o => !o.IsDeleted &&
                           o.PeriodStart >= startDate &&
                           o.PeriodEnd <= endDate)
                .OrderByDescending(o => o.TotalRevenue)
                .Take(top)
                .ToListAsync();
        }

        // ========== Calculations ==========
        public async Task<int> GetTotalOrdersAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Where(o => !o.IsDeleted &&
                           o.OrderDate >= startDate &&
                           o.OrderDate <= endDate)
                .CountAsync();
        }

        public async Task<decimal> CalculateConversionRateAsync(DateTime startDate, DateTime endDate)
        {
            var totalUsers = await GetActiveUsersCountAsync(endDate);
            var totalOrders = await GetTotalOrdersAsync(startDate, endDate);

            if (totalUsers == 0)
                return 0;

            return Math.Round((decimal)totalOrders / totalUsers * 100, 2);
        }

        public Task<decimal> CalculateGrowthPercentageAsync(decimal currentValue, decimal previousValue)
        {
            if (previousValue == 0)
                return Task.FromResult(currentValue > 0 ? 100m : 0m);

            var growth = Math.Round(((currentValue - previousValue) / previousValue) * 100, 2);
            return Task.FromResult(growth);
        }
    }
}