using Domain.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.MerchandiseEntity
{
    public class OrderModel :BaseEntity
    {
        public int UserId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string ShippingAddress { get; set; } = string.Empty; // JSON
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string? CancellationReason { get; set; }

        // Navigation properties
        public virtual UserModel User { get; set; }
        public virtual ICollection<OrderItemModel> OrderItems { get; set; } = new List<OrderItemModel>();
    }



    public enum OrderStatus
    {
        Pending = 1,
        Processing = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5
    }
}