using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Cart
{
    public class CartDto
    {
        public int Id { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public decimal SubTotal { get; set; }
        public int TotalItems { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
    public class CartItemDto
    {
        public int Id { get; set; }
        public int MerchandiseId { get; set; }
        public string MerchandiseName { get; set; }
        public string MerchandiseSKU { get; set; }
        public string? ThumbnailImage { get; set; }
        public int? VariantId { get; set; }
        public string? VariantName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int AvailableStock { get; set; }
    }

    public class AddToCartDto
    {
        public int MerchandiseId { get; set; }
        public int? VariantId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartItemDto
    {
        public int Quantity { get; set; }
    }
}

