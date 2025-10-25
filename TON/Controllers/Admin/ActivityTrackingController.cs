using Application.DTOs.UserLogTracking;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TON.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityTrackingController : ControllerBase
    {
        private readonly IActivityTrackingService _activityService;
        private readonly ILogger<ActivityTrackingController> _logger;

        public ActivityTrackingController(
            IActivityTrackingService activityService,
            ILogger<ActivityTrackingController> logger)
        {
            _activityService = activityService;
            _logger = logger;
        }

        /// <summary>
        /// Get activity status của 1 user (cho admin xem)
        /// </summary>
        [HttpGet("users/{userId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserActivityStatus(int userId)
        {
            try
            {
                var status = await _activityService.GetActivityStatusAsync(userId);
                return Ok(new { success = true, data = status });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "User not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting activity status for user {UserId}", userId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get activity status của nhiều users cùng lúc (bulk)
        /// </summary>
        [HttpPost("users/bulk-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBulkActivityStatus([FromBody] List<int> userIds)
        {
            try
            {
                var statuses = await _activityService.GetBulkActivityStatusAsync(userIds);
                return Ok(new { success = true, data = statuses });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bulk activity status");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get activity history của user
        /// </summary>
        [HttpGet("users/{userId}/history")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetActivityHistory(int userId, [FromQuery] int take = 20)
        {
            try
            {
                var history = await _activityService.GetActivityHistoryAsync(userId, take);
                return Ok(new { success = true, data = history });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "User not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting activity history for user {UserId}", userId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get số lượng users đang online
        /// </summary>
        [HttpGet("online-count")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOnlineCount()
        {
            try
            {
                var count = await _activityService.GetOnlineUsersCountAsync();
                return Ok(new { success = true, data = new { onlineCount = count } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting online users count");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Manual log activity (for testing)
        /// </summary>
        [HttpPost("log")]
        [Authorize]
        public async Task<IActionResult> LogActivity([FromBody] LogActivityRequest request)
        {
            try
            {
                // Lấy userId từ token
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                request.UserId = userId;
                await _activityService.LogActivityAsync(request);

                return Ok(new { success = true, message = "Activity logged" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging activity");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// End session (logout)
        /// </summary>
        [HttpPost("session/end")]
        [Authorize]
        public async Task<IActionResult> EndSession()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                await _activityService.EndSessionAsync(userId);
                return Ok(new { success = true, message = "Session ended" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ending session");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Cleanup old logs (admin only - có thể dùng background job)
        /// </summary>
        [HttpPost("cleanup")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CleanupOldLogs([FromQuery] int daysToKeep = 90)
        {
            try
            {
                await _activityService.CleanupOldLogsAsync(daysToKeep);
                return Ok(new { success = true, message = $"Cleaned up logs older than {daysToKeep} days" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up old logs");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }
}

