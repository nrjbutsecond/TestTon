using Application.DTOs.Activity;
using Application.DTOs.Common;
using Application.Helper;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TON.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkshopsController : ControllerBase
    {

        private readonly IWorkshopService _workshopService;
        private readonly ILogger<WorkshopsController> _logger;

        public WorkshopsController(IWorkshopService workshopService, ILogger<WorkshopsController> logger)
        {
            _workshopService = workshopService;
            _logger = logger;
        }

        /// <summary>
        /// Get upcoming workshops with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paged list of upcoming workshops</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PagedResult<WorkshopListDto>), 200)]
        public async Task<ActionResult<PagedResult<WorkshopListDto>>> GetWorkshops(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var workshops = await _workshopService.GetUpcomingWorkshopsAsync(pageNumber, pageSize);
            return Ok(workshops);
        }

        /// <summary>
        /// Get workshop by ID
        /// </summary>
        /// <param name="id">Workshop ID</param>
        /// <returns>Workshop details</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(WorkshopDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<WorkshopDto>> GetWorkshop(int id)
        {
            var workshop = await _workshopService.GetWorkshopByIdAsync(id);
            if (workshop == null)
                return NotFound(new { message = "Workshop not found" });

            return Ok(workshop);
        }

        /// <summary>
        /// Get official workshops
        /// </summary>
        /// <returns>List of official workshops</returns>
        [HttpGet("official")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<WorkshopListDto>), 200)]
        public async Task<ActionResult<IEnumerable<WorkshopListDto>>> GetOfficialWorkshops()
        {
            var workshops = await _workshopService.GetOfficialWorkshopsAsync();
            return Ok(workshops);
        }

        /// <summary>
        /// Get available workshops (not full)
        /// </summary>
        /// <returns>List of available workshops</returns>
        [HttpGet("available")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<WorkshopListDto>), 200)]
        public async Task<ActionResult<IEnumerable<WorkshopListDto>>> GetAvailableWorkshops()
        {
            var workshops = await _workshopService.GetAvailableWorkshopsAsync();
            return Ok(workshops);
        }

        /// <summary>
        /// Get workshops by organizer
        /// </summary>
        /// <param name="organizerId">Organizer ID</param>
        /// <returns>List of workshops by organizer</returns>
        [HttpGet("organizer/{organizerId}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<WorkshopListDto>), 200)]
        public async Task<ActionResult<IEnumerable<WorkshopListDto>>> GetWorkshopsByOrganizer(int organizerId)
        {
            var workshops = await _workshopService.GetWorkshopsByOrganizerAsync(organizerId);
            return Ok(workshops);
        }

        /// <summary>
        /// Check if user is registered for a workshop
        /// </summary>
        /// <param name="id">Workshop ID</param>
        /// <returns>Registration status</returns>
        [HttpGet("{id}/registration-status")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<ActionResult> CheckRegistrationStatus(int id)
        {
            var userId = GetCurrentUserId();
            var isRegistered = await _workshopService.IsUserRegisteredAsync(id, userId);

            return Ok(new { workshopId = id, userId, isRegistered });
        }

        /// <summary>
        /// Get registered participant count for a workshop
        /// </summary>
        /// <param name="id">Workshop ID</param>
        /// <returns>Number of registered participants</returns>
        [HttpGet("{id}/participants/count")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<ActionResult> GetRegisteredCount(int id)
        {
            var count = await _workshopService.GetRegisteredCountAsync(id);
            return Ok(new { workshopId = id, registeredCount = count });
        }

        /// <summary>
        /// Create a new workshop (Admin only)
        /// </summary>
        /// <param name="dto">Workshop creation data</param>
        /// <returns>Created workshop</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(WorkshopDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<WorkshopDto>> CreateWorkshop(CreateWorkshopDto dto)
        {
            try
            {
                var organizerId = GetCurrentUserId();
                var workshop = await _workshopService.CreateWorkshopAsync(dto, organizerId);

                return CreatedAtAction(
                    nameof(GetWorkshop),
                    new { id = workshop.Id },
                    workshop);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update a workshop (Admin only)
        /// </summary>
        /// <param name="id">Workshop ID</param>
        /// <param name="dto">Workshop update data</param>
        /// <returns>Updated workshop</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(WorkshopDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<WorkshopDto>> UpdateWorkshop(int id, UpdateWorkshopDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "ID mismatch" });

            try
            {
                var organizerId = GetCurrentUserId();
                var workshop = await _workshopService.UpdateWorkshopAsync(dto, organizerId);

                if (workshop == null)
                    return NotFound(new { message = "Workshop not found" });

                return Ok(workshop);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                return Forbid(ex.Message);
            }
        }

        /// <summary>
        /// Delete a workshop (Admin only)
        /// </summary>
        /// <param name="id">Workshop ID</param>
        /// <returns>No content on success</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteWorkshop(int id)
        {
            try
            {
                var organizerId = GetCurrentUserId();
                var result = await _workshopService.DeleteWorkshopAsync(id, organizerId);

                if (!result)
                    return NotFound(new { message = "Workshop not found" });

                return NoContent();
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                return Forbid(ex.Message);
            }
        }

        /// <summary>
        /// Publish a workshop (Admin only)
        /// </summary>
        /// <param name="id">Workshop ID</param>
        /// <returns>Success message</returns>
        [HttpPost("{id}/publish")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> PublishWorkshop(int id)
        {
            try
            {
                var organizerId = GetCurrentUserId();
                var result = await _workshopService.PublishWorkshopAsync(id, organizerId);

                if (!result)
                    return NotFound(new { message = "Workshop not found" });

                return Ok(new { message = "Workshop published successfully" });
            }
            catch (UnauthorizedException ex)
            {
                return Forbid(ex.Message);
            }
        }

        /// <summary>
        /// Cancel a workshop (Admin only)
        /// </summary>
        /// <param name="id">Workshop ID</param>
        /// <param name="request">Cancellation reason</param>
        /// <returns>Success message</returns>
        [HttpPost("{id}/cancel")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> CancelWorkshop(int id, [FromBody] CancelWorkshopRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Reason))
                return BadRequest(new { message = "Cancellation reason is required" });

            try
            {
                var organizerId = GetCurrentUserId();
                var result = await _workshopService.CancelWorkshopAsync(id, request.Reason, organizerId);

                if (!result)
                    return NotFound(new { message = "Workshop not found" });

                _logger.LogInformation("Workshop {WorkshopId} cancelled by user {UserId}", id, organizerId);

                return Ok(new { message = "Workshop cancelled successfully" });
            }
            catch (UnauthorizedException ex)
            {
                return Forbid(ex.Message);
            }
        }

        // Helper method to get current user ID from JWT claims
        

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ??
                              User.FindFirst("UserId") ??
                              User.FindFirst("id");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in claims");
            }

            return userId;
        }

    }

    /// <summary>
    /// Request model for workshop cancellation
    /// </summary>
    public class CancelWorkshopRequest
    {
        /// <summary>
        /// Reason for cancelling the workshop
        /// </summary>
        public string Reason { get; set; } = string.Empty;
    }
}
    
