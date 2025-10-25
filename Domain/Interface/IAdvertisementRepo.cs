using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IAdvertisementRepo :IRepo<AdvertisementModel>
    {
        Task<IEnumerable<AdvertisementModel>> GetActiveAdvertisementsAsync(AdPosition? position = null);
        Task<IEnumerable<AdvertisementModel>> GetAdvertisementsByAdvertiserAsync(int advertiserId);
        Task<IEnumerable<AdvertisementModel>> GetScheduledAdvertisementsAsync(DateTime date);
        Task<AdvertisementModel?> GetAdvertisementWithAdvertiserAsync(int id);
        Task IncrementViewCountAsync(int id);
        Task IncrementClickCountAsync(int id);
        Task UpdateSpentAmountAsync(int id, decimal amount);
        Task<IEnumerable<AdvertisementModel>> GetExpiringAdvertisementsAsync(int daysAhead);
        Task<bool> HasActiveAdvertisementInPositionAsync(int advertiserId, AdPosition position);
        Task<IEnumerable<AdvertisementModel>> GetAdvertisementsByStatusAsync(AdStatus status);
        Task<decimal> GetTotalSpentByAdvertiserAsync(int advertiserId);
    }
}
