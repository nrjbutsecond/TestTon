using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.DTOs.Advertisement
{
    public class AdvertisementDto
    {
        public int Id { get; set; }
        public int AdvertiserId { get; set; }
        public string AdvertiserName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string BannerImageUrl { get; set; } = string.Empty;
        public string TargetUrl { get; set; } = string.Empty;
        public string AdType { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public int ViewCount { get; set; }
        public int ClickCount { get; set; }
        public decimal CostPerView { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingBudget { get; set; }
        public double ClickThroughRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string TargetAudience { get; set; } = "{}"; // Keep as JSON string
        public string Status { get; set; } = string.Empty;
        public bool IsExpired { get; set; }
        public bool IsScheduledToStart { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class AdvertisementListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string AdType { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public int ClickCount { get; set; }
        public double ClickThroughRate { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal TotalBudget { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateAdvertisementDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string BannerImageUrl { get; set; } = string.Empty;
        public string TargetUrl { get; set; } = string.Empty;
        public string AdType { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public decimal CostPerView { get; set; }
        public decimal TotalBudget { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? TargetAudience { get; set; } // JSON string
    }

    public class UpdateAdvertisementDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? BannerImageUrl { get; set; }
        public string? TargetUrl { get; set; }
        public string? AdType { get; set; }
        public string? Position { get; set; }
        public int? DisplayOrder { get; set; }
        public decimal? CostPerView { get; set; }
        public decimal? TotalBudget { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
        public string? TargetAudience { get; set; }
        public string? Status { get; set; }
    }

    public class AdvertisementStatisticsDto
    {
        public int TotalAds { get; set; }
        public int ActiveAds { get; set; }
        public int TotalViews { get; set; }
        public int TotalClicks { get; set; }
        public double AverageClickThroughRate { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal BudgetUtilization { get; set; }
    }
}