using Domain.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AdvertisementModel :BaseEntity
    {
        public int AdvertiserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string BannerImageUrl { get; set; } = string.Empty;
        public string TargetUrl { get; set; } = string.Empty;
        public AdType AdType { get; set; }
        public AdPosition Position { get; set; }
        public int DisplayOrder { get; set; }
        public int ViewCount { get; set; }
        public int ClickCount { get; set; }
        public decimal CostPerView { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal SpentAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string TargetAudience { get; set; } = "{}"; // JSON string
        public AdStatus Status { get; set; }

        // Navigation properties
        public virtual UserModel? Advertiser { get; set; }

        // Calculated properties
        [NotMapped]
        public decimal RemainingBudget => TotalBudget - SpentAmount;
        [NotMapped]
        public double ClickThroughRate => ViewCount > 0 ? (double)ClickCount / ViewCount * 100 : 0;
        [NotMapped]
        public bool IsExpired => EndDate < DateTime.UtcNow;
        [NotMapped]
        public bool IsScheduledToStart => StartDate > DateTime.UtcNow;
    }
    public enum AdType
    {
        Banner,
        Popup,
        Sidebar,
        Featured
    }

    public enum AdPosition
    {
        Header,
        Footer,
        Sidebar,
        Homepage
    }

    public enum AdStatus
    {
        Draft,
        Active,
        Paused,
        Completed,
        Expired
    }
}
