using Application.DTOs.ShippingConfig;
using Application.Interfaces;
using Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TON.Controllers
{
    [ApiController]
    [Route("api/shipping")]
    public class ShippingController : ControllerBase
    {
        private readonly IEnhancedShippingService _shippingService;
        private readonly IShippingConfigService _configService;
        private readonly IOrderService _orderService;

        public ShippingController(
            IEnhancedShippingService shippingService,
            IShippingConfigService configService,
            IOrderService orderService)
        {
            _shippingService = shippingService;
            _configService = configService;
            _orderService = orderService;
        }

        // Calculate shipping with best provider
        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateShipping([FromBody] CalculateShippingRequest request)
        {
            var quote = await _shippingService.CalculateShippingAsync(request);
            return Ok(quote);
        }

        // Get quotes from multiple providers
        [HttpPost("quotes")]
        public async Task<IActionResult> GetMultipleQuotes([FromBody] CalculateShippingRequest request)
        {
            var quotes = await _shippingService.GetMultipleQuotesAsync(request);
            return Ok(quotes);
        }

        // Simple fee calculation (legacy support)
        [HttpGet("fee")]
        public async Task<IActionResult> GetShippingFee(
            [FromQuery] string city,
            [FromQuery] decimal subTotal,
            [FromQuery] int quantity)
        {
            // Use old service for backward compatibility
            var fee = await _configService.CalculateShippingFeeAsync(city, subTotal, quantity);
            return Ok(new { fee });
        }

        // Fee with details (legacy support)
        [HttpGet("fee-details")]
        public async Task<IActionResult> GetShippingFeeDetails(
            [FromQuery] string city,
            [FromQuery] decimal subTotal,
            [FromQuery] int quantity)
        {
            var details = await _configService.GetShippingFeeDetailsAsync(city, subTotal, quantity);
            return Ok(details);
        }

        // Get available shipping methods
        [HttpGet("methods")]
        public async Task<IActionResult> GetAvailableMethods(
            [FromQuery] string fromCity = "Ho Chi Minh",
            [FromQuery] string toCity = "Ha Noi")
        {
            var methods = await _shippingService.GetAvailableMethodsAsync(fromCity, toCity);
            return Ok(methods);
        }

        // Create shipping order (requires auth)
        [HttpPost("order")]
        [Authorize]
        public async Task<IActionResult> CreateShippingOrder([FromBody] CreateShippingOrderDto request)
        {
            // Get current user
            var userId = GetUserId();

            // Validate user owns the order
            var userOwnsOrder = await _orderService.UserOwnsOrderAsync(userId, request.OrderNumber);
            if (!userOwnsOrder)
            {
                return StatusCode(403, new { message = "You do not have permission to create shipping for this order" });
            }

            // Create shipping order
            var result = await _shippingService.CreateShippingOrderAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // Track shipment
        [HttpGet("track/{trackingNumber}")]
        public async Task<IActionResult> TrackShipment(
            string trackingNumber,
            [FromQuery] string provider = "local")
        {
            var tracking = await _shippingService.TrackShipmentAsync(trackingNumber, provider);
            return Ok(tracking);
        }

        // Cancel shipping order (requires auth)
        [HttpPost("cancel/{shippingOrderId}")]
        [Authorize]
        public async Task<IActionResult> CancelShippingOrder(
            string shippingOrderId,
            [FromQuery] string provider)
        {
            var result = await _shippingService.CancelShippingOrderAsync(shippingOrderId, provider);
            return Ok(new { success = result });
        }

        // Get shipping config for a location (public info)
        //[HttpGet("config")]
        //public async Task<IActionResult> GetShippingConfig(
        //    [FromQuery] string city,
        //    [FromQuery] string? district = null,
        //    [FromQuery] string? ward = null)
        //{
        //    // Return public info only (thresholds, etc.)
        //    var config = await _configService.GetPublicConfigAsync(city, district, ward);
        //    return Ok(config);
        //}
        [HttpGet("config")]
        public async Task<IActionResult> GetShippingConfig(
    [FromQuery] string city,
    [FromQuery] string? district = null,
    [FromQuery] string? ward = null)
        {
            var configs = await _configService.GetAllConfigsAsync();

            // Find most specific config (same logic as shipping calculation)
            ShippingConfigDto? config = null;

            // Try ward level first
            if (!string.IsNullOrEmpty(ward))
            {
                config = configs.FirstOrDefault(c =>
                    c.City == city &&
                    c.District == district &&
                    c.Ward == ward &&
                    c.IsActive);
            }

            // Then district level
            if (config == null && !string.IsNullOrEmpty(district))
            {
                config = configs.FirstOrDefault(c =>
                    c.City == city &&
                    c.District == district &&
                    c.Ward == null &&
                    c.IsActive);
            }

            // Then city level
            if (config == null)
            {
                config = configs.FirstOrDefault(c =>
                    c.City == city &&
                    c.District == null &&
                    c.Ward == null &&
                    c.IsActive);
            }

            // Finally default
            if (config == null)
            {
                config = configs.FirstOrDefault(c => c.IsDefault && c.IsActive);
            }

            if (config == null)
                return NotFound("No shipping configuration found");

            // Return public info only
            return Ok(new
            {
                location = new
                {
                    city = config.City,
                    district = config.District,
                    ward = config.Ward
                },
                freeShippingThreshold = config.FreeShippingThreshold,
                bulkOrderThreshold = config.BulkOrderThreshold,
                message = $"Free shipping for orders over {config.FreeShippingThreshold:N0}đ"
            });
        }
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);
        }
    }
}