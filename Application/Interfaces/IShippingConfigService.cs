using Application.DTOs.ShippingConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IShippingConfigService
    {
        Task<IEnumerable<ShippingConfigDto>> GetAllConfigsAsync();
        Task<ShippingConfigDto> GetConfigByIdAsync(int id);
        Task<ShippingConfigDto> CreateConfigAsync(CreateShippingConfigDto dto);
        Task<ShippingConfigDto> UpdateConfigAsync(int id, UpdateShippingConfigDto dto);
        Task<bool> DeleteConfigAsync(int id);
        Task<decimal> CalculateShippingFeeAsync(string city, decimal subTotal, int totalQuantity);
        Task<ShippingFeeCalculationDto> GetShippingFeeDetailsAsync(string city, decimal subTotal, int totalQuantity);
    }
}

