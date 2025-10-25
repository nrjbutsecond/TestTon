using Application.DTOs.ShippingConfig;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class GhnShippingProviderService: IShippingProviderService
    {
        private readonly HttpClient _httpClient;
        private readonly GhnSettings _settings;
        private readonly ILogger<GhnShippingProviderService> _logger;

        public string ProviderName => "GHN";
        public bool IsAvailable => !string.IsNullOrEmpty(_settings.ApiKey);

        public GhnShippingProviderService(HttpClient httpClient, IOptions<GhnSettings> settings, ILogger<GhnShippingProviderService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;

            if (!string.IsNullOrEmpty(_settings.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("Token", _settings.ApiKey);
                _httpClient.DefaultRequestHeaders.Add("ShopId", _settings.ShopId);
            }
        }

        public async Task<ShippingQuote> CalculateShippingAsync(ShippingCalculationRequest request)
        {
            try
            {
                // Map addresses to GHN location IDs
                await PopulateLocationIds(request.FromAddress);
                await PopulateLocationIds(request.ToAddress);

                var payload = new
                {
                    service_type_id = MapServiceType(request.ServiceType),
                    from_district_id = request.FromAddress.DistrictId,
                    to_district_id = request.ToAddress.DistrictId,
                    to_ward_code = request.ToAddress.WardCode,
                    weight = (int)(request.Package.Weight * 1000), // grams
                    length = (int)request.Package.Length,
                    width = (int)request.Package.Width,
                    height = (int)request.Package.Height,
                    insurance_value = (int)request.InsuranceValue,
                    cod_value = (int)request.CodAmount
                };

                // Gọi GHN dev API
                var response = await _httpClient.PostAsJsonAsync("/v2/shipping-order/fee", payload);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("GHN dev API returned non-success status: {Status}", response.StatusCode);
                    return BuildPlaceholderQuote(request);
                }

                var result = await response.Content.ReadFromJsonAsync<GhnFeeResponse>();
                if (result == null)
                {
                    _logger.LogWarning("GHN dev API returned null result");
                    return BuildPlaceholderQuote(request);
                }

                // Map GHN response sang ShippingQuote
                return new ShippingQuote
                {
                    ProviderName = ProviderName,
                    ServiceType = request.ServiceType,
                    ShippingFee = result.fee ?? 0,
                    CodFee = result.cod_fee ?? 0,
                    InsuranceFee = result.insurance_fee ?? 0,
                    TotalFee = (result.fee ?? 0) + (result.cod_fee ?? 0) + (result.insurance_fee ?? 0),
                    EstimatedDays = result.estimated_days ?? 3,
                    EstimatedDeliveryDate = DateTime.UtcNow.AddDays(result.estimated_days ?? 3),
                    Metadata = result.metadata ?? new Dictionary<string, object>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating GHN shipping fee");
                return BuildPlaceholderQuote(request);
            }
        }

        // Helper để fallback nếu GHN dev API lỗi (chưa lấy được api của ghn
        //TODO: lấy api của ghn
        private ShippingQuote BuildPlaceholderQuote(ShippingCalculationRequest request)
        {
            return new ShippingQuote
            {
                ProviderName = ProviderName,
                ServiceType = request.ServiceType,
                ShippingFee = 35000,
                CodFee = request.CodAmount > 0 ? 5000 : 0,
                InsuranceFee = request.InsuranceValue * 0.003m,
                TotalFee = 35000 + (request.CodAmount > 0 ? 5000 : 0) + (request.InsuranceValue * 0.003m),
                EstimatedDays = 3,
                EstimatedDeliveryDate = DateTime.UtcNow.AddDays(3)
            };
        }

        public async Task<CreateShippingOrderResult> CreateShippingOrderAsync(CreateShippingOrderRequest request)
        {
            try
            {
                await PopulateLocationIds(request.FromAddress);
                await PopulateLocationIds(request.ToAddress);

                var items = request.Items.Select(i => new
                {
                    name = i.Name,
                    quantity = i.Quantity,
                    price = (int)i.Price,
                    code = i.Code ?? ""
                }).ToList();

                var payload = new
                {
                    from_name = _settings.ShopName,
                    from_phone = _settings.ShopPhone,
                    from_address = _settings.ShopAddress,
                    from_district_id = request.FromAddress.DistrictId,
                    to_name = request.ToAddress.FullName,
                    to_phone = request.ToAddress.Phone,
                    to_address = request.ToAddress.Address,
                    to_district_id = request.ToAddress.DistrictId,
                    to_ward_code = request.ToAddress.WardCode,
                    service_type_id = MapServiceType(request.ServiceType),
                    payment_type_id = request.CodAmount > 0 ? 2 : 1, // 1: Shop pays, 2: Buyer pays
                    cod_amount = (int)request.CodAmount,
                    insurance_value = (int)request.InsuranceValue,
                    weight = (int)(request.Package.Weight * 1000),
                    length = (int)request.Package.Length,
                    width = (int)request.Package.Width,
                    height = (int)request.Package.Height,
                    note = request.Note ?? "",
                    items = items
                };

                // TODO: Uncomment when API is available
                // var response = await _httpClient.PostAsJsonAsync("/v2/shipping-order/create", payload);
                // var result = await response.Content.ReadFromJsonAsync<GhnCreateOrderResponse>();

                // Placeholder
                return new CreateShippingOrderResult
                {
                    Success = true,
                    ShippingOrderId = $"GHN{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}",
                    TrackingNumber = $"GHN{DateTime.UtcNow:yyyyMMdd}{new Random().Next(10000, 99999)}",
                    TotalFee = 35000,
                    Metadata = { { "provider", "GHN" } }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating GHN shipping order");
                return new CreateShippingOrderResult
                {
                    Success = false,
                    ErrorMessage = "Unable to create shipping order with GHN"
                };
            }
        }

        private async Task PopulateLocationIds(ShippingAddress address)
        {
            // TODO: Implement actual API calls to get location IDs
            // For now, use placeholder values
            if (address.City.ToLower().Contains("ho chi minh"))
            {
                address.ProvinceId = 202;
                address.DistrictId = address.District.ToLower() switch
                {
                    var d when d.Contains("1") => 1442,
                    var d when d.Contains("2") => 1443,
                    var d when d.Contains("3") => 1444,
                    _ => 1442
                };
                address.WardCode = "20101"; // Placeholder
            }
            else if (address.City.ToLower().Contains("ha noi"))
            {
                address.ProvinceId = 201;
                address.DistrictId = 1;
                address.WardCode = "1A0101";
            }
        }

        private int MapServiceType(string serviceType)
        {
            return serviceType.ToLower() switch
            {
                "express" => 1, // GHN Express
                "standard" => 2, // GHN Standard
                _ => 2
            };
        }

        public async Task<List<ShippingMethod>> GetAvailableMethodsAsync(ShippingAddress from, ShippingAddress to)
        {
            // TODO: Call GHN API to get available services
            return new List<ShippingMethod>
            {
                new ShippingMethod
                {
                    MethodId = "2",
                    Name = "Giao hàng tiết kiệm",
                    Description = "Giao trong 3-4 ngày",
                    EstimatedDays = 3,
                    SupportsCod = true
                },
                new ShippingMethod
                {
                    MethodId = "1",
                    Name = "Giao hàng nhanh",
                    Description = "Giao trong 1-2 ngày",
                    EstimatedDays = 2,
                    SupportsCod = true
                }
            };
        }

        public async Task<TrackingInfo> TrackShipmentAsync(string trackingNumber)
        {
            // TODO: Implement actual tracking
            return new TrackingInfo
            {
                TrackingNumber = trackingNumber,
                Status = "delivering",
                StatusDescription = "Đang giao hàng",
                LastUpdated = DateTime.UtcNow
            };
        }

        public async Task<bool> CancelShippingOrderAsync(string shippingOrderId)
        {
            // TODO: Implement cancellation
            return true;
        }
    }

    public class GhnSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ShopId { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://online-gateway.ghn.vn/shiip/public-api"; //idk
        public string ShopName { get; set; } = string.Empty;
        public string ShopPhone { get; set; } = string.Empty;
        public string ShopAddress { get; set; } = string.Empty;
    }

    public class GhnFeeResponse
    {
        /// <summary>
        /// Phí vận chuyển do GHN tính toán (VND)
        /// </summary>
        public decimal? fee { get; set; }

        /// <summary>
        /// Phí COD nếu có (VND)
        /// </summary>
        public decimal? cod_fee { get; set; }

        /// <summary>
        /// Phí bảo hiểm nếu có (VND)
        /// </summary>
        public decimal? insurance_fee { get; set; }

        /// <summary>
        /// Số ngày dự kiến giao hàng
        /// </summary>
        public int? estimated_days { get; set; }

        /// <summary>
        /// Metadata hoặc các thông tin bổ sung khác
        /// </summary>
        public Dictionary<string, object>? metadata { get; set; }
    }
}