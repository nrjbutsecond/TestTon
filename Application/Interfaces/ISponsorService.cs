using Application.DTOs.Sponsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISponsorService
    {
        Task<SponsorDto?> GetByIdAsync(int id);
        Task<List<PublicSponsorDto>> GetActiveSponsorsForPublicAsync();
        Task<SponsorListDto> GetPaginatedAsync(SponsorFilterDto filter);
        Task<SponsorDto> CreateAsync(CreateSponsorDto dto);
        Task<SponsorDto?> UpdateAsync(int id, UpdateSponsorDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> BulkOperationAsync(BulkSponsorOperationDto dto);
        Task<bool> UpdateDisplayOrderAsync(UpdateDisplayOrderDto dto);
        Task<SponsorStatisticsDto> GetStatisticsAsync();
        Task<List<SponsorDto>> GetExpiringContractsAsync(int daysBeforeExpiry = 30);
        Task<bool> IsNameUniqueAsync(string name, int? excludeId = null);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
    }
}
