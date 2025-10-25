using Application.DTOs.Common;
using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ServicePlan
{
    public class ServicePlanDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal MonthlyPrice { get; set; }
        public decimal YearlyPrice { get; set; }
        public int MaxEvents { get; set; }
        public int MaxMerchandiseItems { get; set; }
        public bool IncludesConsultation { get; set; }
        public bool IncludesPersonnelSupport { get; set; }
        public int ConsultationHours { get; set; }
        public List<string> Features { get; set; }
        public bool IsActive { get; set; }
        public bool IsPopular { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountedMonthlyPrice { get; set; }
        public decimal? DiscountedYearlyPrice { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class ServicePlanListDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public decimal MonthlyPrice { get; set; }
        public decimal YearlyPrice { get; set; }
        public bool IsPopular { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public int MaxEvents { get; set; }
        public int MaxMerchandiseItems { get; set; }
    }

    public class CreateServicePlanDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal MonthlyPrice { get; set; }
        public decimal YearlyPrice { get; set; }
        public int MaxEvents { get; set; }
        public int MaxMerchandiseItems { get; set; }
        public bool IncludesConsultation { get; set; }
        public bool IncludesPersonnelSupport { get; set; }
        public int ConsultationHours { get; set; }
        public List<string> Features { get; set; }
        public bool IsActive { get; set; }
        public bool IsPopular { get; set; }
        public int DisplayOrder { get; set; }
        public decimal? DiscountPercentage { get; set; }
    }

    public class UpdateServicePlanDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal MonthlyPrice { get; set; }
        public decimal YearlyPrice { get; set; }
        public int MaxEvents { get; set; }
        public int MaxMerchandiseItems { get; set; }
        public bool IncludesConsultation { get; set; }
        public bool IncludesPersonnelSupport { get; set; }
        public int ConsultationHours { get; set; }
        public List<string> Features { get; set; }
        public bool IsActive { get; set; }
        public bool IsPopular { get; set; }
        public int DisplayOrder { get; set; }
        public decimal? DiscountPercentage { get; set; }
    }

    public class ServicePlanComparisonDto
    {
        public List<ServicePlanDto> Plans { get; set; }
        public List<FeatureComparisonItem> FeatureComparisons { get; set; }
    }

    public class FeatureComparisonItem
    {
        public string FeatureName { get; set; }
        public Dictionary<int, bool> PlanFeatures { get; set; } // Key: PlanId, Value: Has feature
    }

    // Subscription DTOs
    public class UserServicePlanSubscriptionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ServicePlanId { get; set; }
        public string ServicePlanName { get; set; }
        public string ServicePlanCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PaidAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public bool IsActive { get; set; }
        public bool AutoRenew { get; set; }
        public int DaysRemaining { get; set; }
        public bool IsExpired { get; set; }
    }

    public class CreateSubscriptionDto
    {
        public int UserId { get; set; }
        public int ServicePlanId { get; set; }
        public int DurationInMonths { get; set; }
        public string PaymentMethod { get; set; }
        public bool AutoRenew { get; set; }
        public int? ContractNegotiationId { get; set; }
    }

    public class UpdateSubscriptionDto
    {
        public bool AutoRenew { get; set; }
        public string PaymentMethod { get; set; }
    }

    // Contract Negotiation DTOs
    public class ContractNegotiationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int ServicePlanId { get; set; }
        public string ServicePlanName { get; set; }
        public string RequestType { get; set; }
        public string CurrentStatus { get; set; }
        public string Requirements { get; set; }
        public string ProposedTerms { get; set; }
        public string NegotiationNotes { get; set; }
        public decimal ProposedPrice { get; set; }
        public int ContractDuration { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string HandledBy { get; set; }
    }

    public class CreateContractNegotiationDto
    {
        public int ServicePlanId { get; set; }
        public string RequestType { get; set; }
        public string Requirements { get; set; }
        public int ContractDuration { get; set; }
        public decimal? ProposedPrice { get; set; }
    }

    public class UpdateContractNegotiationDto
    {
        public string CurrentStatus { get; set; }
        public string ProposedTerms { get; set; }
        public string NegotiationNotes { get; set; }
        public decimal ProposedPrice { get; set; }
        public string HandledBy { get; set; }
    }

    public class ServicePlanUpgradeDto
    {
        public int CurrentPlanId { get; set; }
        public int NewPlanId { get; set; }
        public decimal ProRatedAmount { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string UpgradeReason { get; set; }
    }
}