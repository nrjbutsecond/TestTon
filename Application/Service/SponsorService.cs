using Application.DTOs.Sponsor;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Service
{
    public class SponsorService : ISponsorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SponsorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SponsorDto?> GetByIdAsync(int id)
        {
            var sponsor = await _unitOfWork.Sponsors.GetByIdAsync(id);
            if (sponsor == null) return null;

            return _mapper.Map<SponsorDto>(sponsor);
        }

        public async Task<List<PublicSponsorDto>> GetActiveSponsorsForPublicAsync()
        {
            var sponsors = await _unitOfWork.Sponsors.GetSponsorsForDisplayAsync();
            return _mapper.Map<List<PublicSponsorDto>>(sponsors);
        }

        public async Task<SponsorListDto> GetPaginatedAsync(SponsorFilterDto filter)
        {
            var (items, totalCount) = await _unitOfWork.Sponsors.GetPaginatedSponsorsAsync(
                filter.PageNumber,
                filter.PageSize,
                filter.SearchTerm,
                filter.SponsorshipLevel,
                filter.IsActive
            );

            return new SponsorListDto
            {
                Items = _mapper.Map<List<SponsorDto>>(items),
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<SponsorDto> CreateAsync(CreateSponsorDto dto)
        {
            // Validate sponsorship level
            if (!SponsorshipLevelEnum.IsValid(dto.SponsorshipLevel))
            {
                throw new ArgumentException($"Invalid sponsorship level: {dto.SponsorshipLevel}");
            }

            // Check name uniqueness
            if (!await IsNameUniqueAsync(dto.Name, null))
            {
                throw new InvalidOperationException($"A sponsor with name '{dto.Name}' already exists.");
            }

            // Check email uniqueness if provided
            if (!string.IsNullOrEmpty(dto.ContactEmail) && !await IsEmailUniqueAsync(dto.ContactEmail, null))
            {
                throw new InvalidOperationException($"A sponsor with email '{dto.ContactEmail}' already exists.");
            }

            // Validate contract dates
            if (dto.ContractStartDate.HasValue && dto.ContractEndDate.HasValue)
            {
                if (dto.ContractEndDate.Value < dto.ContractStartDate.Value)
                {
                    throw new ArgumentException("Contract end date must be after start date.");
                }
            }

            var sponsor = _mapper.Map<SponsorModel>(dto);

            await _unitOfWork.Sponsors.AddAsync(sponsor);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SponsorDto>(sponsor);
        }

        public async Task<SponsorDto?> UpdateAsync(int id, UpdateSponsorDto dto)
        {
            var sponsor = await _unitOfWork.Sponsors.GetByIdAsync(id);
            if (sponsor == null) return null;

            // Validate sponsorship level
            if (!SponsorshipLevelEnum.IsValid(dto.SponsorshipLevel))
            {
                throw new ArgumentException($"Invalid sponsorship level: {dto.SponsorshipLevel}");
            }

            // Check name uniqueness
            if (sponsor.Name != dto.Name && !await IsNameUniqueAsync(dto.Name, id))
            {
                throw new InvalidOperationException($"A sponsor with name '{dto.Name}' already exists.");
            }

            // Check email uniqueness if changed
            if (sponsor.ContactEmail != dto.ContactEmail &&
                !string.IsNullOrEmpty(dto.ContactEmail) &&
                !await IsEmailUniqueAsync(dto.ContactEmail, id))
            {
                throw new InvalidOperationException($"A sponsor with email '{dto.ContactEmail}' already exists.");
            }

            // Validate contract dates
            if (dto.ContractStartDate.HasValue && dto.ContractEndDate.HasValue)
            {
                if (dto.ContractEndDate.Value < dto.ContractStartDate.Value)
                {
                    throw new ArgumentException("Contract end date must be after start date.");
                }
            }

            // Map DTO to entity (only non-null values will be updated due to mapping configuration)
            _mapper.Map(dto, sponsor);

            _unitOfWork.Sponsors.Update(sponsor);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SponsorDto>(sponsor);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sponsor = await _unitOfWork.Sponsors.GetByIdAsync(id);
            if (sponsor == null) return false;

            _unitOfWork.Sponsors.Remove(sponsor);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BulkOperationAsync(BulkSponsorOperationDto dto)
        {
            switch (dto.Operation.ToLower())
            {
                case "activate":
                    await _unitOfWork.Sponsors.BulkUpdateStatusAsync(dto.SponsorIds, true);
                    break;
                case "deactivate":
                    await _unitOfWork.Sponsors.BulkUpdateStatusAsync(dto.SponsorIds, false);
                    break;
                case "delete":
                    var sponsors = await _unitOfWork.Sponsors.FindAsync(s => dto.SponsorIds.Contains(s.Id));
                    _unitOfWork.Sponsors.RemoveRange(sponsors);
                    await _unitOfWork.SaveChangesAsync();
                    break;
                default:
                    throw new ArgumentException($"Invalid operation: {dto.Operation}");
            }
            return true;
        }

        public async Task<bool> UpdateDisplayOrderAsync(UpdateDisplayOrderDto dto)
        {
            await _unitOfWork.Sponsors.UpdateDisplayOrderAsync(dto.SponsorDisplayOrders);
            return true;
        }

        public async Task<SponsorStatisticsDto> GetStatisticsAsync()
        {
            var allSponsors = await _unitOfWork.Sponsors.GetAllAsync();
            var activeSponsors = await _unitOfWork.Sponsors.GetActiveSponsorsAsync();
            var activeContracts = await _unitOfWork.Sponsors.GetSponsorsWithActiveContractsAsync();
            var expiringContracts = await _unitOfWork.Sponsors.GetSponsorsWithExpiringContractsAsync(30);
            var contributionStats = await _unitOfWork.Sponsors.GetContributionStatisticsAsync();

            var countByLevel = allSponsors
                .Where(s => !s.IsDeleted)
                .GroupBy(s => s.SponsorshipLevel)
                .ToDictionary(g => g.Key, g => g.Count());

            // Ensure all levels are represented
            foreach (var level in SponsorshipLevelEnum.All)
            {
                if (!countByLevel.ContainsKey(level))
                {
                    countByLevel[level] = 0;
                }
            }

            return new SponsorStatisticsDto
            {
                TotalSponsors = allSponsors.Count(s => !s.IsDeleted),
                ActiveSponsors = activeSponsors.Count(),
                ActiveContracts = activeContracts.Count(),
                ExpiringContracts = expiringContracts.Count(),
                TotalContributions = contributionStats.Values.Sum(),
                ContributionsByLevel = contributionStats,
                CountByLevel = countByLevel
            };
        }

        public async Task<List<SponsorDto>> GetExpiringContractsAsync(int daysBeforeExpiry = 30)
        {
            var sponsors = await _unitOfWork.Sponsors.GetSponsorsWithExpiringContractsAsync(daysBeforeExpiry);
            return _mapper.Map<List<SponsorDto>>(sponsors);
        }

        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
        {
            var existing = await _unitOfWork.Sponsors.GetByNameAsync(name);
            return existing == null || (excludeId.HasValue && existing.Id == excludeId.Value);
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
        {
            return await _unitOfWork.Sponsors.IsEmailUniqueAsync(email, excludeId);
        }
    }
}