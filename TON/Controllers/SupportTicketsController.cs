using Application.DTOs.SupportTicket;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TON.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupportTicketsController : ControllerBase
    {
        private readonly ISupportTicketService _ticketService;

        public SupportTicketsController(ISupportTicketService ticketService)
        {
            _ticketService = ticketService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        private bool IsStaff()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            return role == "Admin" || role == "Community Staff" || role == "Sales Staff";
        }

        /// <summary>
        /// Get all tickets (Admin/Staff only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Community Staff,Sales Staff")]
        public async Task<ActionResult<IEnumerable<SupportTicketListDto>>> GetAllTickets([FromQuery] TicketFilterDto filter)
        {
            try
            {
                var tickets = await _ticketService.GetAllTicketsAsync(filter);
                return Ok(new { success = true, data = tickets });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get my tickets (for logged-in customer)
        /// </summary>
        [HttpGet("my-tickets")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SupportTicketListDto>>> GetMyTickets()
        {
            try
            {
                var userId = GetCurrentUserId();
                var tickets = await _ticketService.GetMyTicketsAsync(userId);
                return Ok(new { success = true, data = tickets });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get tickets assigned to me (Staff only)
        /// </summary>
        [HttpGet("assigned-to-me")]
        [Authorize(Roles = "Admin,Community Staff,Sales Staff")]
        public async Task<ActionResult<IEnumerable<SupportTicketListDto>>> GetAssignedTickets()
        {
            try
            {
                var userId = GetCurrentUserId();
                var tickets = await _ticketService.GetAssignedTicketsAsync(userId);
                return Ok(new { success = true, data = tickets });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get ticket by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<SupportTicketDetailDto>> GetTicketById(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var ticket = await _ticketService.GetTicketByIdAsync(id, IsStaff() ? null : userId);

                if (ticket == null)
                    return NotFound(new { success = false, message = "Ticket not found or access denied" });

                return Ok(new { success = true, data = ticket });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get ticket by ticket number
        /// </summary>
        [HttpGet("by-number/{ticketNumber}")]
        [Authorize]
        public async Task<ActionResult<SupportTicketDetailDto>> GetTicketByNumber(string ticketNumber)
        {
            try
            {
                var ticket = await _ticketService.GetTicketByNumberAsync(ticketNumber);

                if (ticket == null)
                    return NotFound(new { success = false, message = "Ticket not found" });

                // Check permission
                var userId = GetCurrentUserId();
                if (!IsStaff() && ticket.Customer.Id != userId)
                    return Forbid();

                return Ok(new { success = true, data = ticket });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Create new ticket
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<SupportTicketDetailDto>> CreateTicket([FromBody] CreateSupportTicketDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, errors = ModelState });

                var userId = GetCurrentUserId();
                var ticket = await _ticketService.CreateTicketAsync(dto, userId);

                return CreatedAtAction(
                    nameof(GetTicketById),
                    new { id = ticket.Id },
                    new { success = true, data = ticket, message = "Ticket created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Update ticket (Staff only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Community Staff,Sales Staff")]
        public async Task<ActionResult<SupportTicketDetailDto>> UpdateTicket(int id, [FromBody] UpdateSupportTicketDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, errors = ModelState });

                var ticket = await _ticketService.UpdateTicketAsync(id, dto);
                return Ok(new { success = true, data = ticket, message = "Ticket updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Delete ticket (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteTicket(int id)
        {
            try
            {
                var result = await _ticketService.DeleteTicketAsync(id);

                if (!result)
                    return NotFound(new { success = false, message = "Ticket not found" });

                return Ok(new { success = true, message = "Ticket deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Assign ticket to staff
        /// </summary>
        [HttpPost("{id}/assign")]
        [Authorize(Roles = "Admin,Community Staff,Sales Staff")]
        public async Task<ActionResult> AssignTicket(int id, [FromBody] AssignTicketRequest request)
        {
            try
            {
                var result = await _ticketService.AssignTicketAsync(id, request.AssigneeId);

                if (!result)
                    return NotFound(new { success = false, message = "Ticket not found" });

                return Ok(new { success = true, message = "Ticket assigned successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Unassign ticket
        /// </summary>
        [HttpPost("{id}/unassign")]
        [Authorize(Roles = "Admin,Community Staff,Sales Staff")]
        public async Task<ActionResult> UnassignTicket(int id)
        {
            try
            {
                var result = await _ticketService.UnassignTicketAsync(id);

                if (!result)
                    return NotFound(new { success = false, message = "Ticket not found" });

                return Ok(new { success = true, message = "Ticket unassigned successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Update ticket status
        /// </summary>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Community Staff,Sales Staff")]
        public async Task<ActionResult> UpdateTicketStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                var result = await _ticketService.UpdateTicketStatusAsync(id, request.Status);

                if (!result)
                    return NotFound(new { success = false, message = "Ticket not found" });

                return Ok(new { success = true, message = "Status updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Close ticket
        /// </summary>
        [HttpPost("{id}/close")]
        [Authorize(Roles = "Admin,Community Staff,Sales Staff")]
        public async Task<ActionResult> CloseTicket(int id)
        {
            try
            {
                var result = await _ticketService.CloseTicketAsync(id);

                if (!result)
                    return NotFound(new { success = false, message = "Ticket not found" });

                return Ok(new { success = true, message = "Ticket closed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Reopen ticket
        /// </summary>
        [HttpPost("{id}/reopen")]
        [Authorize]
        public async Task<ActionResult> ReopenTicket(int id)
        {
            try
            {
                var result = await _ticketService.ReopenTicketAsync(id);

                if (!result)
                    return NotFound(new { success = false, message = "Ticket not found" });

                return Ok(new { success = true, message = "Ticket reopened successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Escalate ticket
        /// </summary>
        [HttpPost("{id}/escalate")]
        [Authorize(Roles = "Admin,Community Staff,Sales Staff")]
        public async Task<ActionResult> EscalateTicket(int id)
        {
            try
            {
                var result = await _ticketService.EscalateTicketAsync(id);

                if (!result)
                    return NotFound(new { success = false, message = "Ticket not found" });

                return Ok(new { success = true, message = "Ticket escalated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get ticket messages
        /// </summary>
        [HttpGet("{id}/messages")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TicketMessageDto>>> GetTicketMessages(int id)
        {
            try
            {
                // Verify access
                var userId = GetCurrentUserId();
                var ticket = await _ticketService.GetTicketByIdAsync(id, IsStaff() ? null : userId);

                if (ticket == null)
                    return NotFound(new { success = false, message = "Ticket not found or access denied" });

                var messages = await _ticketService.GetTicketMessagesAsync(id);
                return Ok(new { success = true, data = messages });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Add message to ticket
        /// </summary>
        [HttpPost("{id}/messages")]
        [Authorize]
        public async Task<ActionResult<TicketMessageDto>> AddMessage(int id, [FromBody] AddTicketMessageDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, errors = ModelState });

                // Verify access
                var userId = GetCurrentUserId();
                var ticket = await _ticketService.GetTicketByIdAsync(id, IsStaff() ? null : userId);

                if (ticket == null)
                    return NotFound(new { success = false, message = "Ticket not found or access denied" });

                // Only staff can send internal messages
                if (dto.IsInternal && !IsStaff())
                    return Forbid();

                var message = await _ticketService.AddMessageAsync(id, dto, userId);
                return Ok(new { success = true, data = message, message = "Message sent successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Rate ticket (Customer only)
        /// </summary>
        [HttpPost("{id}/rate")]
        [Authorize]
        public async Task<ActionResult> RateTicket(int id, [FromBody] RateTicketDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, errors = ModelState });

                var userId = GetCurrentUserId();
                var result = await _ticketService.RateTicketAsync(id, dto, userId);

                if (!result)
                    return NotFound(new { success = false, message = "Ticket not found or access denied" });

                return Ok(new { success = true, message = "Rating submitted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get ticket statistics (Staff only)
        /// </summary>
        [HttpGet("statistics")]
        [Authorize(Roles = "Admin,Community Staff,Sales Staff")]
        public async Task<ActionResult<TicketStatisticsDto>> GetStatistics([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            try
            {
                var stats = await _ticketService.GetStatisticsAsync(fromDate, toDate);
                return Ok(new { success = true, data = stats });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get team performance (Admin only)
        /// </summary>
        [HttpGet("team-performance")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<TeamPerformanceDto>>> GetTeamPerformance()
        {
            try
            {
                var performance = await _ticketService.GetTeamPerformanceAsync();
                return Ok(new { success = true, data = performance });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Search tickets
        /// </summary>
        [HttpGet("search")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SupportTicketListDto>>> SearchTickets([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest(new { success = false, message = "Search term is required" });

                var tickets = await _ticketService.SearchTicketsAsync(searchTerm);
                return Ok(new { success = true, data = tickets });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    // Request models
    public class AssignTicketRequest
    {
        public int AssigneeId { get; set; }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = null!;
    }
}