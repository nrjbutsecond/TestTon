using Application.DTOs.Common;
using Application.DTOs.Ticket;
using Application.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Ticket.Application.Interface;

namespace TON.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketTypeController : ControllerBase
    {
        private readonly ITicketTypeService _ticketTypeService;
        private readonly ILogger<TicketTypeController> _logger;

        public TicketTypeController(
            ITicketTypeService ticketTypeService,
            ILogger<TicketTypeController> logger)
        {
            _ticketTypeService = ticketTypeService;
            _logger = logger;
        }

        /// <summary>
        /// Get ticket type by ID
        /// </summary>
        /// <param name="id">Ticket type ID</param>
        /// <returns>Ticket type details</returns>
        /// <response code="200">Returns the ticket type</response>
        /// <response code="404">Ticket type not found</response>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TicketTypeDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TicketTypeDto>> GetById(int id)
        {
            var ticketType = await _ticketTypeService.GetByIdAsync(id);
            if (ticketType == null)
                return NotFound($"Ticket type with id {id} not found");

            return Ok(ticketType);
        }

        // GET: api/tickettype
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<TicketTypeDto>>> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool? onlyAvailable = null,
            [FromQuery] string? orderBy = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var result = await _ticketTypeService.GetPagedAsync(pageNumber, pageSize, onlyAvailable, orderBy);
            return Ok(result);
        }

        // GET: api/tickettype/talkevent/{talkEventId}
        [HttpGet("talkevent/{talkEventId:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<TicketTypeDto>>> GetByTalkEvent(int talkEventId)
        {
            var ticketTypes = await _ticketTypeService.GetByTalkEventAsync(talkEventId);
            return Ok(ticketTypes);
        }

        // GET: api/tickettype/workshop/{workshopId}
        [HttpGet("workshop/{workshopId:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<TicketTypeDto>>> GetByWorkshop(int workshopId)
        {
            var ticketTypes = await _ticketTypeService.GetByWorkshopAsync(workshopId);
            return Ok(ticketTypes);
        }

        // GET: api/tickettype/talkevent/{talkEventId}/available
        [HttpGet("talkevent/{talkEventId:int}/available")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<TicketTypeDto>>> GetAvailableByTalkEvent(int talkEventId)
        {
            var ticketTypes = await _ticketTypeService.GetAvailableByTalkEventAsync(talkEventId);
            return Ok(ticketTypes);
        }

        // GET: api/tickettype/workshop/{workshopId}/available
        [HttpGet("workshop/{workshopId:int}/available")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<TicketTypeDto>>> GetAvailableByWorkshop(int workshopId)
        {
            var ticketTypes = await _ticketTypeService.GetAvailableByWorkshopAsync(workshopId);
            return Ok(ticketTypes);
        }

        // POST: api/tickettype/talkevent
        [HttpPost("talkevent")]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<ActionResult<TicketTypeDto>> CreateForTalkEvent(
            [FromBody] CreateTicketTypeForTalkEventDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var result = await _ticketTypeService.CreateForTalkEventAsync(dto, userId);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Id },
                    result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ticket type for talk event");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // POST: api/tickettype/workshop
        [HttpPost("workshop")]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<ActionResult<TicketTypeDto>> CreateForWorkshop(
            [FromBody] CreateTicketTypeForWorkshopDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var result = await _ticketTypeService.CreateForWorkshopAsync(dto, userId);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Id },
                    result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ticket type for workshop");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // PUT: api/tickettype/{id}
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<ActionResult<TicketTypeDto>> Update(
            int id,
            [FromBody] UpdateTicketTypeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var result = await _ticketTypeService.UpdateAsync(id, dto, userId);

                if (result == null)
                    return NotFound($"Ticket type with id {id} not found");

                return Ok(result);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ticket type {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // DELETE: api/tickettype/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(
            int id,
            [FromQuery] string reason = "No reason provided")
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var result = await _ticketTypeService.DeleteAsync(id, userId, reason);

                if (!result)
                    return NotFound($"Ticket type with id {id} not found");

                return NoContent();
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ticket type {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // GET: api/tickettype/{id}/availability
        [HttpGet("{id:int}/availability")]
        [AllowAnonymous]
        public async Task<ActionResult<AvailabilityResponse>> CheckAvailability(
            int id,
            [FromQuery] int quantity = 1)
        {
            if (quantity < 1)
                return BadRequest("Quantity must be at least 1");

            var isAvailable = await _ticketTypeService.CheckAvailabilityAsync(id, quantity);

            return Ok(new AvailabilityResponse
            {
                TicketTypeId = id,
                RequestedQuantity = quantity,
                IsAvailable = isAvailable
            });
        }

        // PATCH: api/tickettype/{id}/sold-quantity
        [HttpPatch("{id:int}/sold-quantity")]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<ActionResult> UpdateSoldQuantity(
            int id,
            [FromBody] UpdateSoldQuantityRequest request)
        {
            if (request.Quantity < 1)
                return BadRequest("Quantity must be at least 1");

            try
            {
                var result = await _ticketTypeService.UpdateSoldQuantityAsync(
                    id,
                    request.Quantity,
                    request.IsIncrement);

                if (!result)
                    return BadRequest("Failed to update sold quantity");

                return Ok(new { message = "Sold quantity updated successfully" });
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating sold quantity for ticket type {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // GET: api/tickettype/{id}/revenue
        [HttpGet("{id:int}/revenue")]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<ActionResult<RevenueResponse>> GetRevenue(int id)
        {
            var revenue = await _ticketTypeService.GetRevenueAsync(id);

            return Ok(new RevenueResponse
            {
                TicketTypeId = id,
                TotalRevenue = revenue
            });
        }

        // GET: api/tickettype/talkevent/{talkEventId}/total-sold
        [HttpGet("talkevent/{talkEventId:int}/total-sold")]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<ActionResult<TotalSoldResponse>> GetTotalSoldByTalkEvent(int talkEventId)
        {
            var totalSold = await _ticketTypeService.GetTotalSoldByTalkEventAsync(talkEventId);

            return Ok(new TotalSoldResponse
            {
                EventId = talkEventId,
                EventType = "TalkEvent",
                TotalSold = totalSold
            });
        }

        // GET: api/tickettype/workshop/{workshopId}/total-sold
        [HttpGet("workshop/{workshopId:int}/total-sold")]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<ActionResult<TotalSoldResponse>> GetTotalSoldByWorkshop(int workshopId)
        {
            var totalSold = await _ticketTypeService.GetTotalSoldByWorkshopAsync(workshopId);

            return Ok(new TotalSoldResponse
            {
                EventId = workshopId,
                EventType = "Workshop",
                TotalSold = totalSold
            });
        }
    }

   
}
