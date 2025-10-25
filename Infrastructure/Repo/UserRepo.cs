using Domain.Entities;
using Domain.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;

namespace Infrastructure.Repo
{
    public class UserRepo : Repo<UserModel>, IUserRepo
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }

        // Authentication & User lookup
        public async Task<UserModel?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Where(u => u.Email == email && !u.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<UserModel?> GetByEmailWithEventsAsync(string email)
        {
            return await _context.Users
                .Include(u => u.OrganizedEvents) 
                .Where(u => u.Email == email && !u.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<UserModel?> GetByIdWithTicketsAsync(int id)
        {
            return await _context.Users
                .Include(u => u.PurchasedTickets) 
                .Where(u => u.Id == id && !u.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<UserModel?> GetByIdWithFullDetailsAsync(int id)
        {
            return await _context.Users
                .Include(u => u.OrganizedEvents)
                .Include(u => u.PurchasedTickets)
                .Include(u => u.Articles)
                .Where(u => u.Id == id && !u.IsDeleted)
                .FirstOrDefaultAsync();
        }

        // Validation
            public async Task<bool> IsEmailExistsAsync(string email)
            {
                return await _context.Users
        .AnyAsync(u => u.Email == email);
        }

        public async Task<bool> IsPhoneExistsAsync(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return false;

            return await _context.Users
                .AnyAsync(u => u.Phone == phone && !u.IsDeleted);
        }

        // Role-based queries
        public async Task<IEnumerable<UserModel>> GetUsersByRoleAsync(UserRoles role)
        {
            return await _context.Users
                .Where(u => u.Role == role && !u.IsDeleted)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserModel>> GetActiveOrganizersAsync()
        {
            return await _context.Users
                .Where(u => u.Role == UserRoles.Organizer
                    && u.IsActive
                    && u.ServicePlan != ServicePlans.Free
                    && u.ServicePlanExpiry > DateTime.UtcNow
                    && !u.IsDeleted)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserModel>> GetPartneredOrganizersAsync()
        {
            return await _context.Users
                .Where(u => u.Role == UserRoles.Organizer
                    && u.IsPartneredOrganizer
                    && u.IsActive
                    && !u.IsDeleted)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserModel>> GetStaffUsersAsync()
        {
            var staffRoles = new[] {
                UserRoles.CommunityStaff,
                UserRoles.MentoringStaff,
                UserRoles.SalesStaff,
                UserRoles.Admin
            };

            return await _context.Users
                .Where(u => staffRoles.Contains(u.Role)
                    && u.IsActive
                    && !u.IsDeleted)
                .OrderBy(u => u.Role)
                .ThenBy(u => u.FullName)
                .ToListAsync();
        }

        // Service plan queries
        public async Task<IEnumerable<UserModel>> GetUsersWithExpiredPlansAsync()
        {
            return await _context.Users
                .Where(u => u.ServicePlan != ServicePlans.Free
                    && u.ServicePlanExpiry < DateTime.UtcNow
                    && !u.IsDeleted)
                .OrderBy(u => u.ServicePlanExpiry)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserModel>> GetUsersByServicePlanAsync(string servicePlan)
        {
            return await _context.Users
                .Where(u => u.ServicePlan == servicePlan && !u.IsDeleted)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        // Status queries
        public async Task<IEnumerable<UserModel>> GetInactiveUsersAsync()
        {
            return await _context.Users
                .Where(u => !u.IsActive && !u.IsDeleted)
                .OrderBy(u => u.UpdatedAt ?? u.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserModel>> GetUnconfirmedEmailUsersAsync()
        {
            return await _context.Users
                .Where(u => !u.EmailConfirmed && !u.IsDeleted)
                .OrderBy(u => u.CreatedAt)
                .ToListAsync();
        }

        // Statistics
        public async Task<int> CountUsersByRoleAsync(UserRoles role) //why am i create this???
        {
            return await _context.Users
                .CountAsync(u => u.Role == role && u.IsActive && !u.IsDeleted);
        }

        public async Task<int> CountActiveUsersAsync()
        {
            return await _context.Users
                .CountAsync(u => u.IsActive && u.EmailConfirmed && !u.IsDeleted);
        }

        public async Task<int> CountOrganizersWithActivePlanAsync()
        {
            return await _context.Users
                .CountAsync(u => u.Role == UserRoles.Organizer
                    && u.ServicePlan != ServicePlans.Free
                    && u.ServicePlanExpiry > DateTime.UtcNow
                    && u.IsActive
                    && !u.IsDeleted);
        }

        //refresh token
        public async Task<UserModel?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users
                .Where(u => u.RefreshToken == refreshToken
                    && u.RefreshTokenExpiry > DateTime.UtcNow
                    && u.IsActive
                    && !u.IsDeleted)
                .FirstOrDefaultAsync();
        }

        // Update refresh token
        public async Task<bool> UpdateRefreshTokenAsync(int userId, string? refreshToken, DateTime? expiry)
        {
            var user = await GetByIdAsync(userId);
            if (user == null) return false;

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = expiry ?? DateTime.MinValue; //DateTime min = logout with refresh token
            user.UpdatedAt = DateTime.UtcNow;

            
            _context.Users.Update(user);

            return true;
        }
    }
}