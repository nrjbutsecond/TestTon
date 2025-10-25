using Domain.Entities.MerchandiseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IOrderRepo : IRepo<OrderModel>
    {
        Task<OrderModel?> GetOrderWithItemsAsync(int orderId);
        Task<IEnumerable<OrderModel>> GetUserOrdersAsync(int userId, OrderStatus? status = null);
        Task<OrderModel?> GetByOrderNumberAsync(string orderNumber);
        Task<IEnumerable<OrderModel>> GetOrdersByStatusAsync(OrderStatus status);
        Task<bool> HasUserPurchasedMerchandiseAsync(int userId, int merchandiseId);

        Task<(List<OrderModel> Orders, int TotalCount)> GetOrdersPagedAsync(OrderFilter filter);
        Task<(List<OrderModel> Orders, int TotalCount)> GetUserOrdersPagedAsync(int userId, OrderFilter filter);

        Task<List<UserOrderStatistics>> GetUserOrderStatisticsAsync (DateTime? fromDate, DateTime? toDate);

    }
}
