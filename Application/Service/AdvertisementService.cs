using Application.DTOs.Advertisement;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class AdvertisementService: IAdvertisementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdvertisementService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AdvertisementDto> GetByIdAsync(int id)
        {
            var advertisement = await _unitOfWork.Advertisements.GetAdvertisementWithAdvertiserAsync(id);
            if (advertisement == null)
                throw new KeyNotFoundException($"Advertisement with ID {id} not found");

            var dto = _mapper.Map<AdvertisementDto>(advertisement);

            // Set calculated properties
            dto.RemainingBudget = advertisement.RemainingBudget;
            dto.ClickThroughRate = advertisement.ClickThroughRate;
            dto.IsExpired = advertisement.IsExpired;
            dto.IsScheduledToStart = advertisement.IsScheduledToStart;

            // Set TargetAudience if not mapped
            if (string.IsNullOrEmpty(dto.TargetAudience))
                dto.TargetAudience = advertisement.TargetAudience ?? "{}";

            return dto;
        }

        public async Task<IEnumerable<AdvertisementListDto>> GetAllAsync()
        {
            var advertisements = await _unitOfWork.Advertisements.GetAllAsync();
            return _mapper.Map<IEnumerable<AdvertisementListDto>>(advertisements);
        }

        public async Task<IEnumerable<AdvertisementListDto>> GetActiveAdvertisementsAsync(string? position = null)
        {
            AdPosition? adPosition = null;
            if (!string.IsNullOrEmpty(position) && Enum.TryParse<AdPosition>(position, true, out var parsedPosition))
            {
                adPosition = parsedPosition;
            }

            var advertisements = await _unitOfWork.Advertisements.GetActiveAdvertisementsAsync(adPosition);
            return _mapper.Map<IEnumerable<AdvertisementListDto>>(advertisements);
        }

        public async Task<IEnumerable<AdvertisementListDto>> GetAdvertiserAdsAsync(int advertiserId)
        {
            var advertisements = await _unitOfWork.Advertisements.GetAdvertisementsByAdvertiserAsync(advertiserId);
            return _mapper.Map<IEnumerable<AdvertisementListDto>>(advertisements);
        }

        public async Task<AdvertisementDto> CreateAsync(CreateAdvertisementDto dto, int advertiserId)
        {
            // Validate advertiser exists and has Admin role
            var advertiser = await _unitOfWork.Users.GetByIdAsync(advertiserId);
            if (advertiser == null || advertiser.Role != UserRoles.Admin)
                throw new UnauthorizedAccessException("Only Admin can create advertisements");

            // Check if advertiser already has an active ad in this position
            if (await _unitOfWork.Advertisements.HasActiveAdvertisementInPositionAsync(
                advertiserId, Enum.Parse<AdPosition>(dto.Position, true)))
            {
                throw new InvalidOperationException($"You already have an active advertisement in position {dto.Position}");
            }

            var advertisement = _mapper.Map<AdvertisementModel>(dto);
            advertisement.AdvertiserId = advertiserId;
            advertisement.CreatedBy = advertiserId.ToString();
            advertisement.CreatedAt = DateTime.UtcNow;

            // Validate dates
            if (advertisement.StartDate >= advertisement.EndDate)
                throw new ArgumentException("End date must be after start date");

            if (advertisement.StartDate < DateTime.UtcNow.Date)
                throw new ArgumentException("Start date cannot be in the past");

            await _unitOfWork.Advertisements.AddAsync(advertisement);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(advertisement.Id);
        }

        public async Task<AdvertisementDto> UpdateAsync(int id, UpdateAdvertisementDto dto, int updatedBy)
        {
            var advertisement = await _unitOfWork.Advertisements.GetByIdAsync(id);
            if (advertisement == null)
                throw new KeyNotFoundException($"Advertisement with ID {id} not found");

            // Check ownership or admin role
            var user = await _unitOfWork.Users.GetByIdAsync(updatedBy);
            if (user == null || (advertisement.AdvertiserId != updatedBy && user.Role != UserRoles.Admin))
                throw new UnauthorizedAccessException("You don't have permission to update this advertisement");

            // Apply updates manually for better control
            if (!string.IsNullOrEmpty(dto.Title))
                advertisement.Title = dto.Title;

            if (!string.IsNullOrEmpty(dto.Description))
                advertisement.Description = dto.Description;

            if (!string.IsNullOrEmpty(dto.BannerImageUrl))
                advertisement.BannerImageUrl = dto.BannerImageUrl;

            if (!string.IsNullOrEmpty(dto.TargetUrl))
                advertisement.TargetUrl = dto.TargetUrl;

            if (!string.IsNullOrEmpty(dto.AdType) && Enum.TryParse<AdType>(dto.AdType, true, out var adType))
                advertisement.AdType = adType;

            if (!string.IsNullOrEmpty(dto.Position) && Enum.TryParse<AdPosition>(dto.Position, true, out var position))
                advertisement.Position = position;

            if (!string.IsNullOrEmpty(dto.Status) && Enum.TryParse<AdStatus>(dto.Status, true, out var status))
                advertisement.Status = status;

            if (dto.DisplayOrder.HasValue)
                advertisement.DisplayOrder = dto.DisplayOrder.Value;

            if (dto.CostPerView.HasValue)
                advertisement.CostPerView = dto.CostPerView.Value;

            if (dto.TotalBudget.HasValue)
                advertisement.TotalBudget = dto.TotalBudget.Value;

            if (dto.StartDate.HasValue)
                advertisement.StartDate = dto.StartDate.Value;

            if (dto.EndDate.HasValue)
                advertisement.EndDate = dto.EndDate.Value;

            if (dto.IsActive.HasValue)
                advertisement.IsActive = dto.IsActive.Value;

            if (!string.IsNullOrEmpty(dto.TargetAudience))
                advertisement.TargetAudience = dto.TargetAudience;

            advertisement.UpdatedAt = DateTime.UtcNow;
            advertisement.UpdatedBy = updatedBy.ToString();

            // Validate dates if changed
            if (advertisement.StartDate >= advertisement.EndDate)
                throw new ArgumentException("End date must be after start date");

            _unitOfWork.Advertisements.Update(advertisement);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id, string deletedBy)
        {
            var advertisement = await _unitOfWork.Advertisements.GetByIdAsync(id);
            if (advertisement == null)
                return false;

            advertisement.IsDeleted = true;
            advertisement.DeletedAt = DateTime.UtcNow;
            advertisement.UpdatedBy = deletedBy;

            _unitOfWork.Advertisements.Update(advertisement);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateAsync(int id, int userId)
        {
            var advertisement = await _unitOfWork.Advertisements.GetByIdAsync(id);
            if (advertisement == null)
                return false;

            // Check ownership or admin role
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null || (advertisement.AdvertiserId != userId && user.Role != UserRoles.Admin))
                throw new UnauthorizedAccessException("You don't have permission to activate this advertisement");

            // Check if dates are valid
            if (advertisement.StartDate > DateTime.UtcNow)
            {
                advertisement.Status = AdStatus.Active;
                advertisement.IsActive = false; // Will be activated automatically on start date
            }
            else if (advertisement.EndDate < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Cannot activate an expired advertisement");
            }
            else
            {
                advertisement.Status = AdStatus.Active;
                advertisement.IsActive = true;
            }

            advertisement.UpdatedAt = DateTime.UtcNow;
            advertisement.UpdatedBy = userId.ToString();

            _unitOfWork.Advertisements.Update(advertisement);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PauseAsync(int id, int userId)
        {
            var advertisement = await _unitOfWork.Advertisements.GetByIdAsync(id);
            if (advertisement == null)
                return false;

            // Check ownership or admin role
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null || (advertisement.AdvertiserId != userId && user.Role != UserRoles.Admin))
                throw new UnauthorizedAccessException("You don't have permission to pause this advertisement");

            advertisement.Status = AdStatus.Paused;
            advertisement.IsActive = false;
            advertisement.UpdatedAt = DateTime.UtcNow;
            advertisement.UpdatedBy = userId.ToString();

            _unitOfWork.Advertisements.Update(advertisement);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task RecordViewAsync(int id)
        {
            await _unitOfWork.Advertisements.IncrementViewCountAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RecordClickAsync(int id)
        {
            await _unitOfWork.Advertisements.IncrementClickCountAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<AdvertisementStatisticsDto> GetStatisticsAsync(int? advertiserId = null)
        {
            IEnumerable<AdvertisementModel> advertisements;

            if (advertiserId.HasValue)
            {
                advertisements = await _unitOfWork.Advertisements
                    .GetAdvertisementsByAdvertiserAsync(advertiserId.Value);
            }
            else
            {
                advertisements = await _unitOfWork.Advertisements.GetAllAsync();
            }

            var adsList = advertisements.ToList();

            return new AdvertisementStatisticsDto
            {
                TotalAds = adsList.Count,
                ActiveAds = adsList.Count(a => a.IsActive && a.Status == AdStatus.Active),
                TotalViews = adsList.Sum(a => a.ViewCount),
                TotalClicks = adsList.Sum(a => a.ClickCount),
                AverageClickThroughRate = adsList.Any(a => a.ViewCount > 0)
                    ? adsList.Where(a => a.ViewCount > 0).Average(a => a.ClickThroughRate)
                    : 0,
                TotalSpent = adsList.Sum(a => a.SpentAmount),
                TotalBudget = adsList.Sum(a => a.TotalBudget),
                BudgetUtilization = adsList.Sum(a => a.TotalBudget) > 0
                    ? (adsList.Sum(a => a.SpentAmount) / adsList.Sum(a => a.TotalBudget)) * 100
                    : 0
            };
        }

        public async Task<IEnumerable<AdvertisementListDto>> GetExpiringAdsAsync(int daysAhead = 7)
        {
            var advertisements = await _unitOfWork.Advertisements.GetExpiringAdvertisementsAsync(daysAhead);
            return _mapper.Map<IEnumerable<AdvertisementListDto>>(advertisements);
        }

        public async Task<bool> HasActiveAdInPositionAsync(int advertiserId, string position)
        {
            if (!Enum.TryParse<AdPosition>(position, true, out var adPosition))
                throw new ArgumentException($"Invalid position value: {position}");

            return await _unitOfWork.Advertisements.HasActiveAdvertisementInPositionAsync(advertiserId, adPosition);
        }
    }
}