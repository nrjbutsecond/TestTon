using Application.DTOs.ServicePlan;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities.ServicePlan;
using Domain.Interface;
using Domain.Interface.ServicePlan;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class ServicePlanService : IServicePlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ServicePlanService> _logger;
        private readonly IServicePlanRepo _servicePlanRepo;
        private readonly IUserServicePlanSubscriptionRepo _subscriptionRepo;
        private readonly IContractNegotiationRepo _negotiationRepo;

        public ServicePlanService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ServicePlanService> logger,
            IServicePlanRepo servicePlanRepo,
            IUserServicePlanSubscriptionRepo subscriptionRepo,
            IContractNegotiationRepo negotiationRepo
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _servicePlanRepo = servicePlanRepo;
            _subscriptionRepo = subscriptionRepo;
            _negotiationRepo = negotiationRepo;


        }

        // Service Plan operations
        public async Task<ServicePlanDto> GetServicePlanByIdAsync(int id)
        {
            var plan = await _servicePlanRepo.GetByIdAsync(id);
            if (plan == null) return null;

            var dto = _mapper.Map<ServicePlanDto>(plan);

            // Calculate discounted prices if applicable
            if (plan.DiscountPercentage.HasValue)
            {
                dto.DiscountedMonthlyPrice = plan.MonthlyPrice * (1 - plan.DiscountPercentage.Value / 100);
                dto.DiscountedYearlyPrice = plan.YearlyPrice * (1 - plan.DiscountPercentage.Value / 100);
            }

            // Get ratings (mock data for now)
            dto.Rating = 4.5;
            dto.ReviewCount = await _servicePlanRepo.GetActiveSubscriptionCountAsync(id);

            return dto;
        }

        public async Task<ServicePlanDto> GetServicePlanByCodeAsync(string code)
        {
            var plan = await _servicePlanRepo.GetByCodeAsync(code);
            if (plan == null) return null;

            return await GetServicePlanByIdAsync(plan.Id);
        }

        public async Task<IEnumerable<ServicePlanListDto>> GetAllServicePlansAsync()
        {
            var plans = await _servicePlanRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<ServicePlanListDto>>(plans.Where(p => !p.IsDeleted));
        }

        public async Task<IEnumerable<ServicePlanListDto>> GetActiveServicePlansAsync()
        {
            var plans = await _servicePlanRepo.GetActivePlansAsync();
            var dtos = _mapper.Map<IEnumerable<ServicePlanListDto>>(plans);

            // Add ratings for each plan
            foreach (var dto in dtos)
            {
                dto.Rating = 4.5; // Mock rating
                dto.ReviewCount = await _servicePlanRepo.GetActiveSubscriptionCountAsync(dto.Id);
            }

            return dtos;
        }

        public async Task<IEnumerable<ServicePlanListDto>> GetPopularServicePlansAsync()
        {
            var plans = await _servicePlanRepo.GetPopularPlansAsync();
            return _mapper.Map<IEnumerable<ServicePlanListDto>>(plans);
        }

        public async Task<ServicePlanComparisonDto> GetServicePlanComparisonAsync()
        {
            var plans = await _servicePlanRepo.GetActivePlansAsync();
            var planDtos = _mapper.Map<List<ServicePlanDto>>(plans);

            var allFeatures = new HashSet<string>();
            foreach (var plan in planDtos)
            {
                if (plan.Features != null)
                {
                    foreach (var feature in plan.Features)
                    {
                        allFeatures.Add(feature);
                    }
                }
            }

            var featureComparisons = new List<FeatureComparisonItem>();
            foreach (var feature in allFeatures)
            {
                var item = new FeatureComparisonItem
                {
                    FeatureName = feature,
                    PlanFeatures = new Dictionary<int, bool>()
                };

                foreach (var plan in planDtos)
                {
                    item.PlanFeatures[plan.Id] = plan.Features?.Contains(feature) ?? false;
                }

                featureComparisons.Add(item);
            }

            return new ServicePlanComparisonDto
            {
                Plans = planDtos,
                FeatureComparisons = featureComparisons
            };
        }

        public async Task<ServicePlanDto> CreateServicePlanAsync(CreateServicePlanDto dto)
        {
            // Validate unique code
            if (!await _servicePlanRepo.IsPlanCodeUniqueAsync(dto.Code))
            {
                throw new InvalidOperationException($"Service plan with code '{dto.Code}' already exists.");
            }

            var plan = _mapper.Map<ServicePlanModel>(dto);
            plan.Features = JsonConvert.SerializeObject(dto.Features ?? new List<string>());
            plan.CreatedAt = DateTime.UtcNow;

            await _servicePlanRepo.AddAsync(plan);
            await _unitOfWork.SaveChangesAsync();

            return await GetServicePlanByIdAsync(plan.Id);
        }

        public async Task<ServicePlanDto> UpdateServicePlanAsync(int id, UpdateServicePlanDto dto)
        {
            var plan = await _servicePlanRepo.GetByIdAsync(id);
            if (plan == null)
            {
                throw new KeyNotFoundException($"Service plan with ID {id} not found.");
            }

            _mapper.Map(dto, plan);
            plan.Features = JsonConvert.SerializeObject(dto.Features ?? new List<string>());
            plan.UpdatedAt = DateTime.UtcNow;

            _servicePlanRepo.Update(plan);
            await _unitOfWork.SaveChangesAsync();

            return await GetServicePlanByIdAsync(plan.Id);
        }

        public async Task<bool> DeleteServicePlanAsync(int id)
        {
            var plan = await _servicePlanRepo.GetByIdAsync(id);
            if (plan == null) return false;

            // Check for active subscriptions
            var activeCount = await _servicePlanRepo.GetActiveSubscriptionCountAsync(id);
            if (activeCount > 0)
            {
                throw new InvalidOperationException($"Cannot delete plan with {activeCount} active subscriptions.");
            }

            plan.IsDeleted = true;
            plan.DeletedAt = DateTime.UtcNow;

            _servicePlanRepo.Update(plan);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ActivateServicePlanAsync(int id)
        {
            var plan = await _servicePlanRepo.GetByIdAsync(id);
            if (plan == null) return false;

            plan.IsActive = true;
            plan.UpdatedAt = DateTime.UtcNow;

            _servicePlanRepo.Update(plan);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeactivateServicePlanAsync(int id)
        {
            var plan = await _servicePlanRepo.GetByIdAsync(id);
            if (plan == null) return false;

            plan.IsActive = false;
            plan.UpdatedAt = DateTime.UtcNow;

            _servicePlanRepo.Update(plan);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // Subscription operations
        public async Task<UserServicePlanSubscriptionDto> GetActiveSubscriptionAsync(int userId)
        {
            var subscription = await _subscriptionRepo.GetActiveSubscriptionAsync(userId);
            if (subscription == null) return null;

            var dto = _mapper.Map<UserServicePlanSubscriptionDto>(subscription);
            dto.DaysRemaining = (subscription.EndDate - DateTime.UtcNow).Days;
            dto.IsExpired = subscription.EndDate < DateTime.UtcNow;

            return dto;
        }

        public async Task<IEnumerable<UserServicePlanSubscriptionDto>> GetUserSubscriptionHistoryAsync(int userId)
        {
            var subscriptions = await _subscriptionRepo.GetUserSubscriptionHistoryAsync(userId);
            return _mapper.Map<IEnumerable<UserServicePlanSubscriptionDto>>(subscriptions);
        }

        public async Task<UserServicePlanSubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionDto dto)
        {
            // Check if user already has active subscription
            if (await _subscriptionRepo.HasActiveSubscriptionAsync(dto.UserId))
            {
                throw new InvalidOperationException("User already has an active subscription.");
            }

            var plan = await _servicePlanRepo.GetByIdAsync(dto.ServicePlanId);
            if (plan == null || !plan.IsActive)
            {
                throw new KeyNotFoundException("Service plan not found or inactive.");
            }

            var subscription = new UserServicePlanSubscriptionModel
            {
                UserId = dto.UserId,
                ServicePlanId = dto.ServicePlanId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(dto.DurationInMonths),
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = PaymentStatus.Pending.ToString(),
                IsActive = false,
                AutoRenew = dto.AutoRenew,
                ContractNegotiationId = dto.ContractNegotiationId,
                CreatedAt = DateTime.UtcNow
            };

            // Calculate payment amount
            var isYearly = dto.DurationInMonths >= 12;
            subscription.PaidAmount = await _servicePlanRepo.GetDiscountedPriceAsync(dto.ServicePlanId, isYearly);

            await _subscriptionRepo.AddAsync(subscription);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserServicePlanSubscriptionDto>(subscription);
        }

        public async Task<UserServicePlanSubscriptionDto> UpgradeSubscriptionAsync(int userId, ServicePlanUpgradeDto dto)
        {
            var currentSubscription = await _subscriptionRepo.GetActiveSubscriptionAsync(userId);
            if (currentSubscription == null)
            {
                throw new InvalidOperationException("No active subscription found.");
            }

            if (currentSubscription.ServicePlanId == dto.NewPlanId)
            {
                throw new InvalidOperationException("Already subscribed to this plan.");
            }

            // Calculate pro-rated refund
            var refundAmount = await _subscriptionRepo.CalculateProRatedRefundAsync(currentSubscription.Id);

            // Deactivate current subscription
            currentSubscription.IsActive = false;
            currentSubscription.EndDate = DateTime.UtcNow;
            currentSubscription.UpdatedAt = DateTime.UtcNow;
            _subscriptionRepo.Update(currentSubscription);

            // Create new subscription
            var remainingMonths = (int)Math.Ceiling((currentSubscription.EndDate - DateTime.UtcNow).TotalDays / 30);
            var createDto = new CreateSubscriptionDto
            {
                UserId = userId,
                ServicePlanId = dto.NewPlanId,
                DurationInMonths = remainingMonths,
                PaymentMethod = currentSubscription.PaymentMethod,
                AutoRenew = currentSubscription.AutoRenew
            };

            var newSubscription = await CreateSubscriptionAsync(createDto);

            // Apply refund credit
            var subscription = await _subscriptionRepo.GetByIdAsync(newSubscription.Id);
            subscription.PaidAmount -= refundAmount;
            subscription.PaymentStatus = PaymentStatus.Completed.ToString();
            subscription.IsActive = true;
            _subscriptionRepo.Update(subscription);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserServicePlanSubscriptionDto>(subscription);
        }

        public async Task<bool> CancelSubscriptionAsync(int subscriptionId)
        {
            var subscription = await _subscriptionRepo.GetByIdAsync(subscriptionId);
            if (subscription == null) return false;

            subscription.IsActive = false;
            subscription.AutoRenew = false;
            subscription.UpdatedAt = DateTime.UtcNow;

            _subscriptionRepo.Update(subscription);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RenewSubscriptionAsync(int subscriptionId)
        {
            var subscription = await _subscriptionRepo.GetSubscriptionWithPlanAsync(subscriptionId);
            if (subscription == null) return false;

            // Create renewal subscription
            var renewalSubscription = new UserServicePlanSubscriptionModel
            {
                UserId = subscription.UserId,
                ServicePlanId = subscription.ServicePlanId,
                StartDate = subscription.EndDate,
                EndDate = subscription.EndDate.AddMonths(12),
                PaymentMethod = subscription.PaymentMethod,
                PaymentStatus = PaymentStatus.Pending.ToString(),
                PaidAmount = subscription.ServicePlan.YearlyPrice,
                IsActive = false,
                AutoRenew = subscription.AutoRenew,
                CreatedAt = DateTime.UtcNow
            };

            await _subscriptionRepo.AddAsync(renewalSubscription);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<UserServicePlanSubscriptionDto> UpdateSubscriptionAsync(int id, UpdateSubscriptionDto dto)
        {
            var subscription = await _subscriptionRepo.GetByIdAsync(id);
            if (subscription == null)
            {
                throw new KeyNotFoundException($"Subscription with ID {id} not found.");
            }

            subscription.AutoRenew = dto.AutoRenew;
            subscription.PaymentMethod = dto.PaymentMethod;
            subscription.UpdatedAt = DateTime.UtcNow;

            _subscriptionRepo.Update(subscription);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserServicePlanSubscriptionDto>(subscription);
        }

        public async Task<IEnumerable<UserServicePlanSubscriptionDto>> GetExpiringSubscriptionsAsync(int days = 7)
        {
            var subscriptions = await _subscriptionRepo.GetExpiringSubscriptionsAsync(days);
            return _mapper.Map<IEnumerable<UserServicePlanSubscriptionDto>>(subscriptions);
        }

        // Contract Negotiation operations
        public async Task<ContractNegotiationDto> CreateContractNegotiationAsync(int userId, CreateContractNegotiationDto dto)
        {
            // Check for existing pending negotiation
            if (await _negotiationRepo.HasPendingNegotiationAsync(userId, dto.ServicePlanId))
            {
                throw new InvalidOperationException("You already have a pending negotiation for this plan.");
            }

            var negotiation = _mapper.Map<ContractNegotiationModel>(dto);
            negotiation.UserId = userId;
            negotiation.CurrentStatus = ContractStatus.Pending.ToString();
            negotiation.RequestDate = DateTime.UtcNow;
            negotiation.CreatedAt = DateTime.UtcNow;

            await _negotiationRepo.AddAsync(negotiation);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ContractNegotiationDto>(negotiation);
        }

        public async Task<IEnumerable<ContractNegotiationDto>> GetUserNegotiationsAsync(int userId)
        {
            var negotiations = await _negotiationRepo.GetUserNegotiationsAsync(userId);
            return _mapper.Map<IEnumerable<ContractNegotiationDto>>(negotiations);
        }

        public async Task<IEnumerable<ContractNegotiationDto>> GetPendingNegotiationsAsync()
        {
            var negotiations = await _negotiationRepo.GetPendingNegotiationsAsync();
            return _mapper.Map<IEnumerable<ContractNegotiationDto>>(negotiations);
        }

        public async Task<ContractNegotiationDto> UpdateNegotiationStatusAsync(int id, UpdateContractNegotiationDto dto)
        {
            var negotiation = await _negotiationRepo.GetByIdAsync(id);
            if (negotiation == null)
            {
                throw new KeyNotFoundException($"Contract negotiation with ID {id} not found.");
            }

            _mapper.Map(dto, negotiation);
            negotiation.ResponseDate = DateTime.UtcNow;
            negotiation.UpdatedAt = DateTime.UtcNow;

            _negotiationRepo.Update(negotiation);
            await _unitOfWork.SaveChangesAsync();
                
            return _mapper.Map<ContractNegotiationDto>(negotiation);
        }

        public async Task<ContractNegotiationDto> ApproveNegotiationAsync(int id, string approvedBy)
        {
            var negotiation = await _negotiationRepo.GetNegotiationWithDetailsAsync(id);
            if (negotiation == null)
            {
                throw new KeyNotFoundException($"Contract negotiation with ID {id} not found.");
            }

            negotiation.CurrentStatus = ContractStatus.Approved.ToString();
            negotiation.HandledBy = approvedBy;
            negotiation.ResponseDate = DateTime.UtcNow;
            negotiation.UpdatedAt = DateTime.UtcNow;

            _negotiationRepo.Update(negotiation);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ContractNegotiationDto>(negotiation);
        }

        public async Task<ContractNegotiationDto> RejectNegotiationAsync(int id, string rejectedBy, string reason)
        {
            var negotiation = await _negotiationRepo.GetByIdAsync(id);
            if (negotiation == null)
            {
                throw new KeyNotFoundException($"Contract negotiation with ID {id} not found.");
            }

            negotiation.CurrentStatus = ContractStatus.Rejected.ToString();
            negotiation.HandledBy = rejectedBy;
            negotiation.NegotiationNotes = reason;
            negotiation.ResponseDate = DateTime.UtcNow;
            negotiation.UpdatedAt = DateTime.UtcNow;

            _negotiationRepo.Update(negotiation);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ContractNegotiationDto>(negotiation);
        }

        // Validation and business rules
        public async Task<bool> CanUserUpgradeAsync(int userId, int newPlanId)
        {
            var currentSubscription = await _subscriptionRepo.GetActiveSubscriptionAsync(userId);
            if (currentSubscription == null) return false;

            var newPlan = await _servicePlanRepo.GetByIdAsync(newPlanId);
            if (newPlan == null || !newPlan.IsActive) return false;

            // Can only upgrade to higher tier plans
            var currentPlan = await _servicePlanRepo.GetByIdAsync(currentSubscription.ServicePlanId);
            return GetPlanTier(newPlan.Code) > GetPlanTier(currentPlan.Code);
        }

        public async Task<decimal> CalculateUpgradeCostAsync(int userId, int newPlanId)
        {
            var currentSubscription = await _subscriptionRepo.GetActiveSubscriptionAsync(userId);
            if (currentSubscription == null) return 0;

            var refund = await _subscriptionRepo.CalculateProRatedRefundAsync(currentSubscription.Id);
            var newPlan = await _servicePlanRepo.GetByIdAsync(newPlanId);

            var remainingMonths = (int)Math.Ceiling((currentSubscription.EndDate - DateTime.UtcNow).TotalDays / 30);
            var isYearly = remainingMonths >= 12;
            var newCost = await _servicePlanRepo.GetDiscountedPriceAsync(newPlanId, isYearly);

            return Math.Max(0, newCost - refund);
        }

        public async Task<bool> ValidateServicePlanLimitsAsync(int userId, string feature)
        {
            var subscription = await _subscriptionRepo.GetActiveSubscriptionAsync(userId);
            if (subscription == null) return false;

            var plan = subscription.ServicePlan;

            switch (feature.ToLower())
            {
                case "event":
                    // Check event limit - would need to count user's events
                    return true;

                case "merchandise":
                    // Check merchandise limit - would need to count user's items
                    return true;

                case "consultation":
                    return plan.IncludesConsultation;

                case "personnel":
                    return plan.IncludesPersonnelSupport;

                default:
                    // Check if feature is in the plan's feature list
                    var features = JsonConvert.DeserializeObject<List<string>>(plan.Features ?? "[]");
                    return features.Contains(feature);
            }
        }

        private int GetPlanTier(string code)
        {
            return code?.ToUpper() switch
            {
                "BASIC" => 1,
                "STANDARD" => 2,
                "PREMIUM" => 3,
                _ => 0
            };
        }
    }
}