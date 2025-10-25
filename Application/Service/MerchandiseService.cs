using Application.DTOs.Common;
using Application.DTOs.Merchandise;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.MerchandiseEntity;
using Domain.Entities.Organize;
using Domain.Interface;
using Domain.Interface.OrganizationRepoFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Service
{
    public class MerchandiseService : IMerchandiseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMerchandiseRepo _merchandiseRepo;
        private readonly IMerchandiseVariantRepo _variantRepo;
        private readonly IRepo<UserModel> _userRepo;
        private readonly IMapper _mapper;

        public MerchandiseService(
            IUnitOfWork unitOfWork,
            IMerchandiseRepo merchandiseRepo,
            IMerchandiseVariantRepo variantRepo,
            IRepo<UserModel> userRepo,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _merchandiseRepo = merchandiseRepo;
            _variantRepo = variantRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }


        // ===== BROWSE/READ OPERATIONS =====

        public async Task<PagedResult<MerchandiseListDto>> GetActiveProductsAsync(int pageNumber, int pageSize)
        {
            var products = await _merchandiseRepo.GetActiveProductsAsync(pageNumber, pageSize);
            var productsList = products.ToList();

            // Get total count for pagination
            var allActive = await _merchandiseRepo.FindAsync(m => !m.IsDeleted && m.IsActive);
            var totalCount = allActive.Count();

            return new PagedResult<MerchandiseListDto>
            {
                Items = _mapper.Map<List<MerchandiseListDto>>(productsList),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<MerchandiseDetailDto?> GetByIdAsync(int id)
        {
            var merchandise = await _merchandiseRepo.GetWithDetailsAsync(id);
            if (merchandise == null || !merchandise.IsActive)
                return null;

            return _mapper.Map<MerchandiseDetailDto>(merchandise);
        }

        public async Task<List<MerchandiseListDto>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<MerchandiseListDto>();

            var results = await _merchandiseRepo.SearchAsync(query);
            return _mapper.Map<List<MerchandiseListDto>>(results);
        }

        public async Task<List<MerchandiseListDto>> GetByCategoryAsync(string category)
        {
            var products = await _merchandiseRepo.GetByCategoryAsync(category);
            return _mapper.Map<List<MerchandiseListDto>>(products);
        }

        public async Task<List<MerchandiseListDto>> GetOfficialProductsAsync()
        {
            var products = await _merchandiseRepo.GetOfficialProductsAsync();
            return _mapper.Map<List<MerchandiseListDto>>(products);
        }

        public async Task<List<MerchandiseDto>> GetBySellerIdAsync(int sellerId)
        {
            var products = await _merchandiseRepo.GetBySellerAsync(sellerId);
            return _mapper.Map<List<MerchandiseDto>>(products);
        }

        // ===== ORGANIZER CRUD =====

        public async Task<MerchandiseDetailDto> CreateAsync(CreateMerchandiseDto dto, int sellerId)
        {
            // Validate user is Organizer
            var user = await _userRepo.GetByIdAsync(sellerId);
            if (user == null || (user.Role != UserRoles.Organizer && user.Role != UserRoles.Admin))
                throw new UnauthorizedAccessException("Only Organizers or Admin can create merchandise");

            // Check if SKU exists
            if (!string.IsNullOrEmpty(dto.SKU))
            {
                var existingSku = await _merchandiseRepo.GetBySkuAsync(dto.SKU);
                if (existingSku != null)
                    throw new InvalidOperationException("SKU already exists");
            }

            var merchandise = new Merchandise
            {
                SellerId = sellerId,
                SKU = string.IsNullOrEmpty(dto.SKU) ? GenerateSKU() : dto.SKU,
                Name = dto.Name,
                Description = dto.Description,
                Category = dto.Category,
                BasePrice = dto.BasePrice,
                StockQuantity = dto.StockQuantity,
                ReservedQuantity = 0,
                IsOfficial = false,
                IsActive = true,
                Images = SerializeImages(dto.Images),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = sellerId.ToString()
            };

            await _merchandiseRepo.AddAsync(merchandise);
            await _unitOfWork.SaveChangesAsync();

            return (await GetByIdAsync(merchandise.Id))!;
        }

        public async Task<MerchandiseDetailDto> UpdateAsync(int id, UpdateMerchandiseDto dto, int sellerId)
        {
            var merchandise = await _merchandiseRepo.GetByIdAsync(id);
            if (merchandise == null || merchandise.IsDeleted)
                throw new KeyNotFoundException("Merchandise not found");

            if (merchandise.SellerId != sellerId)
                throw new UnauthorizedAccessException("You can only update your own merchandise");
            if (!string.IsNullOrEmpty(dto.SKU) && dto.SKU != merchandise.SKU)
            {
                var existingSku = await _merchandiseRepo.GetBySkuAsync(dto.SKU);
                if (existingSku != null && existingSku.Id != id)
                    throw new InvalidOperationException("SKU already exists");

                merchandise.SKU = dto.SKU;
            }
            merchandise.Name = dto.Name;
            merchandise.Description = dto.Description;
            merchandise.Category = dto.Category;
            merchandise.BasePrice = dto.BasePrice;
            merchandise.StockQuantity = dto.StockQuantity;
            if (dto.IsActive.HasValue)
                merchandise.IsActive = dto.IsActive.Value;
            merchandise.Images = SerializeImages(dto.Images);
            merchandise.UpdatedAt = DateTime.UtcNow;
            merchandise.UpdatedBy = sellerId.ToString();

            _merchandiseRepo.Update(merchandise);
            await _unitOfWork.SaveChangesAsync();

            return (await GetByIdAsync(id))!;
        }

        public async Task<bool> DeleteAsync(int id, int sellerId)
        {
            var merchandise = await _merchandiseRepo.GetByIdAsync(id);
            if (merchandise == null || merchandise.IsDeleted)
                return false;

            if (merchandise.SellerId != sellerId)
                throw new UnauthorizedAccessException("You can only delete your own merchandise");

            merchandise.IsDeleted = true;
            merchandise.DeletedAt = DateTime.UtcNow;
            _merchandiseRepo.Update(merchandise);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ToggleActiveStatusAsync(int id, int sellerId)
        {
            var merchandise = await _merchandiseRepo.GetByIdAsync(id);
            if (merchandise == null || merchandise.IsDeleted)
                return false;

            if (merchandise.SellerId != sellerId)
                throw new UnauthorizedAccessException("You can only modify your own merchandise");

            merchandise.IsActive = !merchandise.IsActive;
            merchandise.UpdatedAt = DateTime.UtcNow;
            merchandise.UpdatedBy = sellerId.ToString();

            _merchandiseRepo.Update(merchandise);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // ===== ADMIN/SALES STAFF OPERATIONS =====

        public async Task<MerchandiseDetailDto> AdminCreateAsync(CreateMerchandiseDto dto, int? sellerId = null, bool isOfficial = false)
        {
            if (!string.IsNullOrEmpty(dto.SKU))
            {
                var existingSku = await _merchandiseRepo.GetBySkuAsync(dto.SKU);
                if (existingSku != null)
                    throw new InvalidOperationException("SKU already exists");
            }

            var merchandise = new Merchandise
            {
                SellerId = sellerId,
                SKU = string.IsNullOrEmpty(dto.SKU) ? GenerateSKU() : dto.SKU,
                Name = dto.Name,
                Description = dto.Description,
                Category = dto.Category,
                BasePrice = dto.BasePrice,
                StockQuantity = dto.StockQuantity,
                ReservedQuantity = 0,
                IsOfficial = isOfficial,
                IsActive = true,
                Images = SerializeImages(dto.Images),
                CreatedAt = DateTime.UtcNow
            };

            await _merchandiseRepo.AddAsync(merchandise);
            await _unitOfWork.SaveChangesAsync();

            return (await GetByIdAsync(merchandise.Id))!;
        }

        public async Task<MerchandiseDetailDto> AdminUpdateAsync(int id, UpdateMerchandiseDto dto)
        {
            var merchandise = await _merchandiseRepo.GetByIdAsync(id);
            if (merchandise == null || merchandise.IsDeleted)
                throw new KeyNotFoundException("Merchandise not found");
            if (!string.IsNullOrEmpty(dto.SKU) && dto.SKU != merchandise.SKU)
            {
                var existingSku = await _merchandiseRepo.GetBySkuAsync(dto.SKU);
                if (existingSku != null && existingSku.Id != id)
                    throw new InvalidOperationException("SKU already exists");

                merchandise.SKU = dto.SKU;
            }
            merchandise.Name = dto.Name;
            merchandise.Description = dto.Description;
            merchandise.Category = dto.Category;
            merchandise.BasePrice = dto.BasePrice;
            merchandise.StockQuantity = dto.StockQuantity;
            if (dto.IsActive.HasValue)
                merchandise.IsActive = dto.IsActive.Value;
            merchandise.Images = SerializeImages(dto.Images);
            merchandise.UpdatedAt = DateTime.UtcNow;

            _merchandiseRepo.Update(merchandise);
            await _unitOfWork.SaveChangesAsync();

            return (await GetByIdAsync(id))!;
        }

        public async Task<bool> AdminDeleteAsync(int id)
        {
            var merchandise = await _merchandiseRepo.GetByIdAsync(id);
            if (merchandise == null || merchandise.IsDeleted)
                return false;

            merchandise.IsDeleted = true;
            merchandise.DeletedAt = DateTime.UtcNow;
            _merchandiseRepo.Update(merchandise);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<PagedResult<MerchandiseDto>> GetAllForManagementAsync(int pageNumber, int pageSize, bool? isOfficial = null)
        {
            var query = await _merchandiseRepo.FindAsync(m => !m.IsDeleted);

            if (isOfficial.HasValue)
                query = query.Where(m => m.IsOfficial == isOfficial.Value);

            var totalCount = query.Count();
            var items = query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<MerchandiseDto>
            {
                Items = _mapper.Map<List<MerchandiseDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        // ===== VARIANT OPERATIONS =====

        public async Task<MerchandiseVariantDto> AddVariantAsync(int merchandiseId, CreateVariantDto dto, int userId)
        {
            var merchandise = await _merchandiseRepo.GetByIdAsync(merchandiseId);
            if (merchandise == null || merchandise.IsDeleted)
                throw new KeyNotFoundException("Merchandise not found");

            var variant = new MerchandiseVariant
            {
                MerchandiseId = merchandiseId,
                Name = dto.Name,
                Value = dto.Value,
                PriceModifier = dto.PriceModifier,
                StockQuantity = dto.StockQuantity,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId.ToString()
            };

            await _variantRepo.AddAsync(variant);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MerchandiseVariantDto>(variant, opt =>
                opt.Items["BasePrice"] = merchandise.BasePrice);
        }

        public async Task<MerchandiseVariantDto> UpdateVariantAsync(int variantId, UpdateVariantDto dto, int userId)
        {
            var variant = await _variantRepo.GetWithMerchandiseAsync(variantId);
            if (variant == null)
                throw new KeyNotFoundException("Variant not found");

            variant.PriceModifier = dto.PriceModifier;
            variant.StockQuantity = dto.StockQuantity;
            variant.UpdatedAt = DateTime.UtcNow;
            variant.UpdatedBy = userId.ToString();

            _variantRepo.Update(variant);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MerchandiseVariantDto>(variant, opt =>
                opt.Items["BasePrice"] = variant.Merchandise.BasePrice);
        }

        public async Task<bool> DeleteVariantAsync(int variantId, int userId)
        {
            var variant = await _variantRepo.GetByIdAsync(variantId);
            if (variant == null || variant.IsDeleted)
                return false;

            variant.IsDeleted = true;
            variant.DeletedAt = DateTime.UtcNow;
            _variantRepo.Update(variant);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<List<MerchandiseVariantDto>> GetVariantsByMerchandiseIdAsync(int merchandiseId)
        {
            var merchandise = await _merchandiseRepo.GetByIdAsync(merchandiseId);
            if (merchandise == null)
                return new List<MerchandiseVariantDto>();

            var variants = await _variantRepo.GetByMerchandiseIdAsync(merchandiseId);
            return variants.Select(v =>
                _mapper.Map<MerchandiseVariantDto>(v, opt =>
                    opt.Items["BasePrice"] = merchandise.BasePrice))
                .ToList();
        }

        // ===== STOCK MANAGEMENT =====

        public async Task<bool> UpdateStockAsync(int id, int quantity)
        {
            var merchandise = await _merchandiseRepo.GetByIdAsync(id);
            if (merchandise == null || merchandise.IsDeleted)
                return false;

            merchandise.StockQuantity = quantity;
            merchandise.UpdatedAt = DateTime.UtcNow;

            _merchandiseRepo.Update(merchandise);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ReserveStockAsync(int id, int? variantId, int quantity)
        {
            if (variantId.HasValue)
            {
                var variant = await _variantRepo.GetByIdAsync(variantId.Value);
                if (variant == null || variant.StockQuantity < quantity)
                    return false;

                variant.StockQuantity -= quantity;
                _variantRepo.Update(variant);
            }
            else
            {
                var merchandise = await _merchandiseRepo.GetByIdAsync(id);
                if (merchandise == null || merchandise.StockQuantity < quantity)
                    return false;

                merchandise.ReservedQuantity += quantity;
                merchandise.StockQuantity -= quantity;
                _merchandiseRepo.Update(merchandise);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReleaseStockAsync(int id, int? variantId, int quantity)
        {
            if (variantId.HasValue)
            {
                var variant = await _variantRepo.GetByIdAsync(variantId.Value);
                if (variant == null)
                    return false;

                variant.StockQuantity += quantity;
                _variantRepo.Update(variant);
            }
            else
            {
                var merchandise = await _merchandiseRepo.GetByIdAsync(id);
                if (merchandise == null)
                    return false;

                merchandise.ReservedQuantity -= quantity;
                merchandise.StockQuantity += quantity;
                _merchandiseRepo.Update(merchandise);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // ===== HELPER METHODS =====

        private string GenerateSKU()
        {
            return $"TON-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        }

        private string SerializeImages(List<string> images)
        {
            return images == null || images.Count == 0
                ? "[]"
                : JsonSerializer.Serialize(images);
        }
    }
}