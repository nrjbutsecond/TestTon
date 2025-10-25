using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.ShippingConfig;
using Domain.Entities.MerchandiseEntity;
namespace Application.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } // String for API response
        public string StatusDisplay { get; set; } //for readable status
        public ShippingAddressDto ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime OrderDate { get; set; }
        public string? CancellationReason { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderListDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string StatusDisplay { get; set; }
        public DateTime OrderDate { get; set; }
        public int ItemCount { get; set; }
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public string MerchandiseName { get; set; }
        public string MerchandiseSKU { get; set; }
        public string? VariantName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? ThumbnailImage { get; set; }
    }

    public class CreateOrderDto
    {
        public ShippingAddressDto ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }
        public string? Note { get; set; }
    }

    //public class ShippingAddressDto
    //{
    //    public string FullName { get; set; }
    //    public string Phone { get; set; }
    //    public string Address { get; set; }
    //    public string City { get; set; }
    //    public string District { get; set; }
    //    public string Ward { get; set; }
    //    public string? Note { get; set; }
    //}

    public class UpdateOrderStatusDto
    {
        public OrderStatus Status { get; set; }
        public string? CancellationReason { get; set; }
    }

    public class OrderFilterDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? OrderNumber { get; set; }
        public OrderStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? SortBy { get; set; } = "OrderDate";
        public bool SortDescending { get; set; } = true;
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    public class UserOrderStatDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalCost { get; set; }
    }

    public class UserOrderStatFilterDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }


}
