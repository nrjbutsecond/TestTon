using Application.DTOs.SupportPersonnel;
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
    public class PersonnelSupportController : ControllerBase
    {
        private readonly IPersonnelSupportService _personnelService;

        public PersonnelSupportController(IPersonnelSupportService personnelService)
        {
            _personnelService = personnelService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        private bool IsCommunityStaff()
        {
            return User.IsInRole("CommunityStaff");
        }

        #region Support Personnel Endpoints

        /// <summary>
        /// Get all support personnel (Admin/Community Staff)
        /// </summary>
        [HttpGet("personnel")]
        [Authorize(Roles = "Admin,CommunityStaff")]
        public async Task<ActionResult<IEnumerable<SupportPersonnelListDto>>> GetAllPersonnel([FromQuery] bool? isActive = null)
        {
            try
            {
                var personnel = await _personnelService.GetAllSupportPersonnelAsync(isActive);
                return Ok(personnel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get support personnel by ID
        /// </summary>
        [HttpGet("personnel/{id}")]
        public async Task<ActionResult<SupportPersonnelDto>> GetPersonnelById(int id)
        {
            try
            {
                var personnel = await _personnelService.GetSupportPersonnelByIdAsync(id);
                return Ok(personnel);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get personnel registered by current organizer
        /// </summary>
        [HttpGet("personnel/my-personnel")]
        [Authorize(Roles = "Organizer")]
        public async Task<ActionResult<IEnumerable<SupportPersonnelListDto>>> GetMyPersonnel()
        {
            try
            {
                var userId = GetCurrentUserId();
                var personnel = await _personnelService.GetPersonnelByOrganizerAsync(userId);
                return Ok(personnel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Register new support personnel (Organizer only)
        /// </summary>
        [HttpPost("personnel")]
        [Authorize(Roles = "Organizer")]
        public async Task<ActionResult<SupportPersonnelDto>> CreatePersonnel([FromBody] CreateSupportPersonnelDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var personnel = await _personnelService.CreateSupportPersonnelAsync(dto, userId);
                return CreatedAtAction(nameof(GetPersonnelById), new { id = personnel.Id }, personnel);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update support personnel
        /// </summary>
        [HttpPut("personnel/{id}")]
        [Authorize(Roles = "Organizer,Admin,CommunityStaff")]
        public async Task<ActionResult<SupportPersonnelDto>> UpdatePersonnel(int id, [FromBody] UpdateSupportPersonnelDto dto)
        {
            try
            {
                var personnel = await _personnelService.UpdateSupportPersonnelAsync(id, dto);
                return Ok(personnel);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete support personnel (Admin/Community Staff)
        /// </summary>
        [HttpDelete("personnel/{id}")]
        [Authorize(Roles = "Admin,CommunityStaff")]
        public async Task<ActionResult> DeletePersonnel(int id)
        {
            try
            {
                var result = await _personnelService.DeleteSupportPersonnelAsync(id);
                if (!result)
                    return NotFound(new { message = "Personnel not found" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Toggle personnel active status
        /// </summary>
        [HttpPatch("personnel/{id}/toggle-status")]
        [Authorize(Roles = "Admin,CommunityStaff")]
        public async Task<ActionResult> TogglePersonnelStatus(int id)
        {
            try
            {
                var result = await _personnelService.TogglePersonnelStatusAsync(id);
                if (!result)
                    return NotFound(new { message = "Personnel not found" });

                return Ok(new { message = "Status toggled successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region Personnel Support Request Endpoints

        /// <summary>
        /// Get all support requests (Admin/Community Staff)
        /// </summary>
        [HttpGet("requests")]
        [Authorize(Roles = "Admin,CommunityStaff")]
        public async Task<ActionResult<IEnumerable<PersonnelSupportRequestListDto>>> GetAllRequests([FromQuery] string? status = null)
        {
            try
            {
                var requests = await _personnelService.GetAllRequestsAsync(status);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get support request by ID
        /// </summary>
        [HttpGet("requests/{id}")]
        public async Task<ActionResult<PersonnelSupportRequestDto>> GetRequestById(int id)
        {
            try
            {
                var request = await _personnelService.GetRequestByIdAsync(id);
                return Ok(request);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get current organizer's requests
        /// </summary>
        [HttpGet("requests/my-requests")]
        [Authorize(Roles = "Organizer")]
        public async Task<ActionResult<IEnumerable<PersonnelSupportRequestListDto>>> GetMyRequests()
        {
            try
            {
                var userId = GetCurrentUserId();
                var requests = await _personnelService.GetRequestsByOrganizerAsync(userId);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get requests by event
        /// </summary>
        [HttpGet("requests/event/{eventId}")]
        public async Task<ActionResult<IEnumerable<PersonnelSupportRequestListDto>>> GetRequestsByEvent(int eventId)
        {
            try
            {
                var requests = await _personnelService.GetRequestsByEventAsync(eventId);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Create new support request (Organizer only)
        /// </summary>
        [HttpPost("requests")]
        [Authorize(Roles = "Organizer")]
        public async Task<ActionResult<PersonnelSupportRequestDto>> CreateRequest([FromBody] CreatePersonnelSupportRequestDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var request = await _personnelService.CreateRequestAsync(dto, userId);
                return CreatedAtAction(nameof(GetRequestById), new { id = request.Id }, request);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update support request (Organizer only, Pending/Cancelled status)
        /// </summary>
        [HttpPut("requests/{id}")]
        [Authorize(Roles = "Organizer")]
        public async Task<ActionResult<PersonnelSupportRequestDto>> UpdateRequest(int id, [FromBody] UpdatePersonnelSupportRequestDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var request = await _personnelService.UpdateRequestAsync(id, dto, userId);
                return Ok(request);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete support request (Organizer only)
        /// </summary>
        [HttpDelete("requests/{id}")]
        [Authorize(Roles = "Organizer")]
        public async Task<ActionResult> DeleteRequest(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _personnelService.DeleteRequestAsync(id, userId);
                if (!result)
                    return NotFound(new { message = "Request not found" });

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region Request Management (Admin/Community Staff)

        /// <summary>
        /// Approve support request
        /// </summary>
        [HttpPost("requests/{id}/approve")]
        [Authorize(Roles = "Admin,CommunityStaff")]
        public async Task<ActionResult<PersonnelSupportRequestDto>> ApproveRequest(int id, [FromBody] string? notes = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var request = await _personnelService.ApproveRequestAsync(id, userId, notes);
                return Ok(request);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Reject support request
        /// </summary>
        [HttpPost("requests/{id}/reject")]
        [Authorize(Roles = "Admin,CommunityStaff")]
        public async Task<ActionResult<PersonnelSupportRequestDto>> RejectRequest(int id, [FromBody] string reason)
        {
            try
            {
                var userId = GetCurrentUserId();
                var request = await _personnelService.RejectRequestAsync(id, userId, reason);
                return Ok(request);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Assign personnel to request
        /// </summary>
        [HttpPost("requests/assign")]
        [Authorize(Roles = "Admin,CommunityStaff")]
        public async Task<ActionResult<PersonnelSupportRequestDto>> AssignPersonnel([FromBody] AssignPersonnelDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var request = await _personnelService.AssignPersonnelToRequestAsync(dto, userId);
                return Ok(request);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Remove personnel from request
        /// </summary>
        [HttpDelete("requests/{requestId}/personnel/{personnelId}")]
        [Authorize(Roles = "Admin,CommunityStaff")]
        public async Task<ActionResult> RemovePersonnel(int requestId, int personnelId)
        {
            try
            {
                var result = await _personnelService.RemovePersonnelFromRequestAsync(requestId, personnelId);
                if (!result)
                    return NotFound(new { message = "Assignment not found" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update assignment status
        /// </summary>
        [HttpPatch("assignments/status")]
        [Authorize(Roles = "Admin,CommunityStaff")]
        public async Task<ActionResult> UpdateAssignmentStatus([FromBody] UpdateAssignmentStatusDto dto)
        {
            try
            {
                var result = await _personnelService.UpdateAssignmentStatusAsync(dto);
                if (!result)
                    return NotFound(new { message = "Assignment not found" });

                return Ok(new { message = "Status updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Mark request as in progress
        /// </summary>
        [HttpPatch("requests/{id}/in-progress")]
        [Authorize(Roles = "Admin,CommunityStaff")]
        public async Task<ActionResult> MarkInProgress(int id)
        {
            try
            {
                var result = await _personnelService.MarkRequestInProgressAsync(id);
                if (!result)
                    return NotFound(new { message = "Request not found" });

                return Ok(new { message = "Request marked as in progress" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Complete request
        /// </summary>
        [HttpPost("requests/{id}/complete")]
        [Authorize(Roles = "Admin,CommunityStaff")]
        public async Task<ActionResult> CompleteRequest(int id, [FromBody] string? notes = null)
        {
            try
            {
                var result = await _personnelService.CompleteRequestAsync(id, notes);
                if (!result)
                    return NotFound(new { message = "Request not found" });

                return Ok(new { message = "Request completed successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cancel request
        /// </summary>
        [HttpPost("requests/{id}/cancel")]
        [Authorize(Roles = "Admin,CommunityStaff,Organizer")]
        public async Task<ActionResult> CancelRequest(int id, [FromBody] string reason)
        {
            try
            {
                var result = await _personnelService.CancelRequestAsync(id, reason);
                if (!result)
                    return NotFound(new { message = "Request not found" });

                return Ok(new { message = "Request cancelled successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region Statistics & Utilities

        /// <summary>
        /// Get request statistics
        /// </summary>
        [HttpGet("statistics")]
        [Authorize(Roles = "Admin,CommunityStaff")]
        public async Task<ActionResult<Dictionary<string, int>>> GetStatistics()
        {
            try
            {
                var stats = await _personnelService.GetRequestStatisticsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get available personnel for date range
        /// </summary>
        [HttpGet("personnel/available")]
        [Authorize(Roles = "Admin,CommunityStaff")]
        public async Task<ActionResult<IEnumerable<SupportPersonnelListDto>>> GetAvailablePersonnel(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                var personnel = await _personnelService.GetAvailablePersonnelAsync(startDate, endDate);
                return Ok(personnel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion
    }
}