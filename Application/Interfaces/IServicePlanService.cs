using Application.DTOs.ServicePlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IServicePlanService
    {
        // Service Plan operations
        Task<ServicePlanDto> GetServicePlanByIdAsync(int id);
        Task<ServicePlanDto> GetServicePlanByCodeAsync(string code);
        Task<IEnumerable<ServicePlanListDto>> GetAllServicePlansAsync();
        Task<IEnumerable<ServicePlanListDto>> GetActiveServicePlansAsync();
        Task<IEnumerable<ServicePlanListDto>> GetPopularServicePlansAsync();
        Task<ServicePlanComparisonDto> GetServicePlanComparisonAsync();
        Task<ServicePlanDto> CreateServicePlanAsync(CreateServicePlanDto dto);
        Task<ServicePlanDto> UpdateServicePlanAsync(int id, UpdateServicePlanDto dto);
        Task<bool> DeleteServicePlanAsync(int id);
        Task<bool> ActivateServicePlanAsync(int id);
        Task<bool> DeactivateServicePlanAsync(int id);

        // Subscription operations
        Task<UserServicePlanSubscriptionDto> GetActiveSubscriptionAsync(int userId);
        Task<IEnumerable<UserServicePlanSubscriptionDto>> GetUserSubscriptionHistoryAsync(int userId);
        Task<UserServicePlanSubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionDto dto);
        Task<UserServicePlanSubscriptionDto> UpgradeSubscriptionAsync(int userId, ServicePlanUpgradeDto dto);
        Task<bool> CancelSubscriptionAsync(int subscriptionId);
        Task<bool> RenewSubscriptionAsync(int subscriptionId);
        Task<UserServicePlanSubscriptionDto> UpdateSubscriptionAsync(int id, UpdateSubscriptionDto dto);
        Task<IEnumerable<UserServicePlanSubscriptionDto>> GetExpiringSubscriptionsAsync(int days = 7);

        // Contract Negotiation operations
        Task<ContractNegotiationDto> CreateContractNegotiationAsync(int userId, CreateContractNegotiationDto dto);
        Task<IEnumerable<ContractNegotiationDto>> GetUserNegotiationsAsync(int userId);
        Task<IEnumerable<ContractNegotiationDto>> GetPendingNegotiationsAsync();
        Task<ContractNegotiationDto> UpdateNegotiationStatusAsync(int id, UpdateContractNegotiationDto dto);
        Task<ContractNegotiationDto> ApproveNegotiationAsync(int id, string approvedBy);
        Task<ContractNegotiationDto> RejectNegotiationAsync(int id, string rejectedBy, string reason);

        // Validation and business rules
        Task<bool> CanUserUpgradeAsync(int userId, int newPlanId);
        Task<decimal> CalculateUpgradeCostAsync(int userId, int newPlanId);
        Task<bool> ValidateServicePlanLimitsAsync(int userId, string feature);
    }
}

