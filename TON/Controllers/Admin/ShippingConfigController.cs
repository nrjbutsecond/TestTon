using Application.DTOs.ShippingConfig;
using Application.Interfaces;
using Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TON.Controllers.Admin
{

    [ApiController]
    [Route("api/admin/shipping-configs")]
    [Authorize(Roles = "Admin,SalesStaff")]
    public class ShippingConfigController : ControllerBase
    {
        private readonly IShippingConfigService _shippingConfigService;
        private readonly IEnhancedShippingService _enhancedShippingService;
        private readonly IShippingProviderFactoryService _providerFactory; 


        public ShippingConfigController(
            IShippingConfigService shippingConfigService,
            IEnhancedShippingService enhancedShippingService,
            IShippingProviderFactoryService providerFactory)
        {
            _shippingConfigService = shippingConfigService;
            _enhancedShippingService = enhancedShippingService;
            _providerFactory = providerFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllConfigs()
        {
            var configs = await _shippingConfigService.GetAllConfigsAsync();
            return Ok(configs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetConfig(int id)
        {
            var config = await _shippingConfigService.GetConfigByIdAsync(id);
            return Ok(config);
        }

        [HttpPost]
        public async Task<IActionResult> CreateConfig([FromBody] CreateShippingConfigDto dto)
        {
            var config = await _shippingConfigService.CreateConfigAsync(dto);
            return CreatedAtAction(nameof(GetConfig), new { id = config.Id }, config);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateConfig(int id, [FromBody] UpdateShippingConfigDto dto)
        {
            var config = await _shippingConfigService.UpdateConfigAsync(id, dto);
            return Ok(config);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfig(int id)
        {
            await _shippingConfigService.DeleteConfigAsync(id);
            return NoContent();
        }

        // Test calculation với local provider (backward compatibility)
        [HttpPost("calculate-local")]
        public async Task<IActionResult> CalculateLocalShippingFee([FromBody] CalculateShippingFeeRequest request)
        {
            var result = await _shippingConfigService.GetShippingFeeDetailsAsync(
                request.City,
                request.SubTotal,
                request.TotalQuantity);
            return Ok(result);
        }

        // Test calculation với any provider
        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateShippingFee([FromBody] AdminCalculateRequest request)
        {
            var shippingRequest = new CalculateShippingRequest
            {
                ToAddress = new ShippingAddressDto
                {
                    City = request.City,
                    District = request.District ?? "",
                    Ward = request.Ward ?? ""
                },
                SubTotal = request.SubTotal,
                TotalItems = request.TotalQuantity,
                TotalWeight = request.Weight ?? 1,
                PreferredProvider = request.Provider ?? "local"
            };

            var quote = await _enhancedShippingService.CalculateShippingAsync(shippingRequest);
            return Ok(quote);
        }


        //=================================//
        // Get all available providers for testing
        [HttpGet("providers")]
        public IActionResult GetAvailableProviders()
        {
            var providers = _providerFactory.GetAvailableProviders();
            return Ok(providers);
        }

        // Test all providers
        [HttpPost("test-providers")]
        public async Task<IActionResult> TestAllProviders([FromBody] AdminCalculateRequest request)
        {
            var shippingRequest = new CalculateShippingRequest
            {
                ToAddress = new ShippingAddressDto
                {
                    City = request.City,
                    District = request.District ?? "",
                    Ward = request.Ward ?? ""
                },
                SubTotal = request.SubTotal,
                TotalItems = request.TotalQuantity,
                TotalWeight = request.Weight ?? 1
            };

            var quotes = await _enhancedShippingService.GetMultipleQuotesAsync(shippingRequest);
            return Ok(quotes);
        }
    }

    public class CalculateShippingFeeRequest
    {
        public string City { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public int TotalQuantity { get; set; }
    }

    public class AdminCalculateRequest : CalculateShippingFeeRequest
    {
        public string? District { get; set; }
        public string? Ward { get; set; }
        public decimal? Weight { get; set; }
        public string? Provider { get; set; }
    }
}