using Application.DTOs.Order;
using Application.Interfaces;
using Domain.Entities.MerchandiseEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TON.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var userId = GetUserId();
            var order = await _orderService.CreateOrderAsync(userId, dto);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyOrders([FromQuery] OrderStatus? status = null)
        {
            var userId = GetUserId();
            var orders = await _orderService.GetUserOrdersAsync(userId, status);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var userId = GetUserId();
            var order = await _orderService.GetOrderDetailsAsync(userId, id);
            return Ok(order);
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id, [FromBody] CancelOrderDto dto)
        {
            var userId = GetUserId();
            var order = await _orderService.CancelOrderAsync(userId, id, dto.Reason);
            return Ok(order);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,SalesStaff")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            var order = await _orderService.UpdateOrderStatusAsync(id, dto);
            return Ok(order);
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,SalesStaff")]
        public async Task<IActionResult> GetOrdersByStatus(OrderStatus status)
        {
            var orders = await _orderService.GetOrdersByStatusAsync(status);
            return Ok(orders);
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);
        }



        [HttpGet("my-orders")]
        [Authorize]
        public async Task<IActionResult> GetMyOrders(
       [FromQuery] int pageNumber = 1,
       [FromQuery] int pageSize = 10,
       [FromQuery] OrderStatus? status = null)
        {
            var userId = GetUserId();
            var filter = new OrderFilterDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Status = status
            };

            var orders = await _orderService.GetUserOrdersPagedAsync(userId, filter);
            return Ok(orders);
        }

        // For admin/staff - get all orders
        [HttpGet("all")]
        [Authorize(Roles = "Admin,SalesStaff")]
        public async Task<IActionResult> GetAllOrders([FromQuery] OrderFilterDto filter)
        {
            var orders = await _orderService.GetAllOrdersAsync(filter);
            return Ok(orders);
        }

        [HttpGet("admin/order-stats/users")]
        [Authorize(Roles = "Admin,SalesStaff")]
        public async Task<IActionResult> GetUserOrderStats([FromQuery] UserOrderStatFilterDto filter)
        {
            var stats = await _orderService.GetUserOrderStatisticsAsync(filter);

            var response = new
            {
                summary = new
                {
                    totalUsers = stats.Count,
                    totalRevenue = stats.Sum(s => s.TotalCost),
                  //  averageRevenuePerUser = stats.Any() ? stats.Average(s => s.TotalCost) : 0
                },
                data = stats
            };

            return Ok(response);
        }

    }



    public class CancelOrderDto
    {
        public string Reason { get; set; } = string.Empty;
    }
}