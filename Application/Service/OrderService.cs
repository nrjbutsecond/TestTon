using Application.DTOs.Order;
using Application.DTOs.ShippingConfig;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities.MerchandiseEntity;
using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Service
{
    public class OrderService : IOrderService

    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICartService _cartService;
        private readonly IShippingConfigService _shippingConfigService;
        private readonly IEnhancedShippingService _shippingService;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, ICartService cartService, IShippingConfigService shippingConfigService, IEnhancedShippingService shippingService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cartService = cartService;
            _shippingConfigService = shippingConfigService;
            _shippingService = shippingService;
        }

        public async Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto dto)
        {
            // Get cart
            var cart = await _unitOfWork.Carts.GetActiveCartByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
                throw new InvalidOperationException("Cart is empty");

            // Validate stock availability
            foreach (var cartItem in cart.CartItems)
            {
                var merchandise = cartItem.Merchandise;
                var availableStock = cartItem.VariantId.HasValue
                    ? cartItem.Variant!.StockQuantity
                    : merchandise.StockQuantity - merchandise.ReservedQuantity;

                if (cartItem.Quantity > availableStock)
                    throw new InvalidOperationException($"Insufficient stock for {merchandise.Name}");
            }

            // Calculate totals
            decimal subTotal = cart.CartItems.Sum(ci => ci.UnitPrice * ci.Quantity);
            decimal shippingFee = await CalculateShippingFeeAsync(dto.ShippingAddress, cart.CartItems);

            // Create order
            var order = new OrderModel
            {
                UserId = userId,
                OrderNumber = GenerateOrderNumber(),
                SubTotal = subTotal,
                ShippingFee = shippingFee,
                TotalAmount = subTotal + shippingFee,
                Status = OrderStatus.Pending,
                ShippingAddress = JsonSerializer.Serialize(dto.ShippingAddress),
                PaymentMethod = dto.PaymentMethod,
                OrderDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId.ToString()
            };

            // Create order items and update stock
            foreach (var cartItem in cart.CartItems)
            {
                var orderItem = new OrderItemModel
                {
                    MerchandiseId = cartItem.MerchandiseId,
                    VariantId = cartItem.VariantId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice,
                    TotalPrice = cartItem.UnitPrice * cartItem.Quantity,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId.ToString()
                };

                order.OrderItems.Add(orderItem);

                // Update stock
                var merchandise = cartItem.Merchandise;
                merchandise.ReservedQuantity -= cartItem.Quantity;
                merchandise.StockQuantity -= cartItem.Quantity;

                if (cartItem.VariantId.HasValue)
                {
                    cartItem.Variant!.StockQuantity -= cartItem.Quantity;
                }
            }

            await _unitOfWork.Orders.AddAsync(order);

            // Clear cart
            cart.CartItems.Clear();

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderListDto>> GetUserOrdersAsync(int userId, OrderStatus? status = null)
        {
            var orders = await _unitOfWork.Orders.GetUserOrdersAsync(userId, status);
            return _mapper.Map<IEnumerable<OrderListDto>>(orders);
        }

        public async Task<OrderDto> GetOrderDetailsAsync(int userId, int orderId)
        {
            var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(orderId);

            if (order == null || order.UserId != userId)
                throw new InvalidOperationException("Order not found");

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto)
        {
            var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(orderId);
            if (order == null)
                throw new InvalidOperationException("Order not found");

            // Validate status transition
            ValidateStatusTransition(order.Status, dto.Status);

            order.Status = dto.Status;
            order.UpdatedAt = DateTime.UtcNow;

            if (dto.Status == OrderStatus.Cancelled)
            {
                order.CancellationReason = dto.CancellationReason;

                // Restore stock
                foreach (var item in order.OrderItems)
                {
                    item.Merchandise.StockQuantity += item.Quantity;

                    if (item.VariantId.HasValue && item.Variant != null)
                    {
                        item.Variant.StockQuantity += item.Quantity;
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CancelOrderAsync(int userId, int orderId, string reason)
        {
            var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(orderId);

            if (order == null || order.UserId != userId)
                throw new InvalidOperationException("Order not found");

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Processing)
                throw new InvalidOperationException("Order cannot be cancelled in current status");

            var dto = new UpdateOrderStatusDto
            {
                Status = OrderStatus.Cancelled,
                CancellationReason = reason
            };

            return await UpdateOrderStatusAsync(orderId, dto);
        }

        public async Task<IEnumerable<OrderListDto>> GetOrdersByStatusAsync(OrderStatus status)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByStatusAsync(status);
            return _mapper.Map<IEnumerable<OrderListDto>>(orders);
        }

        private void ValidateStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            var validTransitions = currentStatus switch
            {
                OrderStatus.Pending => new[] { OrderStatus.Processing, OrderStatus.Cancelled },
                OrderStatus.Processing => new[] { OrderStatus.Shipped, OrderStatus.Cancelled },
                OrderStatus.Shipped => new[] { OrderStatus.Delivered },
                OrderStatus.Delivered => Array.Empty<OrderStatus>(),
                OrderStatus.Cancelled => Array.Empty<OrderStatus>(),
                _ => Array.Empty<OrderStatus>()
            };

            if (!validTransitions.Contains(newStatus))
                throw new InvalidOperationException($"Invalid status transition from {currentStatus} to {newStatus}");
        }
        public async Task<bool> UserOwnsOrderAsync(int userId, string orderNumber)
        {
            var order = await _unitOfWork.Orders
                .FindAsync(o => o.OrderNumber == orderNumber && !o.IsDeleted);

            return order.Any() && order.First().UserId == userId;
        }
        private string GenerateOrderNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
            return $"ORD{timestamp}{random}";
        }
        //private async Task<decimal> CalculateShippingFeeAsync(ShippingAddressDto address, IEnumerable<CartItemModel> items)
        //{
        //    var subTotal = items.Sum(i => i.UnitPrice * i.Quantity);
        //    var totalQuantity = items.Sum(i => i.Quantity);

        //    return await _shippingConfigService.CalculateShippingFeeAsync(
        //        address.City,
        //        subTotal,
        //        totalQuantity);
        //}

        public async Task<PagedResult<OrderListDto>> GetAllOrdersAsync(OrderFilterDto filterDto)
        {
            var filter = _mapper.Map<OrderFilter>(filterDto);

            var (orders, totalCount) = await _unitOfWork.Orders.GetOrdersPagedAsync(filter);

            var orderDtos = _mapper.Map<List<OrderListDto>>(orders);

            return new PagedResult<OrderListDto>
            {
                Items = orderDtos,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PagedResult<OrderListDto>> GetUserOrdersPagedAsync(int userId, OrderFilterDto filterDto)
        {
            var filter = _mapper.Map<OrderFilter>(filterDto);

            var (orders, totalCount) = await _unitOfWork.Orders.GetUserOrdersPagedAsync(userId, filter);

            var orderDtos = _mapper.Map<List<OrderListDto>>(orders);

            return new PagedResult<OrderListDto>
            {
                Items = orderDtos,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<List<UserOrderStatDto>> GetUserOrderStatisticsAsync(UserOrderStatFilterDto filter)
        {
            var statistics = await _unitOfWork.Orders.GetUserOrderStatisticsAsync(
                filter.FromDate,
                filter.ToDate);

            return _mapper.Map<List<UserOrderStatDto>>(statistics);
        }








        private async Task<decimal> CalculateShippingFeeAsync(
       ShippingAddressDto address,
       IEnumerable<CartItemModel> items)
        {
            var request = new CalculateShippingRequest
            {
                ToAddress = address,
                SubTotal = items.Sum(i => i.UnitPrice * i.Quantity),
                TotalItems = items.Sum(i => i.Quantity),
                TotalWeight = CalculateTotalWeight(items),
                ServiceType = "Standard", // Or get from user selection
                CodAmount = 0 // Or calculate based on payment method
            };

            var quote = await _shippingService.CalculateShippingAsync(request);
            return quote.TotalFee;
        }

        private decimal CalculateTotalWeight(IEnumerable<CartItemModel> items)
        {
            // TODO: Add weight to merchandise or use default
            return items.Sum(i => i.Quantity * 0.5m); // Default 0.5kg per item
        }


    }
}