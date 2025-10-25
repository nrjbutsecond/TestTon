using Application.DTOs.Ticket;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Ticket.Application.Interface;
using Ticket.Application.Service;


namespace TON.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ILogger<TicketController> _logger;

        public TicketController(
            ITicketService ticketService,
            ILogger<TicketController> logger)
        {
            _ticketService = ticketService;
            _logger = logger;
        }

        /// <summary>
        /// Reserve a ticket for the current user
        /// </summary>
        [HttpPost("reserve")]
        [Authorize(Roles = "Enthusiast,Organizer,Admin")]
        public async Task<ActionResult<TicketDto>> ReserveTicket([FromBody] ReserveTicketDto dto)
        {
            try
            {
                // Set user ID from JWT token
                dto.UserId = GetCurrentUserId();

                var ticket = await _ticketService.ReserveTicketAsync(dto);
                return Ok(ticket);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when reserving ticket");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation when reserving ticket");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reserving ticket");
                return StatusCode(500, "An error occurred while reserving the ticket");
            }
        }

        /// <summary>
        /// Confirm payment for a ticket
        /// </summary>
        [HttpPost("confirm-payment/{ticketGuid}")]
        [AllowAnonymous] // Payment gateway callback
        public async Task<ActionResult<TicketDto>> ConfirmPayment(Guid ticketGuid)
        {
            try
            {
                // TODO: Verify payment with payment gateway before confirming

                var ticket = await _ticketService.ConfirmPaymentAsync(ticketGuid);
                return Ok(ticket);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Ticket not found: {TicketGuid}", ticketGuid);
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation when confirming payment");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming payment for ticket {TicketGuid}", ticketGuid);
                return StatusCode(500, "An error occurred while confirming payment");
            }
        }

        /// <summary>
        /// Get all tickets for the current user
        /// </summary>
        [HttpGet("my-tickets")]
        [Authorize(Roles = "Enthusiast,Organizer,Admin")]
        public async Task<ActionResult<List<TicketDto>>> GetMyTickets()
        {
            try
            {
                var userId = GetCurrentUserId();
                var tickets = await _ticketService.GetUserTicketsAsync(userId);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user tickets");
                return StatusCode(500, "An error occurred while retrieving tickets");
            }
        }

        /// <summary>
        /// Get tickets for a specific user (Admin only)
        /// </summary>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<TicketDto>>> GetUserTickets(int userId)
        {
            try
            {
                var tickets = await _ticketService.GetUserTicketsAsync(userId);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tickets for user {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving tickets");
            }
        }

        /// <summary>
        /// Get ticket information by QR code
        /// </summary>
        [HttpGet("qr/{qrCode}")]
        [Authorize(Roles = "Staff,Admin,Organizer")]
        public async Task<ActionResult<TicketDto>> GetTicketByQRCode(string qrCode)
        {
            try
            {
                var ticket = await _ticketService.GetTicketByQRCodeAsync(qrCode);
                return Ok(ticket);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Ticket not found for QR code");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ticket by QR code");
                return StatusCode(500, "An error occurred while retrieving the ticket");
            }
        }

        /// <summary>
        /// Scan a ticket at the event
        /// </summary>
        [HttpPost("scan")]
        [Authorize(Roles = "Staff,Admin,Organizer")]
        public async Task<ActionResult<ScanTicketResultDto>> ScanTicket([FromBody] ScanTicketRequest request)
        {
            try
            {
                var scannedBy = GetCurrentUserEmail();
                var result = await _ticketService.ScanTicketAsync(request.QRCode, scannedBy);

                // Return appropriate status code based on scan result
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    // Still return 200 OK but with IsSuccess = false
                    // This allows the scanner app to handle the error gracefully
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scanning ticket");
                return StatusCode(500, "An error occurred while scanning the ticket");
            }
        }

        /// <summary>
        /// Cancel a ticket
        /// </summary>
        [HttpPost("{id}/cancel")]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<ActionResult> CancelTicket(int id, [FromBody] CancelTicketRequest request)
        {
            try
            {
                var userId = GetCurrentUserId(); // Get từ JWT
                var userRole = GetCurrentUserRole(); // Get từ JWT

                // Verify organizer owns the event
                if (userRole == "Organizer")
                {
                    var canManage = await _ticketService.CanOrganizerCancelTicketAsync(id, userId);
                    if (!canManage)
                        return Forbid("You don't have permission to cancel this ticket");
                }

                var success = await _ticketService.CancelTicketAsync(id, request.Reason, userId, userRole);

                if (success)
                {
                    return NoContent(); // 204 No Content
                }

                return BadRequest("Failed to cancel ticket");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when cancelling ticket");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Ticket not found: {TicketId}", id);
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation when cancelling ticket");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling ticket {TicketId}", id);
                return StatusCode(500, "An error occurred while cancelling the ticket");
            }
        }

        /// <summary>
        /// Expire old tickets (Admin only)
        /// </summary>
        [HttpPost("expire")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> ExpireOldTickets()
        {
            try
            {
                var count = await _ticketService.ExpireOldTicketsAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error expiring old tickets");
                return StatusCode(500, "An error occurred while expiring tickets");
            }
        }

        // Helper methods
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
        private string GetCurrentUserRole()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role) ??
                            User.FindFirst("role") ??
                            User.FindFirst("roles");

            if (roleClaim == null || string.IsNullOrEmpty(roleClaim.Value))
            {
                throw new UnauthorizedAccessException("User role not found in claims");
            }

            return roleClaim.Value;
        }

        private string GetCurrentUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value ??
                   User.FindFirst("email")?.Value ??
                   "Unknown";
        }
    }

    // Request models specific to API
    public class ScanTicketRequest
    {
        public string QRCode { get; set; }
    }

    public class CancelTicketRequest
    {
        public string Reason { get; set; }
    }
}