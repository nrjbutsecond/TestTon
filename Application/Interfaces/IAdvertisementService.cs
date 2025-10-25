using Application.DTOs.Advertisement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAdvertisementService
    {
        Task<AdvertisementDto> GetByIdAsync(int id);
        Task<IEnumerable<AdvertisementListDto>> GetAllAsync();
        Task<IEnumerable<AdvertisementListDto>> GetActiveAdvertisementsAsync(string? position = null);
        Task<IEnumerable<AdvertisementListDto>> GetAdvertiserAdsAsync(int advertiserId);
        Task<AdvertisementDto> CreateAsync(CreateAdvertisementDto dto, int advertiserId);
        Task<AdvertisementDto> UpdateAsync(int id, UpdateAdvertisementDto dto, int updatedBy);
        Task<bool> DeleteAsync(int id, string deletedBy);
        Task<bool> ActivateAsync(int id, int userId);
        Task<bool> PauseAsync(int id, int userId);
        Task RecordViewAsync(int id);
        Task RecordClickAsync(int id);
        Task<AdvertisementStatisticsDto> GetStatisticsAsync(int? advertiserId = null);
        Task<IEnumerable<AdvertisementListDto>> GetExpiringAdsAsync(int daysAhead = 7);
        Task<bool> HasActiveAdInPositionAsync(int advertiserId, string position);
    }
}
