using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.ShippingConfig;
namespace Application.Interfaces
{
    public interface IEnhancedShippingService
    {
        Task<ShippingQuoteDto> CalculateShippingAsync(CalculateShippingRequest request);
        Task<List<ShippingQuoteDto>> GetMultipleQuotesAsync(CalculateShippingRequest request);
        Task<CreateShippingOrderResultDto> CreateShippingOrderAsync(CreateShippingOrderDto request);
        Task<TrackingInfoDto> TrackShipmentAsync(string trackingNumber, string provider);
        Task<List<ShippingMethodDto>> GetAvailableMethodsAsync(string fromCity, string toCity);
        Task<bool> CancelShippingOrderAsync(string shippingOrderId, string provider);
    }

}
