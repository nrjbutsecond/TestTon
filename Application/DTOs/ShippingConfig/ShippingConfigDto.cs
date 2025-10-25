using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ShippingConfig
{
    public class ShippingConfigDto
    {
        public int Id { get; set; }
        public string City { get; set; } = string.Empty;
        public string? District { get; set; }  
        public string? Ward { get; set; }
        public decimal BaseFee { get; set; }
        public decimal FreeShippingThreshold { get; set; }
        public decimal BulkOrderThreshold { get; set; }
        public decimal BulkOrderExtraFee { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateShippingConfigDto
    {
        public string City { get; set; } = string.Empty;
        public string? District { get; set; } 
        public string? Ward { get; set; }
        public decimal BaseFee { get; set; }
        public decimal FreeShippingThreshold { get; set; } = 500000m;
        public decimal BulkOrderThreshold { get; set; } = 10;
        public decimal BulkOrderExtraFee { get; set; } = 10000m;
        public bool IsDefault { get; set; } = false;
    }

    public class UpdateShippingConfigDto
    {
        public decimal BaseFee { get; set; }
        public decimal FreeShippingThreshold { get; set; }
        public decimal BulkOrderThreshold { get; set; }
        public decimal BulkOrderExtraFee { get; set; }
        public bool IsActive { get; set; }
    }

    public class ShippingFeeCalculationDto
    {
        public string City { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public int TotalQuantity { get; set; }
        public decimal CalculatedFee { get; set; }
        public string? AppliedRule { get; set; }
    }

    //=========================================//

    //===========================SERVICE/DOMAIN LAYER=====================//
    public class ShippingCalculationRequest
    {
        public ShippingAddress FromAddress { get; set; }
        public ShippingAddress ToAddress { get; set; }
        public PackageInfo Package { get; set; }
        public string ServiceType { get; set; } = "Standard";
        public decimal CodAmount { get; set; } = 0; // Cash on delivery
        public decimal InsuranceValue { get; set; } = 0;
    }

    public class ShippingAddress
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string? PostalCode { get; set; }

        // Provider-specific IDs (populated by provider)
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public string? WardCode { get; set; }
    }

    public class PackageInfo
    {
        public decimal Weight { get; set; } // kg
        public decimal Length { get; set; } // cm
        public decimal Width { get; set; } // cm
        public decimal Height { get; set; } // cm
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
    }

    public class ShippingQuote //domain model use in local
    {
        public string ProviderName { get; set; }
        public string ServiceType { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal CodFee { get; set; } = 0;
        public decimal InsuranceFee { get; set; } = 0;
        public decimal TotalFee { get; set; }
        public int EstimatedDays { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class ShippingMethod
    {
        public string MethodId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int EstimatedDays { get; set; }
        public bool SupportsCod { get; set; }
    }

    public class CreateShippingOrderRequest : ShippingCalculationRequest
    {
        public string OrderId { get; set; }
        public string? Note { get; set; }
        public List<ShippingItem> Items { get; set; } = new();
    }

    public class ShippingItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? Code { get; set; }
    }

    public class CreateShippingOrderResult
    {
        public bool Success { get; set; }
        public string? ShippingOrderId { get; set; }
        public string? TrackingNumber { get; set; }
        public string? SortingCode { get; set; }
        public decimal TotalFee { get; set; }
        public string? ErrorMessage { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class TrackingInfo
    {
        public string TrackingNumber { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<TrackingEvent> Events { get; set; } = new();
    }

    public class TrackingEvent
    {
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string? Location { get; set; }
    }

    //===============================================================================================//

    //===============API layer===================================//
    public class CalculateShippingRequest
    {
        public ShippingAddressDto? FromAddress { get; set; }
        public ShippingAddressDto ToAddress { get; set; }
        public decimal SubTotal { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalWeight { get; set; }
        public PackageDimensionsDto? PackageDimensions { get; set; }
        public string? ServiceType { get; set; }
        public string? PreferredProvider { get; set; }
        public decimal CodAmount { get; set; }
        public decimal InsuranceValue { get; set; }
    }

    public class ShippingAddressDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public string? Note { get; set; }
    }

    public class PackageDimensionsDto
    {
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
    }

    public class ShippingQuoteDto
    {
        public string ProviderName { get; set; }
        public string ServiceType { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal CodFee { get; set; }
        public decimal InsuranceFee { get; set; }
        public decimal TotalFee { get; set; }
        public int EstimatedDays { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class CreateShippingOrderDto
    {
        public string OrderNumber { get; set; }
        public string Provider { get; set; }
        public ShippingAddressDto FromAddress { get; set; }
        public ShippingAddressDto ToAddress { get; set; }
        public PackageDto Package { get; set; }
        public string ServiceType { get; set; }
        public decimal CodAmount { get; set; }
        public decimal InsuranceValue { get; set; }
        public string? Note { get; set; }
        public List<ShippingItemDto> Items { get; set; } = new();
    }

    public class PackageDto
    {
        public decimal Weight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string Description { get; set; }
    }

    public class ShippingItemDto
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? Code { get; set; }
    }

    public class CreateShippingOrderResultDto
    {
        public bool Success { get; set; }
        public string? ShippingOrderId { get; set; }
        public string? TrackingNumber { get; set; }
        public string? SortingCode { get; set; }
        public decimal TotalFee { get; set; }
        public string? ErrorMessage { get; set; }
        public string Provider { get; set; }
    }

    public class ShippingMethodDto
    {
        public string Provider { get; set; }
        public string MethodId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int EstimatedDays { get; set; }
        public bool SupportsCod { get; set; }
    }

    public class TrackingInfoDto
    {
        public string TrackingNumber { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<TrackingEventDto> Events { get; set; } = new();
    }

    public class TrackingEventDto
    {
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string? Location { get; set; }
    }


    //=============================================================//
}