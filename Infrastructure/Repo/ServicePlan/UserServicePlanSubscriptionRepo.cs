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
    public class UserServicePlanSubscriptionRepo : Repo<UserServicePlanSubscriptionModel>, IUserServicePlanSubscriptionRepo
    {
        public UserServicePlanSubscriptionRepo(AppDbContext context) : base(context) { }
        public async Task<UserServicePlanSubscriptionModel> GetActiveSubscriptionAsync(int userId)
        {
            return await _context.UserServicePlanSubscriptions
                .Include(us => us.ServicePlan)
                .FirstOrDefaultAsync(us => us.UserId == userId
                    && us.IsActive
                    && us.EndDate > DateTime.UtcNow
                    && !us.IsDeleted);
        }

        public async Task<IEnumerable<UserServicePlanSubscriptionModel>> GetUserSubscriptionHistoryAsync(int userId)
        {
            return await _context.UserServicePlanSubscriptions
                .Include(us => us.ServicePlan)
                .Where(us => us.UserId == userId && !us.IsDeleted)
                .OrderByDescending(us => us.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserServicePlanSubscriptionModel>> GetExpiringSubscriptionsAsync(int daysBeforeExpiry)
        {
            var targetDate = DateTime.UtcNow.AddDays(daysBeforeExpiry);

            return await _context.UserServicePlanSubscriptions
                .Include(us => us.ServicePlan)
                .Include(us => us.User)
                .Where(us => us.IsActive
                    && us.EndDate <= targetDate
                    && us.EndDate > DateTime.UtcNow
                    && !us.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> HasActiveSubscriptionAsync(int userId)
        {
            return await _context.UserServicePlanSubscriptions
                .AnyAsync(us => us.UserId == userId
                    && us.IsActive
                    && us.EndDate > DateTime.UtcNow
                    && !us.IsDeleted);
        }

        public async Task<UserServicePlanSubscriptionModel> GetSubscriptionWithPlanAsync(int subscriptionId)
        {
            return await _context.UserServicePlanSubscriptions
                .Include(us => us.ServicePlan)
                .Include(us => us.User)
                .FirstOrDefaultAsync(us => us.Id == subscriptionId && !us.IsDeleted);
        }

        public async Task<IEnumerable<UserServicePlanSubscriptionModel>> GetSubscriptionsByPlanAsync(int planId)
        {
            return await _context.UserServicePlanSubscriptions
                .Include(us => us.User)
                .Where(us => us.ServicePlanId == planId && us.IsActive && !us.IsDeleted)
                .ToListAsync();
        }

        public async Task<decimal> CalculateProRatedRefundAsync(int subscriptionId)
        {
            var subscription = await GetByIdAsync(subscriptionId);
            if (subscription == null || !subscription.IsActive) return 0;

            var totalDays = (decimal)(subscription.EndDate - subscription.StartDate).TotalDays;
            var usedDays = (decimal)(DateTime.UtcNow - subscription.StartDate).TotalDays;
            var remainingDays = totalDays - usedDays;

            if (remainingDays <= 0) return 0;

            return (decimal)(subscription.PaidAmount * (remainingDays / totalDays));
        }
    }
}