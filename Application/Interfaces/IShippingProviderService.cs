using Application.DTOs.ShippingConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IShippingProviderService
    {
        string ProviderName { get; }
        bool IsAvailable { get; }

        Task<ShippingQuote> CalculateShippingAsync(ShippingCalculationRequest request);
        Task<List<ShippingMethod>> GetAvailableMethodsAsync(ShippingAddress from, ShippingAddress to);
        Task<CreateShippingOrderResult> CreateShippingOrderAsync(CreateShippingOrderRequest request);
        Task<TrackingInfo> TrackShipmentAsync(string trackingNumber);
        Task<bool> CancelShippingOrderAsync(string shippingOrderId);
    }
}
