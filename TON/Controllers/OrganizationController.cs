using Application.DTOs.Merchandise;
using Application.DTOs.Organization;
using Application.Interfaces.OrganizationServiceInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TON.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;
        private readonly ILogger<OrganizationController> _logger;

        // GET: api/organization
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrganizationListDto>>> GetOrganizations([FromQuery] OrganizationFilterDto filter)
        {
            try
            {
                var organizations = await _organizationService.GetOrganizationsAsync(filter);
                return Ok(organizations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting organizations");
                return StatusCode(500, "An error occurred while retrieving organizations");
            }
        }

        // GET: api/organization/featured
        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<OrganizationListDto>>> GetFeaturedOrganizations()
        {
            try
            {
                var organizations = await _organizationService.GetFeaturedOrganizationsAsync();
                return Ok(organizations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting featured organizations");
                return StatusCode(500, "An error occurred while retrieving featured organizations");
            }
        }

        // GET: api/organization/partners
        [HttpGet("partners")]
        public async Task<ActionResult<IEnumerable<OrganizationListDto>>> GetActivePartners()
        {
            try
            {
                var organizations = await _organizationService.GetActivePartnersAsync();
                return Ok(organizations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active partners");
                return StatusCode(500, "An error occurred while retrieving active partners");
            }
        }

        // GET: api/organization/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrganizationDto>> GetOrganization(int id)
        {
            try
            {
                var organization = await _organizationService.GetOrganizationByIdAsync(id);
                return Ok(organization);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Organization with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting organization {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the organization");
            }
        }

        // GET: api/organization/{id}/dashboard
        [HttpGet("{id}/dashboard")]
        [Authorize]
        public async Task<ActionResult<OrganizationDashboardDto>> GetOrganizationDashboard(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (!await _organizationService.IsMemberOfOrganizationAsync(userId, id))
                {
                    return Forbid("You don't have access to this organization");
                }

                var dashboard = await _organizationService.GetOrganizationDashboardAsync(id);
                return Ok(dashboard);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Organization with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting organization dashboard {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the dashboard");
            }
        }

        // POST: api/organization
        [HttpPost]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<ActionResult<OrganizationDto>> CreateOrganization([FromBody] CreateOrganizationDto dto)
        {
            try
            {
                var createdBy = User.Identity?.Name ?? "System";
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var organization = await _organizationService.CreateOrganizationAsync(dto, createdBy, userId);
                return CreatedAtAction(nameof(GetOrganization), new { id = organization.Id }, organization);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating organization");
                return StatusCode(500, "An error occurred while creating the organization");
            }
        }

        // PUT: api/organization/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<OrganizationDto>> UpdateOrganization(int id, [FromBody] UpdateOrganizationDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (!await _organizationService.IsMemberOfOrganizationAsync(userId, id))
                {
                    return Forbid("You don't have permission to update this organization");
                }

                var updatedBy = User.Identity?.Name ?? "System";
                var organization = await _organizationService.UpdateOrganizationAsync(id, dto, updatedBy);
                return Ok(organization);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Organization with ID {id} not found");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating organization {Id}", id);
                return StatusCode(500, "An error occurred while updating the organization");
            }
        }

        // DELETE: api/organization/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteOrganization(int id)
        {
            try
            {
                var deletedBy = User.Identity?.Name ?? "System";
                await _organizationService.DeleteOrganizationAsync(id, deletedBy);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Organization with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting organization {Id}", id);
                return StatusCode(500, "An error occurred while deleting the organization");
            }
        }

        // MEMBER MANAGEMENT

        // GET: api/organization/{id}/members
        [HttpGet("{id}/members")]
        public async Task<ActionResult<IEnumerable<OrganizationMemberDto>>> GetOrganizationMembers(int id)
        {
            try
            {
                var members = await _organizationService.GetOrganizationMembersAsync(id);
                return Ok(members);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting organization members {Id}", id);
                return StatusCode(500, "An error occurred while retrieving organization members");
            }
        }

        // POST: api/organization/{id}/members
        [HttpPost("{id}/members")]
        [Authorize]
        public async Task<ActionResult<OrganizationMemberDto>> AddMember(int id, [FromBody] AddOrganizationMemberDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var addedBy = User.Identity?.Name ?? "System";
                var member = await _organizationService.AddMemberAsync(id, dto, addedBy, userId);
                return Ok(member);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding member to organization {Id}", id);
                return StatusCode(500, "An error occurred while adding the member");
            }
        }

        // PUT: api/organization/{id}/members/role
        [HttpPut("{id}/members/role")]
        [Authorize]
        public async Task<ActionResult> UpdateMemberRole(int id, [FromBody] UpdateMemberRoleDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var updatedBy = User.Identity?.Name ?? "System";
                await _organizationService.UpdateMemberRoleAsync(dto, updatedBy, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Member not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating member role");
                return StatusCode(500, "An error occurred while updating member role");
            }
        }

        // DELETE: api/organization/{id}/members/{memberId}
        [HttpDelete("{id}/members/{memberId}")]
        [Authorize]
        public async Task<ActionResult> RemoveMember(int id, int memberId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var removedBy = User.Identity?.Name ?? "System";
                await _organizationService.RemoveMemberAsync(id, memberId, removedBy, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Member not found in organization");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing member from organization");
                return StatusCode(500, "An error occurred while removing the member");
            }
        }

        // PARTNERSHIP APPLICATIONS

        // GET: api/organization/applications/pending
        [HttpGet("applications/pending")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<PartnershipApplicationDto>>> GetPendingApplications()
        {
            try
            {
                var applications = await _organizationService.GetPendingApplicationsAsync();
                return Ok(applications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending applications");
                return StatusCode(500, "An error occurred while retrieving pending applications");
            }
        }

        // GET: api/organization/{id}/applications
        [HttpGet("{id}/applications")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PartnershipApplicationDto>>> GetOrganizationApplications(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (!await _organizationService.IsMemberOfOrganizationAsync(userId, id))
                {
                    return Forbid("You don't have access to this organization's applications");
                }

                var applications = await _organizationService.GetOrganizationApplicationsAsync(id);
                return Ok(applications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting organization applications {Id}", id);
                return StatusCode(500, "An error occurred while retrieving applications");
            }
        }

        // POST: api/organization/applications
        [HttpPost("applications")]
        [Authorize]
        public async Task<ActionResult<PartnershipApplicationDto>> CreatePartnershipApplication([FromBody] CreatePartnershipApplicationDto dto)
        {
            try
            {
                var createdBy = User.Identity?.Name ?? "System";
                var application = await _organizationService.CreatePartnershipApplicationAsync(dto, createdBy);
                return Ok(application);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating partnership application");
                return StatusCode(500, "An error occurred while creating the application");
            }
        }

        // PUT: api/organization/applications/review
        [HttpPut("applications/review")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ReviewPartnershipApplication([FromBody] ReviewPartnershipApplicationDto dto)
        {
            try
            {
                var reviewerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var reviewedBy = User.Identity?.Name ?? "System";
                await _organizationService.ReviewPartnershipApplicationAsync(dto, reviewerId, reviewedBy);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Application not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reviewing partnership application");
                return StatusCode(500, "An error occurred while reviewing the application");
            }
        }

        // PUT: api/organization/{id}/partnership/upgrade
        [HttpPut("{id}/partnership/upgrade")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpgradePartnershipTier(int id, [FromBody] string tier)
        {
            try
            {
                var upgradedBy = User.Identity?.Name ?? "System";
                await _organizationService.UpgradePartnershipTierAsync(id, tier, upgradedBy);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Organization with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upgrading partnership tier");
                return StatusCode(500, "An error occurred while upgrading partnership tier");
            }
        }

        // PUT: api/organization/{id}/partnership/renew
        [HttpPut("{id}/partnership/renew")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RenewPartnership(int id, [FromBody] int months)
        {
            try
            {
                var renewedBy = User.Identity?.Name ?? "System";
                await _organizationService.RenewPartnershipAsync(id, months, renewedBy);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Organization with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error renewing partnership");
                return StatusCode(500, "An error occurred while renewing partnership");
            }
        }

        // ACTIVITIES & STATISTICS

        // GET: api/organization/{id}/activities
        [HttpGet("{id}/activities")]
        public async Task<ActionResult<IEnumerable<OrganizationActivityDto>>> GetOrganizationActivities(int id, [FromQuery] int take = 10)
        {
            try
            {
                var activities = await _organizationService.GetOrganizationActivitiesAsync(id, take);
                return Ok(activities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting organization activities {Id}", id);
                return StatusCode(500, "An error occurred while retrieving activities");
            }
        }

        // GET: api/organization/{id}/statistics
        [HttpGet("{id}/statistics")]
        public async Task<ActionResult<IEnumerable<OrganizationStatisticsDto>>> GetOrganizationStatistics(int id, [FromQuery] int? year = null)
        {
            try
            {
                var targetYear = year ?? DateTime.UtcNow.Year;
                var statistics = await _organizationService.GetOrganizationStatisticsAsync(id, targetYear);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting organization statistics {Id}", id);
                return StatusCode(500, "An error occurred while retrieving statistics");
            }
        }

        // POST: api/organization/{id}/statistics/update
        [HttpPost("{id}/statistics/update")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateOrganizationStatistics(int id)
        {
            try
            {
                await _organizationService.UpdateOrganizationStatisticsAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating organization statistics {Id}", id);
                return StatusCode(500, "An error occurred while updating statistics");
            }
        }

        // RELATED CONTENT

        // GET: api/organization/{id}/events
        [HttpGet("{id}/events")]
        public async Task<ActionResult<IEnumerable<TalkEventDto>>> GetOrganizationEvents(int id)
        {
            try
            {
                var events = await _organizationService.GetOrganizationEventsAsync(id);
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting organization events {Id}", id);
                return StatusCode(500, "An error occurred while retrieving events");
            }
        }

        // GET: api/organization/{id}/merchandise
        [HttpGet("{id}/merchandise")]
        public async Task<ActionResult<IEnumerable<MerchandiseDto>>> GetOrganizationMerchandise(int id)
        {
            try
            {
                var merchandise = await _organizationService.GetOrganizationMerchandiseAsync(id);
                return Ok(merchandise);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting organization merchandise {Id}", id);
                return StatusCode(500, "An error occurred while retrieving merchandise");
            }
        }

        // USER ORGANIZATIONS

        // GET: api/organization/user/{userId}
        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrganizationListDto>>> GetUserOrganizations(int userId)
        {
            try
            {
                var requestingUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (requestingUserId != userId && !User.IsInRole("Admin"))
                {
                    return Forbid("You can only view your own organizations");
                }

                var organizations = await _organizationService.GetUserOrganizationsAsync(userId);
                return Ok(organizations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user organizations {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving user organizations");
            }
        }

        // MEDIA

        // PUT: api/organization/{id}/media
        [HttpPut("{id}/media")]
        [Authorize]
        public async Task<ActionResult> UpdateOrganizationMedia(int id, [FromBody] UpdateOrganizationMediaDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (!await _organizationService.IsMemberOfOrganizationAsync(userId, id))
                {
                    return Forbid("You don't have permission to update this organization");
                }

                dto.OrganizationId = id;
                await _organizationService.UpdateOrganizationMediaAsync(dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Organization with ID {id} not found");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating organization media {Id}", id);
                return StatusCode(500, "An error occurred while updating media");
            }
        }

        // POST: api/organization/{id}/activities/log
        [HttpPost("{id}/activities/log")]
        [Authorize]
        public async Task<ActionResult> LogActivity(int id, [FromBody] LogActivityDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (!await _organizationService.IsMemberOfOrganizationAsync(userId, id))
                {
                    return Forbid("You don't have permission to log activities for this organization");
                }

                await _organizationService.LogActivityAsync(id, dto.ActivityType, dto.Description, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging activity for organization {Id}", id);
                return StatusCode(500, "An error occurred while logging activity");
            }
        }
    }

    // Supporting DTO for logging activity
    public class LogActivityDto
    {
        public string ActivityType { get; set; }
        public string Description { get; set; }
    }
}