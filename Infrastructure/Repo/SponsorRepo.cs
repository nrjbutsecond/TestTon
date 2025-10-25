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
    public class SponsorRepo :Repo<SponsorModel>, ISponsorRepo
    {
        public SponsorRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SponsorModel>> GetActiveSponsorsAsync()
        {
            return await _context.Sponsors
                .Where(s => s.IsActive && !s.IsDeleted)
                .OrderBy(s => s.DisplayOrder)
                .ThenByDescending(s => SponsorshipLevelEnum.GetPriority(s.SponsorshipLevel))
                .ToListAsync();
        }

        public async Task<IEnumerable<SponsorModel>> GetBySponsorshipLevelAsync(string level)
        {
            return await _context.Sponsors
                .Where(s => s.SponsorshipLevel == level && !s.IsDeleted)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<SponsorModel>> GetSponsorsWithActiveContractsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Sponsors
                .Where(s => !s.IsDeleted
                    && s.ContractStartDate.HasValue
                    && s.ContractEndDate.HasValue
                    && s.ContractStartDate.Value <= now
                    && s.ContractEndDate.Value >= now)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<SponsorModel>> GetSponsorsWithExpiringContractsAsync(int daysBeforeExpiry)
        {
            var now = DateTime.UtcNow;
            var expiryDate = now.AddDays(daysBeforeExpiry);

            return await _context.Sponsors
                .Where(s => !s.IsDeleted
                    && s.ContractEndDate.HasValue
                    && s.ContractEndDate.Value >= now
                    && s.ContractEndDate.Value <= expiryDate)
                .OrderBy(s => s.ContractEndDate)
                .ToListAsync();
        }

        public async Task<SponsorModel?> GetByNameAsync(string name)
        {
            return await _context.Sponsors
                .FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower() && !s.IsDeleted);
        }

        public async Task<IEnumerable<SponsorModel>> GetSponsorsForDisplayAsync()
        {
            return await _context.Sponsors
                .Where(s => s.IsActive && !s.IsDeleted)
                .OrderBy(s => s.DisplayOrder)
                .ThenByDescending(s => SponsorshipLevelEnum.GetPriority(s.SponsorshipLevel))
                .ThenBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<(IEnumerable<SponsorModel> Items, int TotalCount)> GetPaginatedSponsorsAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            string? sponsorshipLevel = null,
            bool? isActive = null)
        {
            var query = _context.Sponsors.Where(s => !s.IsDeleted);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(s =>
                    s.Name.ToLower().Contains(searchTerm) ||
                    (s.ContactPerson != null && s.ContactPerson.ToLower().Contains(searchTerm)) ||
                    (s.ContactEmail != null && s.ContactEmail.ToLower().Contains(searchTerm)) ||
                    (s.Description != null && s.Description.ToLower().Contains(searchTerm)));
            }

            if (!string.IsNullOrWhiteSpace(sponsorshipLevel))
            {
                query = query.Where(s => s.SponsorshipLevel == sponsorshipLevel);
            }

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsActive == isActive.Value);
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var items = await query
                .OrderBy(s => s.DisplayOrder)
                .ThenByDescending(s => SponsorshipLevelEnum.GetPriority(s.SponsorshipLevel))
                .ThenBy(s => s.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task UpdateDisplayOrderAsync(Dictionary<int, int> sponsorDisplayOrders)
        {
            var sponsorIds = sponsorDisplayOrders.Keys.ToList();
            var sponsors = await _context.Sponsors
                .Where(s => sponsorIds.Contains(s.Id))
                .ToListAsync();

            foreach (var sponsor in sponsors)
            {
                if (sponsorDisplayOrders.TryGetValue(sponsor.Id, out int newOrder))
                {
                    sponsor.DisplayOrder = newOrder;
                    sponsor.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Dictionary<string, decimal>> GetContributionStatisticsAsync()
        {
            var statistics = await _context.Sponsors
                .Where(s => !s.IsDeleted && s.IsActive)
                .GroupBy(s => s.SponsorshipLevel)
                .Select(g => new
                {
                    Level = g.Key,
                    TotalAmount = g.Sum(s => s.ContributionAmount)
                })
                .ToDictionaryAsync(x => x.Level, x => x.TotalAmount);

            // Ensure all levels are represented
            foreach (var level in SponsorshipLevelEnum.All)
            {
                if (!statistics.ContainsKey(level))
                {
                    statistics[level] = 0;
                }
            }

            return statistics;
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeSponsorId = null)
        {
            var query = _context.Sponsors
                .Where(s => !s.IsDeleted && s.ContactEmail == email);

            if (excludeSponsorId.HasValue)
            {
                query = query.Where(s => s.Id != excludeSponsorId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task BulkUpdateStatusAsync(List<int> sponsorIds, bool isActive)
        {
            var sponsors = await _context.Sponsors
                .Where(s => sponsorIds.Contains(s.Id) && !s.IsDeleted)
                .ToListAsync();

            foreach (var sponsor in sponsors)
            {
                sponsor.IsActive = isActive;
                sponsor.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}