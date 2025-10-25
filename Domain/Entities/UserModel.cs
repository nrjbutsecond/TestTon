using Domain.common;
using Domain.Entities.MerchandiseEntity;
using Domain.Entities.ServicePlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserModel :BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // use cho authentication
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public UserRoles Role { get; set; } = UserRoles.Enthusiast;
        public string ServicePlan { get; set; } = ServicePlans.Free; //will change to serivcePlanModel soon
        public DateTime? ServicePlanExpiry { get; set; }
        public bool IsPartneredOrganizer { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public string? RefreshToken { get; set; } // for JWT auth

        public DateTime? RefreshTokenExpiry { get; set; } // Default expiry for refresh token
        // Email confirmation
        public bool EmailConfirmed { get; set; } = false;
        public string? EmailConfirmationToken { get; set; }
        public DateTime? EmailConfirmationTokenExpiry { get; set; }
        //

        //Password reset fields(update soon :v)
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }

        // Navigation properties cho MVP
          public virtual ICollection<TalkEventModel> OrganizedEvents { get; set; } = new List<TalkEventModel>();
          public virtual ICollection<TicketModel> PurchasedTickets { get; set; } = new List<TicketModel>();
        public virtual ICollection<ArticleModel> Articles { get; set; } = new List<ArticleModel>();
        public virtual ICollection<Merchandise> Merchandises { get; set; } = new List<Merchandise>();

        //serivce plan
        public virtual ICollection<UserServicePlanSubscriptionModel> ServicePlanSubscriptions { get; set; } = new List<UserServicePlanSubscriptionModel>();
        public virtual ICollection<ContractNegotiationModel> ContractNegotiations { get; set; } = new List<ContractNegotiationModel>();
    }

    // Constants  Roles and Plans


    public enum UserRoles
    {
        Enthusiast,
        Organizer,
        CommunityStaff,
        MentoringStaff,
        SalesStaff,
        Admin
    }

    public static class ServicePlans
    {
        public const string Free = "Free";
        public const string Premium = "Premium";
        public const string Full = "Full";
        
    }
}