using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface ISponsorRepo :IRepo<SponsorModel>
    {
        // Get active sponsors ordered by display order and sponsorship level
        Task<IEnumerable<SponsorModel>> GetActiveSponsorsAsync();

        // Get sponsors by sponsorship level
        Task<IEnumerable<SponsorModel>> GetBySponsorshipLevelAsync(string level);

        // Get sponsors with active contracts
        Task<IEnumerable<SponsorModel>> GetSponsorsWithActiveContractsAsync();

        // Get sponsors with expiring contracts (within specified days)
        Task<IEnumerable<SponsorModel>> GetSponsorsWithExpiringContractsAsync(int daysBeforeExpiry);

        // Get sponsor by name (for uniqueness check)
        Task<SponsorModel?> GetByNameAsync(string name);

        // Get sponsors ordered for display
        Task<IEnumerable<SponsorModel>> GetSponsorsForDisplayAsync();

        // Get sponsors with pagination
        Task<(IEnumerable<SponsorModel> Items, int TotalCount)> GetPaginatedSponsorsAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            string? sponsorshipLevel = null,
            bool? isActive = null);

        // Update display order for multiple sponsors
        Task UpdateDisplayOrderAsync(Dictionary<int, int> sponsorDisplayOrders);

        // Get total contribution amount by sponsorship level
        Task<Dictionary<string, decimal>> GetContributionStatisticsAsync();

        // Check if email already exists (excluding current sponsor)
        Task<bool> IsEmailUniqueAsync(string email, int? excludeSponsorId = null);

        // Bulk update sponsor status
        Task BulkUpdateStatusAsync(List<int> sponsorIds, bool isActive);
    }
}