using Application.DTOs.Order;
using Domain.Entities.MerchandiseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto dto);
        Task<IEnumerable<OrderListDto>> GetUserOrdersAsync(int userId, OrderStatus? status = null);
        Task<OrderDto> GetOrderDetailsAsync(int userId, int orderId);
        Task<OrderDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto);
        Task<OrderDto> CancelOrderAsync(int userId, int orderId, string reason);
        Task<IEnumerable<OrderListDto>> GetOrdersByStatusAsync(OrderStatus status);
        Task<bool> UserOwnsOrderAsync(int userId, string orderNumber);

        Task<PagedResult<OrderListDto>> GetAllOrdersAsync(OrderFilterDto filter);
        Task<PagedResult<OrderListDto>> GetUserOrdersPagedAsync(int userId, OrderFilterDto filter);
        Task<List<UserOrderStatDto>> GetUserOrderStatisticsAsync(UserOrderStatFilterDto filter);

    }
}
