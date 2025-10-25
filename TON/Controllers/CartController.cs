using Application.DTOs.Cart;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TON.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var userId = GetUserId();
            var cart = await _cartService.AddToCartAsync(userId, dto);
            return Ok(cart);
        }

        [HttpPut("items/{itemId}")]
        public async Task<IActionResult> UpdateCartItem(int itemId, [FromBody] UpdateCartItemDto dto)
        {
            var userId = GetUserId();
            var cart = await _cartService.UpdateCartItemAsync(userId, itemId, dto);
            return Ok(cart);
        }

        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> RemoveFromCart(int itemId)
        {
            var userId = GetUserId();
            var cart = await _cartService.RemoveFromCartAsync(userId, itemId);
            return Ok(cart);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            var cart = await _cartService.ClearCartAsync(userId);
            return Ok(cart);
        }

       
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);
        }
    }
}