using Domain.Entities;
using Domain.Entities.ServicePlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.ServicePlan
{
    public interface IServicePlanRepo : IRepo<ServicePlanModel>
    {
        Task<ServicePlanModel> GetByCodeAsync(string code);
        Task<IEnumerable<ServicePlanModel>> GetActivePlansAsync();
        Task<IEnumerable<ServicePlanModel>> GetPopularPlansAsync();
        Task<ServicePlanModel> GetPlanWithSubscriptionsAsync(int planId);
        Task<bool> IsPlanCodeUniqueAsync(string code, int? excludeId = null);
        Task<decimal> GetDiscountedPriceAsync(int planId, bool isYearly);
        Task<int> GetActiveSubscriptionCountAsync(int planId);
    }
}
