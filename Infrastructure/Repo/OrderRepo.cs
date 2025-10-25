using Domain.Entities.MerchandiseEntity;
using Domain.Interface;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;

namespace Infrastructure.Repo
{
    public class OrderRepo: Repo<OrderModel>, IOrderRepo
    {
        public OrderRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<OrderModel?> GetOrderWithItemsAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Merchandise)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Variant)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<IEnumerable<OrderModel>> GetUserOrdersAsync(int userId, OrderStatus? status = null)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Merchandise)
                .Where(o => o.UserId == userId);

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            return await query
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<OrderModel?> GetByOrderNumberAsync(string orderNumber)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }

        public async Task<IEnumerable<OrderModel>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.User)
                .Where(o => o.Status == status)
                .OrderBy(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<bool> HasUserPurchasedMerchandiseAsync(int userId, int merchandiseId)
        {
            return await _context.Orders
                .AnyAsync(o => o.UserId == userId &&
                    o.Status != OrderStatus.Cancelled &&
                    o.OrderItems.Any(oi => oi.MerchandiseId == merchandiseId));
        }
        public async Task<(List<OrderModel> Orders, int TotalCount)> GetOrdersPagedAsync(OrderFilter filter)
        {
            var query = BuildOrderQuery(filter);

            var totalCount = await query.CountAsync();

            query = ApplySorting(query, filter.SortBy, filter.SortDescending);

            var orders = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (orders, totalCount);
        }

        public async Task<(List<OrderModel> Orders, int TotalCount)> GetUserOrdersPagedAsync(int userId, OrderFilter filter)
        {
            var query = BuildOrderQuery(filter)
                .Where(o => o.UserId == userId);

            var totalCount = await query.CountAsync();

            query = ApplySorting(query, filter.SortBy, filter.SortDescending);

            var orders = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (orders, totalCount);
        }

        private IQueryable<OrderModel> BuildOrderQuery(OrderFilter filter)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Merchandise)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.OrderNumber))
                query = query.Where(o => o.OrderNumber.Contains(filter.OrderNumber));

            if (filter.Status.HasValue)
                query = query.Where(o => o.Status == filter.Status.Value);

            if (filter.FromDate.HasValue)
                query = query.Where(o => o.OrderDate >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(o => o.OrderDate <= filter.ToDate.Value);

            if (filter.MinAmount.HasValue)
                query = query.Where(o => o.TotalAmount >= filter.MinAmount.Value);

            if (filter.MaxAmount.HasValue)
                query = query.Where(o => o.TotalAmount <= filter.MaxAmount.Value);

            if (!string.IsNullOrEmpty(filter.CustomerName))
                query = query.Where(o => o.User.FullName.Contains(filter.CustomerName));

            if (!string.IsNullOrEmpty(filter.CustomerPhone))
                query = query.Where(o => o.User.Phone.Contains(filter.CustomerPhone));

            return query;
        }


        private IQueryable<OrderModel> ApplySorting(IQueryable<OrderModel> query, string? sortBy, bool descending)
        {
            return sortBy?.ToLower() switch
            {
                "ordernumber" => descending
                    ? query.OrderByDescending(o => o.OrderNumber)
                    : query.OrderBy(o => o.OrderNumber),
                "totalamount" => descending
                    ? query.OrderByDescending(o => o.TotalAmount)
                    : query.OrderBy(o => o.TotalAmount),
                "status" => descending
                    ? query.OrderByDescending(o => o.Status)
                    : query.OrderBy(o => o.Status),
                "customer" => descending
                    ? query.OrderByDescending(o => o.User.FullName)
                    : query.OrderBy(o => o.User.FullName),
                _ => descending
                    ? query.OrderByDescending(o => o.OrderDate)
                    : query.OrderBy(o => o.OrderDate)
            };
        }

        public async Task<List<UserOrderStatistics>> GetUserOrderStatisticsAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Orders
                .Where(o => !o.IsDeleted && o.Status != OrderStatus.Cancelled)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(o => o.OrderDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(o => o.OrderDate <= toDate.Value);

            var stats = await query
                .Include(o => o.User)
                .GroupBy(o => new { o.UserId, o.User.FullName, o.User.Email })
                .Select(g => new UserOrderStatistics
                {
                    UserId = g.Key.UserId,
                    UserName = g.Key.FullName,
                    Email = g.Key.Email,
                    TotalOrders = g.Count(),
                    TotalCost = g.Sum(o => o.TotalAmount)
                })
                .OrderByDescending(s => s.TotalCost)
                .ToListAsync();

            return stats;
        }
    }
}