using Domain.Entities;
using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Repo
{
    public class AdvertisementRepo : Repo<AdvertisementModel>, IAdvertisementRepo
    {
        public AdvertisementRepo(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<AdvertisementModel>> GetActiveAdvertisementsAsync(AdPosition? position = null)
        {
            var query = _context.Advertisements
                .Where(a => !a.IsDeleted &&
                           a.IsActive &&
                           a.Status == AdStatus.Active &&
                           a.StartDate <= DateTime.UtcNow &&
                           a.EndDate >= DateTime.UtcNow &&
                           a.SpentAmount < a.TotalBudget);

            if (position.HasValue)
            {
                query = query.Where(a => a.Position == position.Value);
            }

            return await query
                .OrderBy(a => a.DisplayOrder)
                .ThenBy(a => a.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<AdvertisementModel>> GetAdvertisementsByAdvertiserAsync(int advertiserId)
        {
            return await _context.Advertisements
                .Where(a => a.AdvertiserId == advertiserId && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<AdvertisementModel>> GetScheduledAdvertisementsAsync(DateTime date)
        {
            return await _context.Advertisements
                .Where(a => !a.IsDeleted &&
                           a.StartDate <= date &&
                           a.EndDate >= date)
                .OrderBy(a => a.DisplayOrder)
                .ToListAsync();
        }

        public async Task<AdvertisementModel?> GetAdvertisementWithAdvertiserAsync(int id)
        {
            return await _context.Advertisements
                .Include(a => a.Advertiser)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        }

        public async Task IncrementViewCountAsync(int id)
        {
            var ad = await _context.Advertisements.FindAsync(id);
            if (ad != null && !ad.IsDeleted)
            {
                ad.ViewCount++;
                ad.SpentAmount += ad.CostPerView;
                ad.UpdatedAt = DateTime.UtcNow;

                // Check if budget exhausted
                if (ad.SpentAmount >= ad.TotalBudget)
                {
                    ad.Status = AdStatus.Completed;
                    ad.IsActive = false;
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task IncrementClickCountAsync(int id)
        {
            var ad = await _context.Advertisements.FindAsync(id);
            if (ad != null && !ad.IsDeleted)
            {
                ad.ClickCount++;
                ad.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateSpentAmountAsync(int id, decimal amount)
        {
            var ad = await _context.Advertisements.FindAsync(id);
            if (ad != null && !ad.IsDeleted)
            {
                ad.SpentAmount = amount;
                ad.UpdatedAt = DateTime.UtcNow;

                // Check if budget exhausted
                if (ad.SpentAmount >= ad.TotalBudget)
                {
                    ad.Status = AdStatus.Completed;
                    ad.IsActive = false;
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<AdvertisementModel>> GetExpiringAdvertisementsAsync(int daysAhead)
        {
            var targetDate = DateTime.UtcNow.AddDays(daysAhead);

            return await _context.Advertisements
                .Where(a => !a.IsDeleted &&
                           a.IsActive &&
                           a.Status == AdStatus.Active &&
                           a.EndDate >= DateTime.UtcNow &&
                           a.EndDate <= targetDate)
                .OrderBy(a => a.EndDate)
                .ToListAsync();
        }

        public async Task<bool> HasActiveAdvertisementInPositionAsync(int advertiserId, AdPosition position)
        {
            return await _context.Advertisements
                .AnyAsync(a => a.AdvertiserId == advertiserId &&
                              a.Position == position &&
                              a.IsActive &&
                              a.Status == AdStatus.Active &&
                              !a.IsDeleted &&
                              a.StartDate <= DateTime.UtcNow &&
                              a.EndDate >= DateTime.UtcNow);
        }

        public async Task<IEnumerable<AdvertisementModel>> GetAdvertisementsByStatusAsync(AdStatus status)
        {
            return await _context.Advertisements
                .Where(a => a.Status == status && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalSpentByAdvertiserAsync(int advertiserId)
        {
            return await _context.Advertisements
                .Where(a => a.AdvertiserId == advertiserId && !a.IsDeleted)
                .SumAsync(a => a.SpentAmount);
        }
    }
}
 