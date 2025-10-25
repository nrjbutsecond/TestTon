using Application.DTOs.ShippingConfig;
using Application.Interfaces;
using Domain.Entities.MerchandiseEntity;
using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class LocalShippingProviderService: IShippingProviderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public string ProviderName => "Local";
        public bool IsAvailable => true;

        public LocalShippingProviderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ShippingQuote> CalculateShippingAsync(ShippingCalculationRequest request)
        {
            // Try to find specific config for location
            var config = await FindBestMatchingConfig(request.ToAddress);

            if (config == null)
                throw new InvalidOperationException("No shipping configuration found");

            decimal baseFee = config.BaseFee;

            // Weight-based calculation
            if (request.Package.Weight > 5)
            {
                baseFee += (request.Package.Weight - 5) * 10000; // 10k per extra kg
            }

            // Dimensional weight (if package is too large)
            var dimensionalWeight = (request.Package.Length * request.Package.Width * request.Package.Height) / 6000;
            if (dimensionalWeight > request.Package.Weight)
            {
                var extraDimWeight = dimensionalWeight - request.Package.Weight;
                baseFee += extraDimWeight * 8000; // 8k per dimensional kg
            }

            return new ShippingQuote
            {
                ProviderName = ProviderName,
                ServiceType = request.ServiceType,
                ShippingFee = baseFee,
                CodFee = request.CodAmount > 0 ? request.CodAmount * 0.01m : 0, // 1% COD fee
                InsuranceFee = request.InsuranceValue * 0.005m, // 0.5% insurance
                TotalFee = baseFee + (request.CodAmount > 0 ? request.CodAmount * 0.01m : 0) + (request.InsuranceValue * 0.005m),
                EstimatedDays = GetEstimatedDays(request.ToAddress.City),
                EstimatedDeliveryDate = DateTime.UtcNow.AddDays(GetEstimatedDays(request.ToAddress.City))
            };
        }

        private async Task<ShippingConfig?> FindBestMatchingConfig(ShippingAddress address)
        {
            // Try ward level first
            if (!string.IsNullOrEmpty(address.Ward))
            {
                var wardConfig = await _unitOfWork.ShippingConfigs
                    .FindAsync(c => c.City == address.City &&
                                   c.District == address.District &&
                                   c.Ward == address.Ward &&
                                   c.IsActive);
                if (wardConfig.Any())
                    return wardConfig.OrderByDescending(c => c.Priority).First();
            }

            // Then district level
            if (!string.IsNullOrEmpty(address.District))
            {
                var districtConfig = await _unitOfWork.ShippingConfigs
                    .FindAsync(c => c.City == address.City &&
                                   c.District == address.District &&
                                   c.Ward == null &&
                                   c.IsActive);
                if (districtConfig.Any())
                    return districtConfig.OrderByDescending(c => c.Priority).First();
            }

            // Then city level
            var cityConfig = await _unitOfWork.ShippingConfigs
                .FindAsync(c => c.City == address.City &&
                              c.District == null &&
                              c.Ward == null &&
                              c.IsActive);
            if (cityConfig.Any())
                return cityConfig.OrderByDescending(c => c.Priority).First();

            // Finally default
            var defaultConfig = await _unitOfWork.ShippingConfigs
                .FindAsync(c => c.IsDefault && c.IsActive);
            return defaultConfig.OrderByDescending(c => c.Priority).FirstOrDefault();
        }

        public async Task<List<ShippingMethod>> GetAvailableMethodsAsync(ShippingAddress from, ShippingAddress to)
        {
            return new List<ShippingMethod>
            {
                new ShippingMethod
                {
                    MethodId = "standard",
                    Name = "Giao hàng tiêu chuẩn",
                    Description = "Giao trong 2-3 ngày",
                    EstimatedDays = GetEstimatedDays(to.City),
                    SupportsCod = true
                },
                new ShippingMethod
                {
                    MethodId = "express",
                    Name = "Giao hàng nhanh",
                    Description = "Giao trong 1-2 ngày",
                    EstimatedDays = Math.Max(1, GetEstimatedDays(to.City) - 1),
                    SupportsCod = true
                }
            };
        }

        public async Task<CreateShippingOrderResult> CreateShippingOrderAsync(CreateShippingOrderRequest request)
        {
            // For local provider, just generate a tracking number
            var trackingNumber = $"LOCAL{DateTime.UtcNow:yyyyMMdd}{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
            var quote = await CalculateShippingAsync(request);

            return new CreateShippingOrderResult
            {
                Success = true,
                ShippingOrderId = Guid.NewGuid().ToString(),
                TrackingNumber = trackingNumber,
                TotalFee = quote.TotalFee,
                Metadata = new Dictionary<string, object>
                {
                    { "provider", ProviderName },
                    { "createdAt", DateTime.UtcNow }
                }
            };
        }

        public Task<TrackingInfo> TrackShipmentAsync(string trackingNumber)
        {
            // Mock tracking for local provider
            return Task.FromResult(new TrackingInfo
            {
                TrackingNumber = trackingNumber,
                Status = "in_transit",
                StatusDescription = "Đang vận chuyển",
                LastUpdated = DateTime.UtcNow,
                Events = new List<TrackingEvent>
                {
                    new TrackingEvent
                    {
                        Timestamp = DateTime.UtcNow.AddDays(-1),
                        Status = "picked_up",
                        Description = "Đã lấy hàng",
                        Location = "Kho hàng"
                    }
                }
            });
        }

        public Task<bool> CancelShippingOrderAsync(string shippingOrderId)
        {
            // Local provider always allows cancellation
            return Task.FromResult(true);
        }

        private int GetEstimatedDays(string city)
        {
            return city.ToLower() switch
            {
                "ho chi minh" => 2,
                "ha noi" => 3,
                _ => 4
            };
        }
    }
}