using Application.DTOs.ConsultationRequest;
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
    [Authorize]
    public class ConsultationRequestsController : ControllerBase
    {
        private readonly IConsultationRequestService _consultationService;

        public ConsultationRequestsController(IConsultationRequestService consultationService)
        {
            _consultationService = consultationService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        private string GetUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "Guest";
        }

        /// <summary>
        /// Create a new consultation request (Organizer only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> CreateConsultationRequest([FromBody] CreateConsultationRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = GetUserId();
                var result = await _consultationService.CreateConsultationRequestAsync(userId, dto);
                return CreatedAtAction(nameof(GetConsultationRequestById), new { id = result.Id }, result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the consultation request", details = ex.Message });
            }
        }

        /// <summary>
        /// Get consultation request by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetConsultationRequestById(int id)
        {
            try
            {
                var userId = GetUserId();
                var userRole = GetUserRole();
                var result = await _consultationService.GetConsultationRequestByIdAsync(id, userId, userRole);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the consultation request", details = ex.Message });
            }
        }

        /// <summary>
        /// Get my consultation requests (Organizer only)
        /// </summary>
        [HttpGet("my-requests")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> GetMyConsultationRequests()
        {
            try
            {
                var userId = GetUserId();
                var result = await _consultationService.GetMyConsultationRequestsAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving consultation requests", details = ex.Message });
            }
        }

        /// <summary>
        /// Get all consultation requests with optional status filter (Admin/Community Staff only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,CommunityStaff")]
        public async Task<IActionResult> GetAllConsultationRequests([FromQuery] string? status = null)
        {
            try
            {
                var result = await _consultationService.GetAllConsultationRequestsAsync(status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving consultation requests", details = ex.Message });
            }
        }

        /// <summary>
        /// Get assigned consultations for staff (Community Staff only)
        /// </summary>
        [HttpGet("assigned")]
        [Authorize(Roles = "CommunityStaff")]
        public async Task<IActionResult> GetAssignedConsultations()
        {
            try
            {
                var userId = GetUserId();
                var result = await _consultationService.GetAssignedConsultationsAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving assigned consultations", details = ex.Message });
            }
        }

        /// <summary>
        /// Update consultation request (Organizer only, only pending requests)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> UpdateConsultationRequest(int id, [FromBody] UpdateConsultationRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = GetUserId();
                var result = await _consultationService.UpdateConsultationRequestAsync(id, userId, dto);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the consultation request", details = ex.Message });
            }
        }

        /// <summary>
        /// Schedule a consultation request (Admin/Community Staff only)
        /// </summary>
        [HttpPost("{id}/schedule")]
        [Authorize(Roles = "Admin,CommunityStaff")]
        public async Task<IActionResult> ScheduleConsultation(int id, [FromBody] ScheduleConsultationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _consultationService.ScheduleConsultationAsync(id, dto);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while scheduling the consultation", details = ex.Message });
            }
        }

        /// <summary>
        /// Update consultation status (Admin/Community Staff only)
        /// </summary>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,CommunityStaff")]
        public async Task<IActionResult> UpdateConsultationStatus(int id, [FromBody] UpdateConsultationStatusDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _consultationService.UpdateConsultationStatusAsync(id, dto);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the consultation status", details = ex.Message });
            }
        }

        /// <summary>
        /// Cancel consultation request
        /// </summary>
        [HttpPost("{id}/cancel")]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> CancelConsultationRequest(int id)
        {
            try
            {
                var userId = GetUserId();
                var userRole = GetUserRole();
                var result = await _consultationService.CancelConsultationRequestAsync(id, userId, userRole);
                return Ok(new { success = result, message = "Consultation request cancelled successfully" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while cancelling the consultation request", details = ex.Message });
            }
        }

        /// <summary>
        /// Get consultation statistics (Admin only)
        /// </summary>
        [HttpGet("statistics")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetConsultationStats()
        {
            try
            {
                var result = await _consultationService.GetConsultationStatsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving consultation statistics", details = ex.Message });
            }
        }
    }
}