using Domain.Entities;
using Domain.Entities.ServicePlan;
using Domain.Interface.ServicePlan;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;

namespace Infrastructure.Repo.ServicePlan
{
    public class ServicePlanRepo : Repo<ServicePlanModel>, IServicePlanRepo
    {
        public ServicePlanRepo(AppDbContext context) : base(context) { }

        public async Task<ServicePlanModel> GetByCodeAsync(string code)
        {
            return await _context.ServicePlans
                .FirstOrDefaultAsync(sp => sp.Code == code && !sp.IsDeleted);
        }

        public async Task<IEnumerable<ServicePlanModel>> GetActivePlansAsync()
        {
            return await _context.ServicePlans
                .Where(sp => sp.IsActive && !sp.IsDeleted)
                .OrderBy(sp => sp.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServicePlanModel>> GetPopularPlansAsync()
        {
            return await _context.ServicePlans
                .Where(sp => sp.IsActive && sp.IsPopular && !sp.IsDeleted)
                .OrderBy(sp => sp.DisplayOrder)
                .ToListAsync();
        }

        public async Task<ServicePlanModel> GetPlanWithSubscriptionsAsync(int planId)
        {
            return await _context.ServicePlans
                .Include(sp => sp.UserSubscriptions)
                    .ThenInclude(us => us.User)
                .FirstOrDefaultAsync(sp => sp.Id == planId && !sp.IsDeleted);
        }

        public async Task<bool> IsPlanCodeUniqueAsync(string code, int? excludeId = null)
        {
            var query = _context.ServicePlans.Where(sp => sp.Code == code && !sp.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(sp => sp.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<decimal> GetDiscountedPriceAsync(int planId, bool isYearly)
        {
            var plan = await GetByIdAsync(planId);
            if (plan == null) return 0;

            var basePrice = isYearly ? plan.YearlyPrice : plan.MonthlyPrice;

            if (plan.DiscountPercentage.HasValue)
            {
                return basePrice * (1 - plan.DiscountPercentage.Value / 100);
            }

            return basePrice;
        }

        public async Task<int> GetActiveSubscriptionCountAsync(int planId)
        {
            return await _context.UserServicePlanSubscriptions
                .CountAsync(us => us.ServicePlanId == planId
                    && us.IsActive
                    && !us.IsDeleted);
        }
    }
}