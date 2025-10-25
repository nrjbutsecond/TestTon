using Application.DTOs.ShippingConfig;
using Application.Interfaces;
using Domain.Entities.MerchandiseEntity;
using Domain.Interface;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class EnhancedShippingService : IEnhancedShippingService
    {
        private readonly IShippingProviderFactoryService _providerFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EnhancedShippingService> _logger;

        public EnhancedShippingService(
            IShippingProviderFactoryService providerFactory,
            IUnitOfWork unitOfWork,
            ILogger<EnhancedShippingService> logger)
        {
            _providerFactory = providerFactory;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ShippingQuoteDto> CalculateShippingAsync(CalculateShippingRequest request)
        {
            // Get shipping config for business rules
            var config = await GetShippingConfigAsync(request.ToAddress);

            // Determine provider
            var providerName = config?.PreferredProvider ?? request.PreferredProvider ?? "local";
            var provider = _providerFactory.GetProvider(providerName);
            if (provider == null)
            {
                throw new InvalidOperationException($"Shipping provider '{providerName}' is not registered");
            }
            // Get base quote from provider
            var providerRequest = MapToProviderRequest(request);
            var quote = await provider.CalculateShippingAsync(providerRequest);

            // Apply business rules
            var finalQuote = await ApplyBusinessRules(quote, config, request.SubTotal);

            return MapToQuoteDto(finalQuote);
        }

        public async Task<List<ShippingQuoteDto>> GetMultipleQuotesAsync(CalculateShippingRequest request)
        {
            var quotes = new List<ShippingQuoteDto>();
            var availableProviders = _providerFactory.GetAvailableProviders();

            foreach (var providerName in availableProviders)
            {
                try
                {
                    var provider = _providerFactory.GetProvider(providerName);
                    var providerRequest = MapToProviderRequest(request);
                    var quote = await provider.CalculateShippingAsync(providerRequest);

                    var config = await GetShippingConfigAsync(request.ToAddress);
                    var finalQuote = await ApplyBusinessRules(quote, config, request.SubTotal);

                    quotes.Add(MapToQuoteDto(finalQuote));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Failed to get quote from {providerName}");
                }
            }

            return quotes.OrderBy(q => q.TotalFee).ToList();
        }

        private async Task<ShippingQuote> ApplyBusinessRules(
            ShippingQuote quote,
            ShippingConfig? config,
            decimal orderSubTotal)
        {
            if (config == null) return quote;

            var adjustedQuote = new ShippingQuote
            {
                ProviderName = quote.ProviderName,
                ServiceType = quote.ServiceType,
                ShippingFee = quote.ShippingFee,
                CodFee = quote.CodFee,
                InsuranceFee = quote.InsuranceFee,
                TotalFee = quote.TotalFee,
                EstimatedDays = quote.EstimatedDays,
                EstimatedDeliveryDate = quote.EstimatedDeliveryDate,
                Metadata = quote.Metadata
            };

            // Apply free shipping threshold
            if (orderSubTotal >= config.FreeShippingThreshold)
            {
                adjustedQuote.ShippingFee = 0;
                adjustedQuote.TotalFee = adjustedQuote.CodFee + adjustedQuote.InsuranceFee;
                adjustedQuote.Metadata["freeShipping"] = true;
                return adjustedQuote;
            }

            // Apply markup
            if (config.MarkupPercentage > 0)
            {
                adjustedQuote.ShippingFee *= (1 + config.MarkupPercentage / 100);
            }

            adjustedQuote.ShippingFee += config.MarkupFixedAmount;

            // Apply min/max limits
            adjustedQuote.ShippingFee = Math.Max(config.MinShippingFee,
                                                 Math.Min(config.MaxShippingFee, adjustedQuote.ShippingFee));

            // Apply promotional discount
            if (config.DiscountPercentage > 0 && IsPromotionActive(config))
            {
                adjustedQuote.ShippingFee *= (1 - config.DiscountPercentage / 100);
                adjustedQuote.Metadata["promotionApplied"] = true;
            }

            // Recalculate total
            adjustedQuote.TotalFee = adjustedQuote.ShippingFee +
                                    adjustedQuote.CodFee +
                                    adjustedQuote.InsuranceFee;

            return adjustedQuote;
        }

        private async Task<ShippingConfig?> GetShippingConfigAsync(ShippingAddressDto address)
        {
            // Find most specific config (ward > district > city > default)
            var configs = await _unitOfWork.ShippingConfigs
                .FindAsync(c => c.IsActive &&
                               (c.City == address.City || c.IsDefault));

            return configs
                .OrderByDescending(c => !c.IsDefault)
                .ThenByDescending(c => c.Ward == address.Ward)
                .ThenByDescending(c => c.District == address.District)
                .ThenByDescending(c => c.Priority)
                .FirstOrDefault();
        }

        private bool IsPromotionActive(ShippingConfig config)
        {
            var now = DateTime.UtcNow;
            return config.PromotionStartDate <= now &&
                   (!config.PromotionEndDate.HasValue || config.PromotionEndDate > now);
        }

        private ShippingCalculationRequest MapToProviderRequest(CalculateShippingRequest request)
        {
            return new ShippingCalculationRequest
            {
                FromAddress = new ShippingAddress
                {
                    FullName = request.FromAddress?.FullName ?? "Shop",
                    Phone = request.FromAddress?.Phone ?? "",
                    Address = request.FromAddress?.Address ?? "",
                    City = request.FromAddress?.City ?? "Ho Chi Minh",
                    District = request.FromAddress?.District ?? "",
                    Ward = request.FromAddress?.Ward ?? ""
                },
                ToAddress = new ShippingAddress
                {
                    FullName = request.ToAddress.FullName,
                    Phone = request.ToAddress.Phone,
                    Address = request.ToAddress.Address,
                    City = request.ToAddress.City,
                    District = request.ToAddress.District,
                    Ward = request.ToAddress.Ward
                },
                Package = new PackageInfo
                {
                    Weight = request.TotalWeight,
                    Length = request.PackageDimensions?.Length ?? 30,
                    Width = request.PackageDimensions?.Width ?? 20,
                    Height = request.PackageDimensions?.Height ?? 15,
                    Description = "Merchandise",
                    Quantity = request.TotalItems
                },
                ServiceType = request.ServiceType ?? "Standard",
                CodAmount = request.CodAmount,
                InsuranceValue = request.InsuranceValue
            };
        }

        private ShippingQuoteDto MapToQuoteDto(ShippingQuote quote)
        {
            return new ShippingQuoteDto
            {
                ProviderName = quote.ProviderName,
                ServiceType = quote.ServiceType,
                ShippingFee = quote.ShippingFee,
                CodFee = quote.CodFee,
                InsuranceFee = quote.InsuranceFee,
                TotalFee = quote.TotalFee,
                EstimatedDays = quote.EstimatedDays,
                EstimatedDeliveryDate = quote.EstimatedDeliveryDate,
                Metadata = quote.Metadata
            };
        }
     
        public async Task<CreateShippingOrderResultDto> CreateShippingOrderAsync(CreateShippingOrderDto request)
        {
            var provider = _providerFactory.GetProvider(request.Provider);

            var providerRequest = new CreateShippingOrderRequest
            {
                OrderId = request.OrderNumber,
                FromAddress = MapAddress(request.FromAddress),
                ToAddress = MapAddress(request.ToAddress),
                Package = new PackageInfo
                {
                    Weight = request.Package.Weight,
                    Length = request.Package.Length,
                    Width = request.Package.Width,
                    Height = request.Package.Height,
                    Description = request.Package.Description
                },
                ServiceType = request.ServiceType,
                CodAmount = request.CodAmount,
                InsuranceValue = request.InsuranceValue,
                Note = request.Note,
                Items = request.Items.Select(i => new ShippingItem
                {
                    Name = i.Name,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    Code = i.Code
                }).ToList()
            };

            var result = await provider.CreateShippingOrderAsync(providerRequest);

            return new CreateShippingOrderResultDto
            {
                Success = result.Success,
                ShippingOrderId = result.ShippingOrderId,
                TrackingNumber = result.TrackingNumber,
                SortingCode = result.SortingCode,
                TotalFee = result.TotalFee,
                ErrorMessage = result.ErrorMessage,
                Provider = provider.ProviderName
            };
        }

        public async Task<TrackingInfoDto> TrackShipmentAsync(string trackingNumber, string provider)
        {
            var shippingProvider = _providerFactory.GetProvider(provider);
            var tracking = await shippingProvider.TrackShipmentAsync(trackingNumber);

            return new TrackingInfoDto
            {
                TrackingNumber = tracking.TrackingNumber,
                Status = tracking.Status,
                StatusDescription = tracking.StatusDescription,
                LastUpdated = tracking.LastUpdated,
                Events = tracking.Events.Select(e => new TrackingEventDto
                {
                    Timestamp = e.Timestamp,
                    Status = e.Status,
                    Description = e.Description,
                    Location = e.Location
                }).ToList()
            };
        }

        public async Task<List<ShippingMethodDto>> GetAvailableMethodsAsync(string fromCity, string toCity)
        {
            var methods = new List<ShippingMethodDto>();
            var providers = _providerFactory.GetAvailableProviders();

            foreach (var providerName in providers)
            {
                try
                {
                    var provider = _providerFactory.GetProvider(providerName);
                    var from = new ShippingAddress { City = fromCity };
                    var to = new ShippingAddress { City = toCity };

                    var providerMethods = await provider.GetAvailableMethodsAsync(from, to);

                    methods.AddRange(providerMethods.Select(m => new ShippingMethodDto
                    {
                        Provider = providerName,
                        MethodId = m.MethodId,
                        Name = m.Name,
                        Description = m.Description,
                        EstimatedDays = m.EstimatedDays,
                        SupportsCod = m.SupportsCod
                    }));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Failed to get methods from {providerName}");
                }
            }

            return methods;
        }

        public async Task<bool> CancelShippingOrderAsync(string shippingOrderId, string provider)
        {
            var shippingProvider = _providerFactory.GetProvider(provider);
            return await shippingProvider.CancelShippingOrderAsync(shippingOrderId);
        }

        private ShippingAddress MapAddress(ShippingAddressDto dto)
        {
            return new ShippingAddress
            {
                FullName = dto.FullName,
                Phone = dto.Phone,
                Address = dto.Address,
                City = dto.City,
                District = dto.District,
                Ward = dto.Ward,
                PostalCode = dto.PostalCode
            };
        }
    }
}