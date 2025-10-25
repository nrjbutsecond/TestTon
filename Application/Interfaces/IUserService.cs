using Application.DTOs.UserDtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Application.Interfaces
{
    public interface IUserService
    { // Authentication
      
        Task<UserModel> AuthenticateAsync(LoginDto loginDto);
        Task<UserDtos> RegisterAsync(CreateUserDto createUserDto);
        Task<bool> ConfirmEmailAsync(string email, string token);
        Task<bool> ResendConfirmationEmailAsync(string email); // New method 
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);

        // User Management
        Task<UserDtos?> GetByIdAsync(int id);
        Task<UserDtos?> GetByEmailAsync(string email);
        Task<IEnumerable<UserDtos>> GetAllAsync();
        Task<IEnumerable<UserDtos>> GetUsersByRoleAsync(UserRoles role);
        Task<UserDtos> UpdateAsync(int id, UpdateUserDto updateUserDto);
        Task<bool> DeleteAsync(int id);

        // Role & Plan Management (Admin only)
        Task<UserDtos> UpdateUserRoleAsync(int id, UpdateUserRoleDto updateRoleDto);
        Task<UserDtos> UpdateServicePlanAsync(int id, UpdateUserServicePlanDto updatePlanDto);
        Task<bool> ActivateUserAsync(int id);
        Task<bool> DeactivateUserAsync(int id);

        // Validation
        Task<bool> IsEmailAvailableAsync(string email);
        Task<bool> CanUserOrganizeEventsAsync(int userId);

        //refresh token
        Task<bool> SaveRefreshTokenAsync(int userId, string refreshToken, DateTime expiry);
        Task<UserModel?> ValidateRefreshTokenAsync(string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(int userId);
        Task<bool> RevokeSpecificRefreshTokenAsync(string refreshToken);
    }
}