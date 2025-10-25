using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Helper;
using Domain.Entities;
namespace Application.DTOs.UserDtos
{
    public class UserDtos
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Role { get; set; } = string.Empty;
        public string ServicePlan { get; set; } = string.Empty;
        public DateTime? ServicePlanExpiry { get; set; }
        public bool IsPartneredOrganizer { get; set; }
        public bool IsActive { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Email is required")]
        [Email]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,100}$",
            ErrorMessage = "Password must contain at least one uppercase, one lowercase, one number and one special character")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string? Phone { get; set; }
    }

    public class UpdateUserDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string? Phone { get; set; }
    }

    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [Email]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;
    }

  

    //
    public class UpdateUserRoleDto
    {
        [Required(ErrorMessage = "Role is required")]
        [EnumDataType(typeof(UserRoles), ErrorMessage = "Invalid role value")]
        public UserRoles Role { get; set; }
    }

    public class UpdateUserServicePlanDto
    {
        [Required(ErrorMessage = "Service plan is required")]
        [RegularExpression("^(Free|Premium|Full)$", ErrorMessage = "Invalid service plan")]
        public string ServicePlan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Service plan expiry is required")]
        public DateTime ServicePlanExpiry { get; set; }

        public bool IsPartneredOrganizer { get; set; }
    }

  
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public UserDtos User { get; set; } = new UserDtos();
    }

    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,100}$",
            ErrorMessage = "Password must contain at least one uppercase, one lowercase, one number and one special character")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
    public class ResendConfirmationDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
    }
    //Service plan haven't done
    /*
    public class UpdateUserServicePlanDto
    {
        [Required(ErrorMessage = "Service plan is required")]
        [RegularExpression("^(Free|Premium)$", ErrorMessage = "Invalid service plan")]
        public string ServicePlan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Service plan expiry is required")]
        public DateTime ServicePlanExpiry { get; set; }

        public bool IsPartneredOrganizer { get; set; }
    }
    */

    //refresh token

    public class RefreshTokenDto
    {
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class TokenResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}