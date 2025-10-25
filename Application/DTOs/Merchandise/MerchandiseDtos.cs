using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Merchandise
{
    public class MerchandiseDto
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public int StockQuantity { get; set; }
        public int AvailableStock { get; set; }
        public bool IsOfficial { get; set; }
        public List<string> Images { get; set; } = new(); // Base64 data URLs 
        public string? SellerName { get; set; }
    }

    public class MerchandiseDetailDto : MerchandiseDto
    {
        public int? SellerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<MerchandiseVariantDto> Variants { get; set; } = new();
    }

    public class CreateMerchandiseDto
    {
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public int StockQuantity { get; set; }
        public List<string> Images { get; set; } = new();
    }
    public class MerchandiseListDto
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public int AvailableStock { get; set; }
        public bool IsInStock { get; set; }
        public bool IsOfficial { get; set; }
        public List<string> Images { get; set; } = new(); //get first image
    }
    public class UpdateMerchandiseDto
    {
        public string? SKU { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public int StockQuantity { get; set; }
        public bool? IsActive { get; set; }
        public List<string> Images { get; set; } = new();
    }

    public class UpdateVariantDto
    {
        public decimal PriceModifier { get; set; }
        public int StockQuantity { get; set; }
    }

    public class MerchandiseVariantDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public decimal PriceModifier { get; set; }
        public int StockQuantity { get; set; }
        public decimal FinalPrice { get; set; }
        public bool IsInStock { get; set; }
    }
    public class CreateVariantDto
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public decimal PriceModifier { get; set; }
        public int StockQuantity { get; set; }
    }
}