using Application.DTOs.UserDtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Ticket.Domain.Interface;
namespace Application.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserService> _logger; // Add this
        

        public UserService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IEmailService emailService,
            ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
        }

        // Authentication -  verify credentials and return user
        public async Task<UserModel> AuthenticateAsync(LoginDto loginDto)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email);
            if (user == null || !user.IsActive)
                throw new UnauthorizedAccessException("Invalid credentials");

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            if (!user.EmailConfirmed)
                throw new UnauthorizedAccessException("Please confirm your email first");

            return user; // Return full UserModel entity, JWT will generate in Controller
        }

        public async Task<UserDtos> RegisterAsync(CreateUserDto createUserDto)
        {
            try
            {
                var emailExists = await _unitOfWork.Users.IsEmailExistsAsync(createUserDto.Email);
                if (emailExists)
                    throw new InvalidOperationException("Email already exists");
            }
            catch (Exception ex)
            {
                throw new Exception($"Email check failed: {ex.Message}", ex);
            }

            // Generate confirmation token
            var confirmationToken = GenerateConfirmationToken();
            var user = new UserModel
            {
                Email = createUserDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
                FullName = createUserDto.FullName,
                Phone = createUserDto.Phone,
                Role = UserRoles.Enthusiast,
                ServicePlan = "Free",
                IsActive = true,
                EmailConfirmed = false,
                EmailConfirmationToken = confirmationToken,
                EmailConfirmationTokenExpiry = DateTime.UtcNow.AddHours(24), // 24 hour expiry
                CreatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Adding user to database");
            await _unitOfWork.Users.AddAsync(user);

            _logger.LogInformation("Saving changes");
            await _unitOfWork.SaveChangesAsync();

            // Send confirmation email 
            try
            {
                await _emailService.SendEmailConfirmationAsync(user.Email, user.FullName, confirmationToken);
                _logger.LogInformation($"Confirmation email sent to {user.Email}");
            }
            catch (Exception emailEx)
            {
                // Log the error 
                _logger.LogWarning($"Failed to send confirmation email to {user.Email}: {emailEx.Message}");
                
            }

            return _mapper.Map<UserDtos>(user);
        }

        public async Task<bool> ConfirmEmailAsync(string email, string token)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
                return false;

            // Check if already confirmed
            if (user.EmailConfirmed)
                return true;

            // Validate token
            if (user.EmailConfirmationToken != token)
                return false;

            // Check token expiry
            if (user.EmailConfirmationTokenExpiry < DateTime.UtcNow)
                return false;

            // Confirm email
            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null; // Clear token
            user.EmailConfirmationTokenExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        // resend confirmation email if needed

        public async Task<bool> ResendConfirmationEmailAsync(string email)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null || user.EmailConfirmed)
                return false;

            // Generate new token
            var confirmationToken = GenerateConfirmationToken();
            user.EmailConfirmationToken = confirmationToken;
            user.EmailConfirmationTokenExpiry = DateTime.UtcNow.AddHours(24);
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // Send email
            try
            {
                await _emailService.SendEmailConfirmationAsync(user.Email, user.FullName, confirmationToken);
                return true;
            }
            catch
            {
                return false;
            }
        }



        //


        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return false;

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Current password is incorrect");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // User Management
        public async Task<UserDtos?> GetByIdAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            return _mapper.Map<UserDtos>(user);
        }

        public async Task<UserDtos?> GetByEmailAsync(string email)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            return _mapper.Map<UserDtos>(user);
        }

        public async Task<IEnumerable<UserDtos>> GetAllAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDtos>>(users);
        }

        public async Task<IEnumerable<UserDtos>> GetUsersByRoleAsync(UserRoles role)
        {
            var users = await _unitOfWork.Users.GetUsersByRoleAsync(role);
            return _mapper.Map<IEnumerable<UserDtos>>(users);
        }

        public async Task<UserDtos> UpdateAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            user.FullName = updateUserDto.FullName;
            user.Phone = updateUserDto.Phone;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserDtos>(user);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return false;

            _unitOfWork.Users.Remove(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // Role & Plan Management
        public async Task<UserDtos> UpdateUserRoleAsync(int id, UpdateUserRoleDto updateRoleDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            var validRoles = new[] {
                UserRoles.Enthusiast, UserRoles.Organizer,
                UserRoles.CommunityStaff, UserRoles.MentoringStaff,
                UserRoles.SalesStaff, UserRoles.Admin
            };

            if (!Array.Exists(validRoles, r => r == updateRoleDto.Role))
                throw new ArgumentException("Invalid role");

            user.Role = updateRoleDto.Role;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserDtos>(user);
        }

        public async Task<UserDtos> UpdateServicePlanAsync(int id, UpdateUserServicePlanDto updatePlanDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            user.ServicePlan = updatePlanDto.ServicePlan;
            user.ServicePlanExpiry = updatePlanDto.ServicePlanExpiry;
            user.IsPartneredOrganizer = updatePlanDto.IsPartneredOrganizer;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserDtos>(user);
        }

        public async Task<bool> ActivateUserAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return false;

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeactivateUserAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // Validation
        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            return !await _unitOfWork.Users.IsEmailExistsAsync(email);
        }

        public async Task<bool> CanUserOrganizeEventsAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return false;

            return user.Role == UserRoles.Organizer
              //  && user.ServicePlan != ServicePlans.Free
               // && user.ServicePlanExpiry > DateTime.UtcNow
                && user.IsActive;
        }
        private string GenerateConfirmationToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        //refresh token 
        public async Task<bool> SaveRefreshTokenAsync(int userId, string refreshToken, DateTime expiry)
        {
            try
            {
                // Hash token for security
                var hashedToken = HashToken(refreshToken);

                var result = await _unitOfWork.Users.UpdateRefreshTokenAsync(userId, hashedToken, expiry);
                if (result)
                {

                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"Refresh token saved for user {userId}, expires at {expiry}");//

                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving refresh token");
                return false;
            }
        }

        // Validate refresh token
        public async Task<UserModel?> ValidateRefreshTokenAsync(string refreshToken)
        {
            try
            {
                if (string.IsNullOrEmpty(refreshToken))
                {
                    _logger.LogWarning("Refresh token validation failed - empty token");
                    return null;
                }

                // Hash token to compare with stored hash
                var hashedToken = HashToken(refreshToken);

                // Get user by refresh token
                var user = await _unitOfWork.Users.GetByRefreshTokenAsync(hashedToken);
                if (user == null)
                {
                    _logger.LogWarning("Token not found in database");
                    return null;
                }

                if (user.RefreshTokenExpiry <= DateTime.UtcNow)
                {
                    _logger.LogWarning($"Refresh token expired for user {user.Id}");
                    return null;
                }

                if (user == null)
                {
                    _logger.LogWarning("Refresh token validation failed - token not found");
                    return null;
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning($"Refresh token validation failed for user {user.Id} - user inactive");
                    return null;
                }

                if (!user.EmailConfirmed)
                {
                    _logger.LogWarning($"Refresh token validation failed for user {user.Id} - email not confirmed");
                    return null;
                }

                _logger.LogInformation($"Refresh token validated successfully for user {user.Id}");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating refresh token");
                return null;
            }
        }

        // Revoke token (logout without token)
        public async Task<bool> RevokeRefreshTokenAsync(int userId)
        {
            try
            {
                var result = await _unitOfWork.Users.UpdateRefreshTokenAsync(userId, null, null);
                if (result)
                {
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"Refresh token revoked for user {userId}");//
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking refresh token");
                _logger.LogError(ex, $"Error revoking refresh token for user {userId}");

                return false;
            }
        }
        // Revoke Specific Refresh Token (for logout with token)
        public async Task<bool> RevokeSpecificRefreshTokenAsync(string refreshToken)
        {
            try
            {
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return false;
                }

                // Validate token and get user
                var user = await ValidateRefreshTokenAsync(refreshToken);
                if (user == null)
                {
                    _logger.LogWarning("Cannot revoke refresh token - invalid token");
                    return false;
                }

                // Clear the refresh token
                var result = await _unitOfWork.Users.UpdateRefreshTokenAsync(user.Id, null, null);

                if (result)
                {
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"Specific refresh token revoked for user {user.Id}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking specific refresh token");
                return false;
            }
        }

        // Helper method to hash tokens
        private string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }


    }
}