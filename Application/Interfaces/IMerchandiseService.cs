using Application.DTOs.Common;
using Application.DTOs.Merchandise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMerchandiseService
    {
        // Browse/Read operations (Guest + Member)
        Task<PagedResult<MerchandiseListDto>> GetActiveProductsAsync(int pageNumber, int pageSize);
        Task<MerchandiseDetailDto?> GetByIdAsync(int id);
        Task<List<MerchandiseListDto>> SearchAsync(string query);
        Task<List<MerchandiseListDto>> GetByCategoryAsync(string category);
        Task<List<MerchandiseListDto>> GetOfficialProductsAsync();
        Task<List<MerchandiseDto>> GetBySellerIdAsync(int sellerId);

        // CRUD operations for Organizers (own merch)
        Task<MerchandiseDetailDto> CreateAsync(CreateMerchandiseDto dto, int sellerId);
        Task<MerchandiseDetailDto> UpdateAsync(int id, UpdateMerchandiseDto dto, int sellerId);
        Task<bool> DeleteAsync(int id, int sellerId);
        Task<bool> ToggleActiveStatusAsync(int id, int sellerId);

        // CRUD operations for Sales Staff & Admin (all merch)
        Task<MerchandiseDetailDto> AdminCreateAsync(CreateMerchandiseDto dto, int? sellerId = null, bool isOfficial = false);
        Task<MerchandiseDetailDto> AdminUpdateAsync(int id, UpdateMerchandiseDto dto);
        Task<bool> AdminDeleteAsync(int id);
        Task<PagedResult<MerchandiseDto>> GetAllForManagementAsync(int pageNumber, int pageSize, bool? isOfficial = null);

        // Variant operations
        Task<MerchandiseVariantDto> AddVariantAsync(int merchandiseId, CreateVariantDto dto, int userId);
        Task<MerchandiseVariantDto> UpdateVariantAsync(int variantId, UpdateVariantDto dto, int userId);
        Task<bool> DeleteVariantAsync(int variantId, int userId);
        Task<List<MerchandiseVariantDto>> GetVariantsByMerchandiseIdAsync(int merchandiseId);

        // Stock management
        Task<bool> UpdateStockAsync(int id, int quantity);
        Task<bool> ReserveStockAsync(int id, int? variantId, int quantity);
        Task<bool> ReleaseStockAsync(int id, int? variantId, int quantity);
    }
}
