using Domain.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.MerchandiseEntity;

namespace Domain.Entities
{
    public class ProductModel :BaseEntity
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }
        public string? SKU { get; set; }

        // Images
        public string? MainImage { get; set; }
        public string? ImageGallery { get; set; } // JSON array of image URLs

        // Categories
        public ProductCategory Category { get; set; }
        public bool IsOfficial { get; set; } = true; // Official vs Partner merch
        public int? PartnerId { get; set; }

        // Status
        public ProductStatus Status { get; set; } = ProductStatus.Active;
        public bool IsFeatured { get; set; } = false;

        // Navigation
        public virtual UserModel? Partner { get; set; }
        public virtual ICollection<OrderItemModel> OrderItems { get; set; } = new List<OrderItemModel>();
    }

    public enum ProductCategory
    {
        Apparel,
        Accessories,
        Stationery,
        Digital,
        Other
    }

    public enum ProductStatus
    {
        Draft,
        Active,
        OutOfStock,
        Discontinued
    }
}