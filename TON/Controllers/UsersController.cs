using Application.DTOs.UserDtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TON.Services;

namespace TON.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserService userService,
            IJwtService jwtService,
            IMapper mapper,
            ILogger<UsersController> logger)
        {
            _userService = userService;
            _jwtService = jwtService;
            _mapper = mapper;
            _logger = logger;
        }

        #region Authentication Endpoints

        /// <summary>
        /// User login
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                //  Authenticate user (business logic)
                var user = await _userService.AuthenticateAsync(loginDto);

                //  Generate JWT token 
                var token = _jwtService.GenerateToken(user);

                //Generate refresh token if RememberMe is true
                string? refreshToken = null;
                if (loginDto.RememberMe)
                {
                    refreshToken = _jwtService.GenerateRefreshToken();

                    // Save refresh token with appropriate expiry
                    var refreshTokenExpiry = DateTime.UtcNow.AddDays(30); // 30 days for Remember Me

                    var saved = await _userService.SaveRefreshTokenAsync(
                        user.Id,
                        refreshToken,
                        refreshTokenExpiry
                    );

                    if (!saved)
                    {
                        _logger.LogWarning($"Failed to save refresh token for user {user.Id}");
                        refreshToken = null; // Don't return token if save failed 
                    }
                }

                //  Map to DTO and return
                var userDto = _mapper.Map<UserDtos>(user);

                return Ok(new LoginResponseDto
                {
                    Token = token,
                    RefreshToken = refreshToken, //can be null if not RememberMe
                    User = userDto
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        /// <summary>
        /// Register new user
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDtos>> Register([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                var result = await _userService.RegisterAsync(createUserDto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");

                return StatusCode(500, new { message = "An error occurred during registration" });
            }
        }

        /// <summary>
        /// Confirm email address
        /// </summary>
         [AllowAnonymous]
        [HttpGet("confirm-email")]
        
        public async Task<ActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            try
            {
                var result = await _userService.ConfirmEmailAsync(email, token);
                if (result)
                    return Ok(new { message = "Email confirmed successfully" });

                return BadRequest(new { message = "Invalid confirmation link" });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming email");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// Change password
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var result = await _userService.ChangePasswordAsync(userId, changePasswordDto);

                if (result)
                    return Ok(new { message = "Password changed successfully" });

                return BadRequest(new { message = "Failed to change password" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }


        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenDto request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.RefreshToken))
                {
                    return BadRequest(new { message = "Refresh token is required" });
                }

                // Validate refresh token and get user
                var user = await _userService.ValidateRefreshTokenAsync(request.RefreshToken);
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid or expired refresh token" });
                }

                // Generate new JWT token
                var newToken = _jwtService.GenerateToken(user);

                // Generate new refresh token (token rotation for security)
                //  var newRefreshToken = _jwtService.GenerateRefreshToken();

                // Save new refresh token
                //var refreshTokenExpiry = DateTime.UtcNow.AddDays(30);
                return Ok(new TokenResponseDto
                {
                    Token = newToken,
                    RefreshToken = request.RefreshToken  // Trả lại token cũ
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return StatusCode(500, new { message = "An error occurred while refreshing token" });
            }
        }

        /// <summary>
        /// Logout - for users who didn't use Remember Me
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                // Revoke any existing refresh token
                await _userService.RevokeRefreshTokenAsync(userId);

                return Ok(new
                {
                    message = "Logged out successfully",
                    instruction = "Please remove JWT token from client storage"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                // Still return success - client should clear tokens regardless
                return Ok(new { message = "Logged out successfully" });
            }
        }

        /// <summary>
        /// Logout with refresh token - for users who used Remember Me
        /// </summary>
        [HttpPost("logout-with-token")]
        [AllowAnonymous] // Allow because JWT might be expired
        public async Task<ActionResult> LogoutWithRefreshToken([FromBody] LogoutDto logoutDto)
        {
            try
            {
                if (string.IsNullOrEmpty(logoutDto?.RefreshToken))
                {
                    return BadRequest(new { message = "Refresh token is required" });
                }

                // Revoke the specific refresh token
                var revoked = await _userService.RevokeSpecificRefreshTokenAsync(logoutDto.RefreshToken);

                if (!revoked)
                {
                    // Token might be invalid or expired - still return success
                    _logger.LogWarning("Attempted to revoke invalid refresh token");
                }

                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout with token");
                // Still return success - client should clear tokens regardless
                return Ok(new { message = "Logged out successfully" });
            }
        }
    
    public class LogoutDto
        {
            public string? RefreshToken { get; set; }
        }

        #endregion

        #region User Management Endpoints

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<UserDtos>> GetProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var user = await _userService.GetByIdAsync(userId);

                if (user == null)
                    return NotFound();

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// Update current user profile
        /// </summary>
        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult<UserDtos>> UpdateProfile([FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var result = await _userService.UpdateAsync(userId, updateUserDto);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// Get user by ID (Admin only)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDtos>> GetById(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                    return NotFound();

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDtos>>> GetAll()
        {
            try
            {
                var users = await _userService.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// Get users by role (Admin only)
        /// </summary>
        [HttpGet("role/{role}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDtos>>> GetByRole(UserRoles role)
        {
            try
            {
                var users = await _userService.GetUsersByRoleAsync(role);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by role");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// Delete user (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _userService.DeleteAsync(id);
                if (result)
                    return NoContent();

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        #endregion

        #region Admin Management Endpoints

        /// <summary>
        /// Update user role (Admin only)
        /// </summary>
        [HttpPut("{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDtos>> UpdateRole(int id, [FromBody] UpdateUserRoleDto updateRoleDto)
        {
            try
            {
                var result = await _userService.UpdateUserRoleAsync(id, updateRoleDto);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user role");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// Update user service plan (Admin only)
        /// </summary>
        [HttpPut("{id}/service-plan")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDtos>> UpdateServicePlan(int id, [FromBody] UpdateUserServicePlanDto updatePlanDto)
        {
            try
            {
                var result = await _userService.UpdateServicePlanAsync(id, updatePlanDto);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating service plan");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// Activate user (Admin only)
        /// </summary>
        [HttpPut("{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ActivateUser(int id)
        {
            try
            {
                var result = await _userService.ActivateUserAsync(id);
                if (result)
                    return Ok(new { message = "User activated successfully" });

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// Deactivate user (Admin only)
        /// </summary>
        [HttpPut("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeactivateUser(int id)
        {
            try
            {
                var result = await _userService.DeactivateUserAsync(id);
                if (result)
                    return Ok(new { message = "User deactivated successfully" });

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        #endregion

        #region Validation Endpoints

        /// <summary>
        /// Check if email is available
        /// </summary>
        [HttpGet("check-email")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> CheckEmailAvailable([FromQuery] string email)
        {
            try
            {
                var isAvailable = await _userService.IsEmailAvailableAsync(email);
                return Ok(new { available = isAvailable });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email availability");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// Check if user can organize events
        /// </summary>
        [HttpGet("{id}/can-organize-events")]
        [Authorize]
        public async Task<ActionResult<bool>> CanOrganizeEvents(int id)
        {
            try
            {
                // Check if user is checking their own permission or is admin
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var isAdmin = User.IsInRole("Admin");

                if (currentUserId != id && !isAdmin)
                    return Forbid();

                var canOrganize = await _userService.CanUserOrganizeEventsAsync(id);
                return Ok(new { canOrganize });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking organize permission");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }
        /// <summary>
        /// Resend email confirmation
        /// </summary>
        [HttpPost("resend-confirmation")]
        [AllowAnonymous]
        public async Task<ActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationDto dto)
        {
            try
            {
                var result = await _userService.ResendConfirmationEmailAsync(dto.Email);
                if (result)
                    return Ok(new { message = "Confirmation email sent successfully" });

                return BadRequest(new { message = "Unable to send confirmation email. Email may be already confirmed or not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending confirmation email");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        #endregion
    }
}