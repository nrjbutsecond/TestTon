using Application.DTOs.Sponsor;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TON.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SponsorController : ControllerBase
    {
        private readonly ISponsorService _sponsorService;

        public SponsorController(ISponsorService sponsorService)
        {
            _sponsorService = sponsorService;
        }

        /// <summary>
        /// Get active sponsors for public display
        /// </summary>
        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<ActionResult<List<PublicSponsorDto>>> GetPublicSponsors()
        {
            var sponsors = await _sponsorService.GetActiveSponsorsForPublicAsync();
            return Ok(sponsors);
        }

        /// <summary>
        /// Get sponsor by ID (Admin only)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SponsorDto>> GetById(int id)
        {
            var sponsor = await _sponsorService.GetByIdAsync(id);
            if (sponsor == null)
                return NotFound($"Sponsor with ID {id} not found.");

            return Ok(sponsor);
        }

        /// <summary>
        /// Get paginated sponsors with filters (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SponsorListDto>> GetPaginated([FromQuery] SponsorFilterDto filter)
        {
            var result = await _sponsorService.GetPaginatedAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// Create new sponsor (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SponsorDto>> Create([FromBody] CreateSponsorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var sponsor = await _sponsorService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = sponsor.Id }, sponsor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Update sponsor (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SponsorDto>> Update(int id, [FromBody] UpdateSponsorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var sponsor = await _sponsorService.UpdateAsync(id, dto);
                if (sponsor == null)
                    return NotFound($"Sponsor with ID {id} not found.");

                return Ok(sponsor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete sponsor (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _sponsorService.DeleteAsync(id);
            if (!result)
                return NotFound($"Sponsor with ID {id} not found.");

            return NoContent();
        }

        /// <summary>
        /// Bulk operations on sponsors (Admin only)
        /// </summary>
        [HttpPost("bulk")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> BulkOperation([FromBody] BulkSponsorOperationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _sponsorService.BulkOperationAsync(dto);
                return Ok(new { message = $"Bulk {dto.Operation} completed successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Update display order for sponsors (Admin only)
        /// </summary>
        [HttpPut("display-order")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateDisplayOrder([FromBody] UpdateDisplayOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _sponsorService.UpdateDisplayOrderAsync(dto);
            return Ok(new { message = "Display order updated successfully." });
        }

        /// <summary>
        /// Get sponsor statistics (Admin only)
        /// </summary>
        [HttpGet("statistics")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SponsorStatisticsDto>> GetStatistics()
        {
            var statistics = await _sponsorService.GetStatisticsAsync();
            return Ok(statistics);
        }

        /// <summary>
        /// Get sponsors with expiring contracts (Admin only)
        /// </summary>
        [HttpGet("expiring-contracts")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<SponsorDto>>> GetExpiringContracts([FromQuery] int daysBeforeExpiry = 30)
        {
            var sponsors = await _sponsorService.GetExpiringContractsAsync(daysBeforeExpiry);
            return Ok(sponsors);
        }

        /// <summary>
        /// Check if sponsor name is unique (Admin only)
        /// </summary>
        [HttpGet("check-name")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> CheckNameUnique([FromQuery] string name, [FromQuery] int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name is required.");

            var isUnique = await _sponsorService.IsNameUniqueAsync(name, excludeId);
            return Ok(new { isUnique });
        }

        /// <summary>
        /// Check if sponsor email is unique (Admin only)
        /// </summary>
        [HttpGet("check-email")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> CheckEmailUnique([FromQuery] string email, [FromQuery] int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var isUnique = await _sponsorService.IsEmailUniqueAsync(email, excludeId);
            return Ok(new { isUnique });
        }
    }
}