using Application.DTOs.Merchandise;
using Application.Interfaces;
using Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TON.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchandiseController : ControllerBase
    {
        private readonly IMerchandiseService _merchandiseService;
        private readonly IReviewService _reviewService;

        public MerchandiseController(
            IMerchandiseService merchandiseService,
            IReviewService reviewService)
        {
            _merchandiseService = merchandiseService;
            _reviewService = reviewService;
        }

        // ===== PUBLIC ENDPOINTS =====

        [HttpGet]
        public async Task<IActionResult> GetActiveProducts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _merchandiseService.GetActiveProductsAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _merchandiseService.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { message = "Merchandise not found" });

            var reviewStats = await _reviewService.GetReviewStatsAsync(id);
            product.AverageRating = reviewStats.AverageRating;
            product.ReviewCount = reviewStats.TotalReviews;

            return Ok(product);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest(new { message = "Search query required" });

            var products = await _merchandiseService.SearchAsync(q);
            return Ok(products);
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetByCategory(string category)
        {
            var products = await _merchandiseService.GetByCategoryAsync(category);
            return Ok(products);
        }

        [HttpGet("official")]
        public async Task<IActionResult> GetOfficialProducts()
        {
            var products = await _merchandiseService.GetOfficialProductsAsync();
            return Ok(products);
        }

        [HttpGet("{merchandiseId}/variants")]
        public async Task<IActionResult> GetVariants(int merchandiseId)
        {
            var variants = await _merchandiseService.GetVariantsByMerchandiseIdAsync(merchandiseId);
            return Ok(variants);
        }

        // ===== ORGANIZER ENDPOINTS =====

        [Authorize(Roles = "Organizer, Admin")]
        [HttpGet("my-merchandise")]
        public async Task<IActionResult> GetMyMerchandise()
        {
            var userId = GetUserId();
            var products = await _merchandiseService.GetBySellerIdAsync(userId);
            return Ok(products);
        }

        [Authorize(Roles = "Organizer, Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMerchandiseDto dto)
        {
            try
            {
                var userId = GetUserId();
                var product = await _merchandiseService.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Organizer, Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMerchandiseDto dto)
        {
            try
            {
                var userId = GetUserId();
                var product = await _merchandiseService.UpdateAsync(id, dto, userId);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [Authorize(Roles = "Organizer, Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = GetUserId();
                var result = await _merchandiseService.DeleteAsync(id, userId);
                if (!result)
                    return NotFound(new { message = "Merchandise not found" });

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [Authorize(Roles = "Organizer, Admin")]
        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var userId = GetUserId();
                var result = await _merchandiseService.ToggleActiveStatusAsync(id, userId);
                if (!result)
                    return NotFound(new { message = "Merchandise not found" });

                return Ok(new { message = "Status toggled successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        // ===== VARIANT ENDPOINTS =====

        [Authorize(Roles = "Organizer,SalesStaff,Admin")]
        [HttpPost("{merchandiseId}/variants")]
        public async Task<IActionResult> AddVariant(int merchandiseId, [FromBody] CreateVariantDto dto)
        {
            try
            {
                var userId = GetUserId();
                var variant = await _merchandiseService.AddVariantAsync(merchandiseId, dto, userId);
                return CreatedAtAction(nameof(GetVariants), new { merchandiseId }, variant);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Organizer,SalesStaff,Admin")]
        [HttpPut("variants/{variantId}")]
        public async Task<IActionResult> UpdateVariant(int variantId, [FromBody] UpdateVariantDto dto)
        {
            try
            {
                var userId = GetUserId();
                var variant = await _merchandiseService.UpdateVariantAsync(variantId, dto, userId);
                return Ok(variant);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Organizer,SalesStaff,Admin")]
        [HttpDelete("variants/{variantId}")]
        public async Task<IActionResult> DeleteVariant(int variantId)
        {
            var userId = GetUserId();
            var result = await _merchandiseService.DeleteVariantAsync(variantId, userId);
            if (!result)
                return NotFound(new { message = "Variant not found" });

            return NoContent();
        }

        // ===== ADMIN/SALES STAFF ENDPOINTS =====

        [Authorize(Roles = "SalesStaff,Admin")]
        [HttpGet("management")]
        public async Task<IActionResult> GetAllForManagement(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] bool? isOfficial = null)
        {
            var result = await _merchandiseService.GetAllForManagementAsync(pageNumber, pageSize, isOfficial);
            return Ok(result);
        }

        [Authorize(Roles = "SalesStaff,Admin")]
        [HttpPost("admin")]
        public async Task<IActionResult> AdminCreate(
            [FromBody] CreateMerchandiseDto dto,
            [FromQuery] int? sellerId = null,
            [FromQuery] bool isOfficial = false)
        {
            try
            {
                var product = await _merchandiseService.AdminCreateAsync(dto, sellerId, isOfficial);
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "SalesStaff,Admin")]
        [HttpPut("admin/{id}")]
        public async Task<IActionResult> AdminUpdate(int id, [FromBody] UpdateMerchandiseDto dto)
        {
            try
            {
                var product = await _merchandiseService.AdminUpdateAsync(id, dto);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "SalesStaff,Admin")]
        [HttpDelete("admin/{id}")]
        public async Task<IActionResult> AdminDelete(int id)
        {
            var result = await _merchandiseService.AdminDeleteAsync(id);
            if (!result)
                return NotFound(new { message = "Merchandise not found" });

            return NoContent();
        }

        [Authorize(Roles = "SalesStaff,Admin")]
        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockRequest request)
        {
            var result = await _merchandiseService.UpdateStockAsync(id, request.Quantity);
            if (!result)
                return NotFound(new { message = "Merchandise not found" });

            return Ok(new { message = "Stock updated successfully" });
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);
        }
    }

    public class UpdateStockRequest
    {
        public int Quantity { get; set; }
    }
}