using Application.DTOs.Common;
using Application.DTOs.TalkEvent;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TON.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TalkEventController : ControllerBase
    {
        private readonly ITalkEventService _talkEventService;

        public TalkEventController(ITalkEventService talkEventService)
        {
            _talkEventService = talkEventService;
        }

        // GET: api/talkevent
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<TalkEventListDto>>> GetEvents(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] string? orderBy = null)
        {
            TalkEventStatus? parsedStatus = null;
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<TalkEventStatus>(status, true, out var tempStatus))
            {
                parsedStatus = tempStatus;
            }
            var result = await _talkEventService.GetPagedEventsAsync(pageNumber, pageSize, parsedStatus, orderBy);
            return Ok(result);
        }

        // GET: api/talkevent/upcoming
        [HttpGet("upcoming")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<TalkEventListDto>>> GetUpcomingEvents(
            [FromQuery] int limit = 10)
        {
            var events = await _talkEventService.GetUpcomingEventsAsync(limit);
            return Ok(events);
        }

        // GET: api/talkevent/partnered
        [HttpGet("partnered")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<TalkEventListDto>>> GetPartneredEvents()
        {
            var events = await _talkEventService.GetPartneredEventsAsync();
            return Ok(events);
        }

        // GET: api/talkevent/organizer/{organizerId}
        [HttpGet("organizer/{organizerId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TalkEventListDto>>> GetOrganizerEvents(int organizerId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

            if (userRole != "Admin" && userId != organizerId)
                return Forbid();

            var events = await _talkEventService.GetOrganizerEventsAsync(organizerId);
            return Ok(events);
        }

        // GET: api/talkevent/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<TalkEventResponseDto>> GetEvent(int id)
        {
            var talkEvent = await _talkEventService.GetByIdAsync(id);
            if (talkEvent == null)
                return NotFound();

            return Ok(talkEvent);
        }

        // POST: api/talkevent
        [HttpPost]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<ActionResult<TalkEventResponseDto>> CreateEvent(CreateTalkEventDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "";

            try
            {
                var result = await _talkEventService.CreateAsync(dto, userId, userEmail);
                return CreatedAtAction(nameof(GetEvent), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/talkevent/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<ActionResult<TalkEventResponseDto>> UpdateEvent(int id, UpdateTalkEventDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "";
            if (!Enum.TryParse<UserRoles>(userRole, out var userRoleE))
                return Forbid();

            if (!await _talkEventService.CanUserManageEventAsync(id, userId, userRoleE))
                return Forbid();

            try
            {
                var result = await _talkEventService.UpdateAsync(id, dto, userEmail);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PATCH: api/talkevent/{id}/status
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> UpdateEventStatus(
            int id,
            [FromBody] UpdateStatusDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "";
            if (!Enum.TryParse<UserRoles>(userRole, out var userRoleE))
                return Forbid();
            if (!await _talkEventService.CanUserManageEventAsync(id, userId, userRoleE))
                return Forbid();

            try
            {
                if (!Enum.TryParse<TalkEventStatus>(dto.Status, out var parsedStatus))
                    return BadRequest("Invalid status value.");

                var result = await _talkEventService.UpdateStatusAsync(id, parsedStatus, userEmail, dto.Reason);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/talkevent/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "";

            if (!Enum.TryParse<UserRoles>(userRole, out var userRoleE))
                return Forbid();
            if (!await _talkEventService.CanUserManageEventAsync(id, userId, userRoleE))
                return Forbid();

            try
            {
                var result = await _talkEventService.DeleteAsync(id, userEmail);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("deleted")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagedResult<DeletedTalkEventDto>>> GetDeletedEvents(
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10,
           [FromQuery] string? searchTerm = null,
           [FromQuery] DateTime? deletedFrom = null,
           [FromQuery] DateTime? deletedTo = null,
           [FromQuery] bool? hasTicketsSold = null,
           [FromQuery] string? orderBy = "deleted")
        {
            var filter = new DeletedEventFilterDto
            {
                SearchTerm = searchTerm,
                DeletedFrom = deletedFrom,
                DeletedTo = deletedTo,
                HasTicketsSold = hasTicketsSold,
                OrderBy = orderBy
            };

            var result = await _talkEventService.GetDeletedEventsAsync(pageNumber, pageSize, filter);
            return Ok(result);
        }

        // GET: api/talkevent/deleted/{id}
        [HttpGet("deleted/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DeletedTalkEventDto>> GetDeletedEventDetails(int id)
        {
            var result = await _talkEventService.GetDeletedEventDetailsAsync(id);
            if (result == null)
                return NotFound(new { message = "Deleted event not found" });

            return Ok(result);
        }

        // GET: api/talkevent/{id}/restore/validate
        [HttpGet("{id}/restore/validate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RestoreValidationDto>> ValidateRestore(int id)
        {
            var result = await _talkEventService.ValidateRestoreAsync(id);
            return Ok(result);
        }

        // PATCH: api/talkevent/{id}/restore
        [HttpPatch("{id}/restore")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreEvent(int id, [FromBody] RestoreEventDto? dto = null)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "";

            var (success, error) = await _talkEventService.RestoreAsync(id, userEmail);

            if (!success)
                return BadRequest(new { message = error });

            return Ok(new { message = "Event restored successfully" });
        }
    }





    

    public class UpdateStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
    }
}