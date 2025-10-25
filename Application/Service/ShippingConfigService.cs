using Application.DTOs.ShippingConfig;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities.MerchandiseEntity;
using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class ShippingConfigService : IShippingConfigService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShippingConfigService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ShippingConfigDto>> GetAllConfigsAsync()
        {
            var configs = await _unitOfWork.ShippingConfig.GetActiveConfigsAsync();
            return _mapper.Map<IEnumerable<ShippingConfigDto>>(configs);
        }

        public async Task<ShippingConfigDto> GetConfigByIdAsync(int id)
        {
            var config = await _unitOfWork.ShippingConfig.GetByIdAsync(id);
            if (config == null)
                throw new InvalidOperationException("Shipping config not found");

            return _mapper.Map<ShippingConfigDto>(config);
        }

        public async Task<ShippingConfigDto> CreateConfigAsync(CreateShippingConfigDto dto)
        {
            // Validate
            if (dto.IsDefault)
            {
                var existingDefault = await _unitOfWork.ShippingConfig.GetDefaultConfigAsync();
                if (existingDefault != null)
                    throw new InvalidOperationException("Default config already exists. Please update the existing one.");
            }
            else
            {
                var exists = await _unitOfWork.ShippingConfig.IsCityConfigExistsAsync(dto.City);
                if (exists)
                    throw new InvalidOperationException($"Config for {dto.City} already exists");
            }

            var config = _mapper.Map<ShippingConfig>(dto);
            config.IsActive = true;
            config.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.ShippingConfig.AddAsync(config);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ShippingConfigDto>(config);
        }

        public async Task<ShippingConfigDto> UpdateConfigAsync(int id, UpdateShippingConfigDto dto)
        {
            var config = await _unitOfWork.ShippingConfig.GetByIdAsync(id);
            if (config == null)
                throw new InvalidOperationException("Shipping config not found");

            _mapper.Map(dto, config);
            config.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ShippingConfigDto>(config);
        }

        public async Task<bool> DeleteConfigAsync(int id)
        {
            var config = await _unitOfWork.ShippingConfig.GetByIdAsync(id);
            if (config == null)
                throw new InvalidOperationException("Shipping config not found");

            if (config.IsDefault)
                throw new InvalidOperationException("Cannot delete default config");

            config.IsDeleted = true;
            config.DeletedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> CalculateShippingFeeAsync(string city, decimal subTotal, int totalQuantity)
        {
            var calculation = await GetShippingFeeDetailsAsync(city, subTotal, totalQuantity);
            return calculation.CalculatedFee;
        }

        public async Task<ShippingFeeCalculationDto> GetShippingFeeDetailsAsync(string city, decimal subTotal, int totalQuantity)
        {
            // Get config for city
            var config = await _unitOfWork.ShippingConfig.GetConfigByCityAsync(city);

            // If not found, get default config
            config ??= await _unitOfWork.ShippingConfig.GetDefaultConfigAsync();

            if (config == null)
                throw new InvalidOperationException("No shipping configuration found");

            var result = new ShippingFeeCalculationDto
            {
                City = city,
                SubTotal = subTotal,
                TotalQuantity = totalQuantity
            };

            // Free shipping check
            if (subTotal >= config.FreeShippingThreshold)
            {
                result.CalculatedFee = 0;
                result.AppliedRule = $"Miễn phí vận chuyển cho đơn hàng từ {config.FreeShippingThreshold:N0}đ";
                return result;
            }

            // Calculate base fee
            decimal fee = config.BaseFee;
            result.AppliedRule = $"phí cơ bản: {config.BaseFee:N0}đ";

            // Additional fee for bulk orders
            if (totalQuantity > config.BulkOrderThreshold)
            {
                fee += config.BulkOrderExtraFee;
                result.AppliedRule += $" + Phí số lượng lớn: {config.BulkOrderExtraFee:N0}đ";
            }

            result.CalculatedFee = fee;
            return result;
        }
    }
}