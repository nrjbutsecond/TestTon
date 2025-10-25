using Domain.Entities.ServicePlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.ServicePlan
{
    public interface IUserServicePlanSubscriptionRepo : IRepo<UserServicePlanSubscriptionModel>
    {
        Task<UserServicePlanSubscriptionModel> GetActiveSubscriptionAsync(int userId);
        Task<IEnumerable<UserServicePlanSubscriptionModel>> GetUserSubscriptionHistoryAsync(int userId);
        Task<IEnumerable<UserServicePlanSubscriptionModel>> GetExpiringSubscriptionsAsync(int daysBeforeExpiry);
        Task<bool> HasActiveSubscriptionAsync(int userId);
        Task<UserServicePlanSubscriptionModel> GetSubscriptionWithPlanAsync(int subscriptionId);
        Task<IEnumerable<UserServicePlanSubscriptionModel>> GetSubscriptionsByPlanAsync(int planId);
        Task<decimal> CalculateProRatedRefundAsync(int subscriptionId);
    }
}
