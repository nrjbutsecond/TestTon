using Application.DTOs.Advertisement;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TON.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdvertisementController : ControllerBase
    {
        private readonly IAdvertisementService _advertisementService;

        public AdvertisementController(IAdvertisementService advertisementService)
        {
            _advertisementService = advertisementService;
        }

        /// <summary>
        /// Get all advertisements (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<AdvertisementListDto>>> GetAll()
        {
            try
            {
                var advertisements = await _advertisementService.GetAllAsync();
                return Ok(advertisements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching advertisements", error = ex.Message });
            }
        }

        /// <summary>
        /// Get active advertisements for display (Public)
        /// </summary>
        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AdvertisementListDto>>> GetActiveAdvertisements([FromQuery] string? position = null)
        {
            try
            {
                var advertisements = await _advertisementService.GetActiveAdvertisementsAsync(position);
                return Ok(advertisements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching active advertisements", error = ex.Message });
            }
        }

        /// <summary>
        /// Get advertisement by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<AdvertisementDto>> GetById(int id)
        {
            try
            {
                var advertisement = await _advertisementService.GetByIdAsync(id);

                // Check if user has permission to view details
                var userId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();

                if (userRole != "Admin" && advertisement.AdvertiserId != userId)
                {
                    return Forbid("You don't have permission to view this advertisement details");
                }

                return Ok(advertisement);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the advertisement", error = ex.Message });
            }
        }

        /// <summary>
        /// Get advertisements by advertiser (Organizer)
        /// </summary>
        [HttpGet("advertiser/{advertiserId}")]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<ActionResult<IEnumerable<AdvertisementListDto>>> GetByAdvertiser(int advertiserId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();

                // Check permission
                if (userRole != "Admin" && advertiserId != userId)
                {
                    return Forbid("You can only view your own advertisements");
                }

                var advertisements = await _advertisementService.GetAdvertiserAdsAsync(advertiserId);
                return Ok(advertisements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching advertiser's advertisements", error = ex.Message });
            }
        }

        /// <summary>
        /// Get my advertisements (Organizer)
        /// </summary>
        [HttpGet("my-ads")]
        [Authorize(Roles = "Organizer")]
        public async Task<ActionResult<IEnumerable<AdvertisementListDto>>> GetMyAds()
        {
            try
            {
                var userId = GetCurrentUserId();
                var advertisements = await _advertisementService.GetAdvertiserAdsAsync(userId);
                return Ok(advertisements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching your advertisements", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new advertisement (Organizer with eligible plan)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Organizer")]
        public async Task<ActionResult<AdvertisementDto>> Create([FromBody] CreateAdvertisementDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();

                // TODO: Check if user has eligible service plan for ads
                // This should be implemented based on your ServicePlan requirements

                var advertisement = await _advertisementService.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = advertisement.Id }, advertisement);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the advertisement", error = ex.Message });
            }
        }

        /// <summary>
        /// Update advertisement
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<ActionResult<AdvertisementDto>> Update(int id, [FromBody] UpdateAdvertisementDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                var advertisement = await _advertisementService.UpdateAsync(id, dto, userId);
                return Ok(advertisement);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the advertisement", error = ex.Message });
            }
        }

        /// <summary>
        /// Activate advertisement
        /// </summary>
        [HttpPut("{id}/activate")]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Activate(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _advertisementService.ActivateAsync(id, userId);

                if (!result)
                    return NotFound(new { message = "Advertisement not found" });

                return Ok(new { message = "Advertisement activated successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while activating the advertisement", error = ex.Message });
            }
        }

        /// <summary>
        /// Pause advertisement
        /// </summary>
        [HttpPut("{id}/pause")]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Pause(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _advertisementService.PauseAsync(id, userId);

                if (!result)
                    return NotFound(new { message = "Advertisement not found" });

                return Ok(new { message = "Advertisement paused successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while pausing the advertisement", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete advertisement (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = GetCurrentUserId().ToString();
                var result = await _advertisementService.DeleteAsync(id, userId);

                if (!result)
                    return NotFound(new { message = "Advertisement not found" });

                return Ok(new { message = "Advertisement deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the advertisement", error = ex.Message });
            }
        }

        /// <summary>
        /// Record ad view (Public)
        /// </summary>
        [HttpPost("{id}/view")]
        [AllowAnonymous]
        public async Task<IActionResult> RecordView(int id)
        {
            try
            {
                await _advertisementService.RecordViewAsync(id);
                return Ok(new { message = "View recorded" });
            }
            catch (Exception ex)
            {
                // Log error but don't expose details for tracking endpoints
                return Ok(new { message = "View recorded" }); // Always return success for tracking
            }
        }

        /// <summary>
        /// Record ad click (Public)
        /// </summary>
        [HttpPost("{id}/click")]
        [AllowAnonymous]
        public async Task<IActionResult> RecordClick(int id)
        {
            try
            {
                await _advertisementService.RecordClickAsync(id);
                return Ok(new { message = "Click recorded" });
            }
            catch (Exception ex)
            {
                // Log error but don't expose details for tracking endpoints
                return Ok(new { message = "Click recorded" }); // Always return success for tracking
            }
        }

        /// <summary>
        /// Get advertisement statistics
        /// </summary>
        [HttpGet("statistics")]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<ActionResult<AdvertisementStatisticsDto>> GetStatistics([FromQuery] int? advertiserId = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();

                // If not admin, can only see own statistics
                if (userRole != "Admin")
                {
                    advertiserId = userId;
                }

                var statistics = await _advertisementService.GetStatisticsAsync(advertiserId);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching statistics", error = ex.Message });
            }
        }

        /// <summary>
        /// Get expiring advertisements
        /// </summary>
        [HttpGet("expiring")]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<ActionResult<IEnumerable<AdvertisementListDto>>> GetExpiringAds([FromQuery] int daysAhead = 7)
        {
            try
            {
                var advertisements = await _advertisementService.GetExpiringAdsAsync(daysAhead);
                return Ok(advertisements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching expiring advertisements", error = ex.Message });
            }
        }

        /// <summary>
        /// Check if advertiser has active ad in position
        /// </summary>
        [HttpGet("check-position")]
        [Authorize(Roles = "Organizer")]
        public async Task<ActionResult<bool>> CheckPosition([FromQuery] string position)
        {
            try
            {
                var userId = GetCurrentUserId();
                var hasActiveAd = await _advertisementService.HasActiveAdInPositionAsync(userId, position);
                return Ok(new { hasActiveAd, position });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking position availability", error = ex.Message });
            }
        }

        // Helper methods
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
                return userId;
            throw new UnauthorizedAccessException("User ID not found in claims");
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        }
    }
}