using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IUserRepo : IRepo<UserModel>
    {
        // Authentication & User lookup
        Task<UserModel?> GetByEmailAsync(string email);
        Task<UserModel?> GetByEmailWithEventsAsync(string email);
        Task<UserModel?> GetByIdWithTicketsAsync(int id);
        Task<UserModel?> GetByIdWithFullDetailsAsync(int id); // Include all navigation properties

        // Validation
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsPhoneExistsAsync(string phone);

        // Role-based queries
        Task<IEnumerable<UserModel>> GetUsersByRoleAsync(UserRoles role);
        Task<IEnumerable<UserModel>> GetActiveOrganizersAsync();
        Task<IEnumerable<UserModel>> GetPartneredOrganizersAsync();
        Task<IEnumerable<UserModel>> GetStaffUsersAsync(); // All staff roles

        // Service plan queries
        Task<IEnumerable<UserModel>> GetUsersWithExpiredPlansAsync();
        Task<IEnumerable<UserModel>> GetUsersByServicePlanAsync(string servicePlan);

        // Status queries
        Task<IEnumerable<UserModel>> GetInactiveUsersAsync();
        Task<IEnumerable<UserModel>> GetUnconfirmedEmailUsersAsync();

        // Statistics counts -- not use now
        Task<int> CountUsersByRoleAsync(UserRoles role);
        Task<int> CountActiveUsersAsync();
        Task<int> CountOrganizersWithActivePlanAsync();

        //refresh token
        Task<UserModel?> GetByRefreshTokenAsync(string refreshToken);
        Task<bool> UpdateRefreshTokenAsync(int userId, string? refreshToken, DateTime? expiry);
    }
}