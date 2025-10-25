using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Extensions
{

    public static class UserExtensions
    {// Permission checks
        public static bool CanOrganizeEvents(this UserModel user)
        {
            return user.Role == UserRoles.Organizer
                && user.IsActive
                && user.ServicePlan != ServicePlans.Free
                && user.ServicePlanExpiry > DateTime.UtcNow;
        }

        public static bool CanManageArticles(this UserModel user)
        {
            var allowedRoles = new[] { UserRoles.SalesStaff, UserRoles.Admin };
            return allowedRoles.Contains(user.Role) && user.IsActive;
        }

        public static bool CanManageWorkshops(this UserModel user)
        {
            return user.Role == UserRoles.Admin && user.IsActive;
        }

        public static bool CanManageUsers(this UserModel user)
        {
            return user.Role == UserRoles.Admin && user.IsActive;
        }

        public static bool CanManagePersonnelSupport(this UserModel user)
        {
            var allowedRoles = new[] { UserRoles.CommunityStaff, UserRoles.Admin };
            return allowedRoles.Contains(user.Role) && user.IsActive;
        }

        // Status checks
        public static bool HasActiveSubscription(this UserModel user)
        {
            return user.ServicePlan != ServicePlans.Free
                && user.ServicePlanExpiry > DateTime.UtcNow;
        }

        public static bool IsSubscriptionExpiring(this UserModel user, int daysThreshold = 7)
        {
            if (user.ServicePlan == ServicePlans.Free || !user.ServicePlanExpiry.HasValue)
                return false;

            var daysUntilExpiry = (user.ServicePlanExpiry.Value - DateTime.UtcNow).TotalDays;
            return daysUntilExpiry > 0 && daysUntilExpiry <= daysThreshold;
        }

        public static bool IsStaff(this UserModel user)
        {
            var staffRoles = new[] {
                UserRoles.CommunityStaff,
                UserRoles.MentoringStaff,
                UserRoles.SalesStaff,
                UserRoles.Admin
            };
            return staffRoles.Contains(user.Role);
        }

        // Display helpers
        public static string GetDisplayName(this UserModel user)
        {
            return string.IsNullOrWhiteSpace(user.FullName) ? user.Email : user.FullName;
        }

        public static string GetRoleDisplayName(this UserModel user)
        {
            return user.Role switch
            {
                UserRoles.Enthusiast => "Enthusiast Member",
                UserRoles.Organizer => user.IsPartneredOrganizer ? "Partnered Organizer" : "Individual Organizer",
                UserRoles.CommunityStaff => "Community Staff",
                UserRoles.MentoringStaff => "Mentoring Staff",
                UserRoles.SalesStaff => "Sales Staff",
                UserRoles.Admin => "Administrator",
                _ => user.Role.ToString()
            };
        }

        public static string GetServicePlanDisplayName(this UserModel user)
        {
            if (user.ServicePlan == ServicePlans.Free)
                return "TON's Community (Free)";

            if (user.ServicePlan == ServicePlans.Premium && user.IsPartneredOrganizer)
                return "TON's Services (Partnered)";

            return "TON's Services (Premium)";
        }
    }
}