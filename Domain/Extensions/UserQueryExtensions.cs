using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//
namespace Domain.Extensions
{
    public static class UserQueryExtensions
    {
        public static IQueryable<UserModel> Active(this IQueryable<UserModel> query)
        {
            return query.Where(u => u.IsActive && u.EmailConfirmed);
        }

        public static IQueryable<UserModel> ByRole(this IQueryable<UserModel> query, UserRoles role)
        {
            return query.Where(u => u.Role == role);
        }

        public static IQueryable<UserModel> WithActiveSubscription(this IQueryable<UserModel> query)
        {
            return query.Where(u => u.ServicePlan != ServicePlans.Free
                && u.ServicePlanExpiry > DateTime.UtcNow);
        }

        public static IQueryable<UserModel> Organizers(this IQueryable<UserModel> query)
        {
            return query.Where(u => u.Role == UserRoles.Organizer);
        }

        public static IQueryable<UserModel> Staff(this IQueryable<UserModel> query)
        {
            var staffRoles = new[] {
                UserRoles.CommunityStaff,
                UserRoles.MentoringStaff,
                UserRoles.SalesStaff,
                UserRoles.Admin
            };
            return query.Where(u => staffRoles.Contains(u.Role));
        }

        public static IQueryable<UserModel> SearchByNameOrEmail(this IQueryable<UserModel> query, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var lowerSearchTerm = searchTerm.ToLower();
            return query.Where(u =>
                u.FullName.ToLower().Contains(lowerSearchTerm) ||
                u.Email.ToLower().Contains(lowerSearchTerm));
        }
    }
}