using Application.DTOs.ServicePlan;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TON.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicePlanController : ControllerBase
    {
        private readonly IServicePlanService _servicePlanService;
        private readonly ILogger<ServicePlanController> _logger;

        public ServicePlanController(
            IServicePlanService servicePlanService,
            ILogger<ServicePlanController> logger)
        {
            _servicePlanService = servicePlanService;
            _logger = logger;
        }

        // Public endpoints for viewing plans
        [HttpGet]
        public async Task<IActionResult> GetAllPlans()
        {
            var plans = await _servicePlanService.GetActiveServicePlansAsync();
            return Ok(plans);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlanById(int id)
        {
            var plan = await _servicePlanService.GetServicePlanByIdAsync(id);
            if (plan == null)
                return NotFound(new { message = "Service plan not found" });

            return Ok(plan);
        }

        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetPlanByCode(string code)
        {
            var plan = await _servicePlanService.GetServicePlanByCodeAsync(code);
            if (plan == null)
                return NotFound(new { message = "Service plan not found" });

            return Ok(plan);
        }

        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularPlans()
        {
            var plans = await _servicePlanService.GetPopularServicePlansAsync();
            return Ok(plans);
        }

        [HttpGet("comparison")]
        public async Task<IActionResult> GetPlanComparison()
        {
            var comparison = await _servicePlanService.GetServicePlanComparisonAsync();
            return Ok(comparison);
        }

        // Admin endpoints for managing plans
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePlan([FromBody] CreateServicePlanDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var plan = await _servicePlanService.CreateServicePlanAsync(dto);
                return CreatedAtAction(nameof(GetPlanById), new { id = plan.Id }, plan);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePlan(int id, [FromBody] UpdateServicePlanDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var plan = await _servicePlanService.UpdateServicePlanAsync(id, dto);
                return Ok(plan);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Service plan not found" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePlan(int id)
        {
            try
            {
                var result = await _servicePlanService.DeleteServicePlanAsync(id);
                if (!result)
                    return NotFound(new { message = "Service plan not found" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivatePlan(int id)
        {
            var result = await _servicePlanService.ActivateServicePlanAsync(id);
            if (!result)
                return NotFound(new { message = "Service plan not found" });

            return Ok(new { message = "Service plan activated successfully" });
        }

        [HttpPost("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivatePlan(int id)
        {
            var result = await _servicePlanService.DeactivateServicePlanAsync(id);
            if (!result)
                return NotFound(new { message = "Service plan not found" });

            return Ok(new { message = "Service plan deactivated successfully" });
        }

        // User subscription endpoints
        [HttpGet("subscription/active")]
        [Authorize]
        public async Task<IActionResult> GetActiveSubscription()
        {
            var userId = GetUserId();
            var subscription = await _servicePlanService.GetActiveSubscriptionAsync(userId);

            if (subscription == null)
                return NotFound(new { message = "No active subscription found" });

            return Ok(subscription);
        }

        [HttpGet("subscription/history")]
        [Authorize]
        public async Task<IActionResult> GetSubscriptionHistory()
        {
            var userId = GetUserId();
            var history = await _servicePlanService.GetUserSubscriptionHistoryAsync(userId);
            return Ok(history);
        }

        [HttpPost("subscription")]
        [Authorize]
        public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            dto.UserId = GetUserId();

            try
            {
                var subscription = await _servicePlanService.CreateSubscriptionAsync(dto);
                return Ok(subscription);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("subscription/upgrade")]
        [Authorize]
        public async Task<IActionResult> UpgradeSubscription([FromBody] ServicePlanUpgradeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();

            try
            {
                var subscription = await _servicePlanService.UpgradeSubscriptionAsync(userId, dto);
                return Ok(subscription);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("subscription/{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelSubscription(int id)
        {
            var result = await _servicePlanService.CancelSubscriptionAsync(id);
            if (!result)
                return NotFound(new { message = "Subscription not found" });

            return Ok(new { message = "Subscription cancelled successfully" });
        }

        [HttpPost("subscription/{id}/renew")]
        [Authorize]
        public async Task<IActionResult> RenewSubscription(int id)
        {
            var result = await _servicePlanService.RenewSubscriptionAsync(id);
            if (!result)
                return NotFound(new { message = "Subscription not found" });

            return Ok(new { message = "Subscription renewal initiated" });
        }

        // Contract negotiation endpoints
        [HttpPost("contract-negotiation")]
        [Authorize]
        public async Task<IActionResult> RequestContractNegotiation([FromBody] CreateContractNegotiationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();

            try
            {
                var negotiation = await _servicePlanService.CreateContractNegotiationAsync(userId, dto);
                return Ok(negotiation);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("contract-negotiation/my")]
        [Authorize]
        public async Task<IActionResult> GetMyNegotiations()
        {
            var userId = GetUserId();
            var negotiations = await _servicePlanService.GetUserNegotiationsAsync(userId);
            return Ok(negotiations);
        }

        [HttpGet("contract-negotiation/pending")]
        [Authorize(Roles = "Admin,SalesStaff")]
        public async Task<IActionResult> GetPendingNegotiations()
        {
            var negotiations = await _servicePlanService.GetPendingNegotiationsAsync();
            return Ok(negotiations);
        }

        [HttpPut("contract-negotiation/{id}")]
        [Authorize(Roles = "Admin,SalesStaff")]
        public async Task<IActionResult> UpdateNegotiation(int id, [FromBody] UpdateContractNegotiationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var negotiation = await _servicePlanService.UpdateNegotiationStatusAsync(id, dto);
                return Ok(negotiation);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Contract negotiation not found" });
            }
        }

        [HttpPost("contract-negotiation/{id}/approve")]
        [Authorize(Roles = "Admin,SalesStaff")]
        public async Task<IActionResult> ApproveNegotiation(int id)
        {
            var approvedBy = User.Identity.Name;

            try
            {
                var negotiation = await _servicePlanService.ApproveNegotiationAsync(id, approvedBy);
                return Ok(negotiation);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Contract negotiation not found" });
            }
        }

        [HttpPost("contract-negotiation/{id}/reject")]
        [Authorize(Roles = "Admin,SalesStaff")]
        public async Task<IActionResult> RejectNegotiation(int id, [FromBody] RejectNegotiationDto dto)
        {
            var rejectedBy = User.Identity.Name;

            try
            {
                var negotiation = await _servicePlanService.RejectNegotiationAsync(id, rejectedBy, dto.Reason);
                return Ok(negotiation);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Contract negotiation not found" });
            }
        }

        // Validation endpoints
        [HttpGet("validate/can-upgrade/{planId}")]
        [Authorize]
        public async Task<IActionResult> CanUpgrade(int planId)
        {
            var userId = GetUserId();
            var canUpgrade = await _servicePlanService.CanUserUpgradeAsync(userId, planId);
            return Ok(new { canUpgrade });
        }

        [HttpGet("calculate/upgrade-cost/{planId}")]
        [Authorize]
        public async Task<IActionResult> CalculateUpgradeCost(int planId)
        {
            var userId = GetUserId();
            var cost = await _servicePlanService.CalculateUpgradeCostAsync(userId, planId);
            return Ok(new { upgradeCost = cost });
        }

        [HttpGet("validate/feature/{feature}")]
        [Authorize]
        public async Task<IActionResult> ValidateFeature(string feature)
        {
            var userId = GetUserId();
            var hasAccess = await _servicePlanService.ValidateServicePlanLimitsAsync(userId, feature);
            return Ok(new { hasAccess });
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }

    public class RejectNegotiationDto
    {
        public string Reason { get; set; }
    }
}