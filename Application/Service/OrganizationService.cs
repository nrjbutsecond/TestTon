using Application.DTOs.Merchandise;
using Application.DTOs.Organization;
using Application.DTOs.TalkEvent;
using Application.Interfaces.OrganizationServiceInterface;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Organize;
using Domain.Interface;
using Domain.Interface.OrganizationRepoFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrganizationRepo _organizationRepo;
        private readonly IOrganizationMemberRepo _memberRepo;
        private readonly IPartnershipApplicationRepo _applicationRepo;
        private readonly IOrganizationActivityRepo _activityRepo;
        private readonly IOrganizationStatisticsRepo _statisticsRepo;
        private readonly IMapper _mapper;

        public OrganizationService(
            IUnitOfWork unitOfWork,
            IOrganizationRepo organizationRepo,
            IOrganizationMemberRepo memberRepo,
            IPartnershipApplicationRepo applicationRepo,
            IOrganizationActivityRepo activityRepo,
            IOrganizationStatisticsRepo statisticsRepo,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _organizationRepo = organizationRepo;
            _memberRepo = memberRepo;
            _applicationRepo = applicationRepo;
            _activityRepo = activityRepo;
            _statisticsRepo = statisticsRepo;
            _mapper = mapper;
        }

        public async Task<OrganizationDto> GetOrganizationByIdAsync(int id)
        {
            var organization = await _organizationRepo.GetOrganizationWithDetailsAsync(id);
            if (organization == null)
                throw new KeyNotFoundException($"Organization with ID {id} not found");

            var dto = _mapper.Map<OrganizationDto>(organization);

            // Add last activity date
            if (organization.Activities?.Any() == true)
            {
                dto.LastActivityDate = organization.Activities
                    .OrderByDescending(a => a.ActivityDate)
                    .First().ActivityDate;
            }

            return dto;
        }

        public async Task<OrganizationDto> CreateOrganizationAsync(CreateOrganizationDto dto, string createdBy, int creatorUserId)
        {
            // Validate unique name
            if (!await _organizationRepo.IsNameUniqueAsync(dto.Name))
                throw new InvalidOperationException($"Organization name '{dto.Name}' already exists");

            var organization = _mapper.Map<OrganizationModel>(dto);
            organization.CreatedBy = createdBy;
            organization.CreatedAt = DateTime.UtcNow;
            organization.Location = $"{dto.Address}, {dto.District}, {dto.City}, {dto.Country}";
            organization.Status = PartnershipStatus.Pending;
            organization.Type = OrganizationType.Standard;

            var created = await _organizationRepo.AddAsync(organization);
            await _unitOfWork.SaveChangesAsync();

            // Add creator as admin member
            var member = new OrganizationMember
            {
                OrganizationId = created.Id,
                UserId = creatorUserId,
                Role = OrganizationRole.Admin,
                JoinedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            await _memberRepo.AddAsync(member);
            await _unitOfWork.SaveChangesAsync();

            // Log activity
            await _activityRepo.LogActivityAsync(created.Id, ActivityType.Created,
                $"Organization '{created.Name}' was created", creatorUserId);

            return _mapper.Map<OrganizationDto>(created);
        }

        // Dashboard with events and merchandise
        public async Task<OrganizationDashboardDto> GetOrganizationDashboardAsync(int id)
        {
            var organization = await GetOrganizationByIdAsync(id);
            var activities = await GetOrganizationActivitiesAsync(id, 10);

            // Get events through member relationship
            var events = await _organizationRepo.GetOrganizationEventsAsync(id);
            var recentEvents = events
                .OrderByDescending(e => e.StartDate)
                .Take(5)
                .Select(e => new EventSummaryDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    StartDate = e.StartDate,
                    AttendeeCount = e.MaxAttendees,
                    Status = e.Status.ToString()
                })
                .ToList();

            // Get current month statistics
            var now = DateTime.UtcNow;
            var currentStats = await _statisticsRepo.GetStatisticsByPeriodAsync(id, now.Year, now.Month)
                ?? await _statisticsRepo.CalculateCurrentStatisticsAsync(id);

            var yearlyStats = await GetOrganizationStatisticsAsync(id, now.Year);

            return new OrganizationDashboardDto
            {
                Organization = organization,
                RecentActivities = activities.ToList(),
                RecentEvents = recentEvents,
                CurrentMonthStats = _mapper.Map<OrganizationStatisticsDto>(currentStats),
                YearlyStats = yearlyStats.ToList()
            };
        }

        // Get organization's events through members
        public async Task<IEnumerable<TalkEventDto>> GetOrganizationEventsAsync(int organizationId)
        {
            var events = await _organizationRepo.GetOrganizationEventsAsync(organizationId);
            return events.Select(e => new TalkEventDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Location = e.Location,
                MaxAttendees = e.MaxAttendees,
                Status = e.Status.ToString(),
                HasTicketSale = e.HasTicketSale,
                BannerImage = e.BannerImage,
                ThumbnailImage = e.ThumbnailImage
            });
        }

        // Get organization's merchandise through members
        public async Task<IEnumerable<MerchandiseOrganizationDto>> GetOrganizationMerchandiseAsync(int organizationId)
        {
            var merchandise = await _organizationRepo.GetOrganizationMerchandiseAsync(organizationId);
            return merchandise.Select(m => new MerchandiseOrganizationDto
            {
                Id = m.Id,
                SKU = m.SKU,
                Name = m.Name,
                Description = m.Description,
                Category = m.Category,
                BasePrice = m.BasePrice,
                StockQuantity = m.StockQuantity,
                IsActive = m.IsActive,
                Images = m.Images
            });
        }

        // Member Management with authorization
        public async Task<OrganizationMemberDto> AddMemberAsync(int organizationId, AddOrganizationMemberDto dto, string addedBy, int addedByUserId)
        {
            // Check if adding user has permission (must be admin or organizer)
            if (!await _memberRepo.CanUserManageOrganizationContentAsync(addedByUserId, organizationId))
                throw new UnauthorizedAccessException("You don't have permission to add members to this organization");

            // Check if user is already a member
            if (await _memberRepo.IsMemberOfOrganizationAsync(dto.UserId, organizationId))
                throw new InvalidOperationException("User is already a member of this organization");

            var member = new OrganizationMember
            {
                OrganizationId = organizationId,
                UserId = dto.UserId,
                Role = Enum.Parse<OrganizationRole>(dto.Role),
                JoinedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = addedBy,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _memberRepo.AddAsync(member);
            await _unitOfWork.SaveChangesAsync();

            // Get member with details
            var memberWithDetails = await _memberRepo.GetMemberWithDetailsAsync(created.Id);

            // Log activity
            await _activityRepo.LogActivityAsync(organizationId, ActivityType.MemberAdded,
                $"User '{memberWithDetails.User.FullName}' was added as {dto.Role}", addedByUserId);

            return _mapper.Map<OrganizationMemberDto>(memberWithDetails);
        }

        public async Task<bool> UpdateMemberRoleAsync(UpdateMemberRoleDto dto, string updatedBy, int updatedByUserId)
        {
            var member = await _memberRepo.GetByIdAsync(dto.MemberId);
            if (member == null)
                throw new KeyNotFoundException($"Member with ID {dto.MemberId} not found");

            // Check permission
            if (!await _memberRepo.HasRoleInOrganizationAsync(updatedByUserId, member.OrganizationId, OrganizationRole.Admin))
                throw new UnauthorizedAccessException("Only admins can update member roles");

            var oldRole = member.Role;
            member.Role = Enum.Parse<OrganizationRole>(dto.Role);
            member.UpdatedBy = updatedBy;
            member.UpdatedAt = DateTime.UtcNow;

            _memberRepo.Update(member);
            await _unitOfWork.SaveChangesAsync();

            // Log activity
            await _activityRepo.LogActivityAsync(member.OrganizationId, ActivityType.RoleChanged,
                $"Member role changed from {oldRole} to {dto.Role}", updatedByUserId);

            return true;
        }

        // Authorization helpers
        public async Task<bool> CanUserCreateContentForOrganization(int userId, int organizationId)
        {
            return await _memberRepo.CanUserManageOrganizationContentAsync(userId, organizationId);
        }

        public async Task<bool> IsMemberOfOrganizationAsync(int userId, int organizationId)
        {
            return await _memberRepo.IsMemberOfOrganizationAsync(userId, organizationId);
        }

        public async Task<bool> IsUserOrganizerInOrganization(int userId, int organizationId)
        {
            return await _memberRepo.HasRoleInOrganizationAsync(userId, organizationId, OrganizationRole.Organizer)
                || await _memberRepo.HasRoleInOrganizationAsync(userId, organizationId, OrganizationRole.Admin);
        }

        // Get organization member for event/merchandise creation
        public async Task<OrganizationMember> GetOrganizerMemberAsync(int userId, int organizationId)
        {
            return await _memberRepo.GetOrganizerMemberAsync(userId, organizationId);
        }

        // Statistics
        public async Task UpdateOrganizationStatisticsAsync(int organizationId)
        {
            // Update cached statistics
            await _organizationRepo.UpdateStatisticsAsync(organizationId);

            // Update monthly statistics
            var now = DateTime.UtcNow;
            await _statisticsRepo.UpdateOrCreateStatisticsAsync(organizationId, now.Year, now.Month);
        }

        public async Task<IEnumerable<OrganizationStatisticsDto>> GetOrganizationStatisticsAsync(int organizationId, int year)
        {
            var stats = await _statisticsRepo.GetYearlyStatisticsAsync(organizationId, year);

            // If no stats exist, calculate them
            if (!stats.Any())
            {
                for (int month = 1; month <= 12; month++)
                {
                    await _statisticsRepo.UpdateOrCreateStatisticsAsync(organizationId, year, month);
                }
                stats = await _statisticsRepo.GetYearlyStatisticsAsync(organizationId, year);
            }

            return _mapper.Map<IEnumerable<OrganizationStatisticsDto>>(stats);
        }

        // Activity logging helpers for other services
        public async Task LogEventCreatedAsync(int userId, int eventId, string eventTitle)
        {
            var member = await _memberRepo.GetOrganizationsForUserAsync(userId);
            if (member.Any())
            {
                var orgId = member.First().OrganizationId;
                await _activityRepo.LogActivityAsync(orgId, ActivityType.EventCreated,
                    $"Event '{eventTitle}' was created", userId);
            }
        }

        public async Task LogMerchandiseAddedAsync(int userId, int merchandiseId, string merchandiseName)
        {
            var member = await _memberRepo.GetOrganizationsForUserAsync(userId);
            if (member.Any())
            {
                var orgId = member.First().OrganizationId;
                await _activityRepo.LogActivityAsync(orgId, ActivityType.MerchandiseAdded,
                    $"Merchandise '{merchandiseName}' was added", userId);
            }
        }

        public async Task<IEnumerable<OrganizationListDto>> GetOrganizationsAsync(OrganizationFilterDto filter)
        {
            var query = _organizationRepo.GetQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(o => o.Name.Contains(filter.SearchTerm)
                    || o.Description.Contains(filter.SearchTerm)
                    || o.City.Contains(filter.SearchTerm));
            }

            if (!string.IsNullOrWhiteSpace(filter.Type))
            {
                if (Enum.TryParse<OrganizationType>(filter.Type, out var type))
                    query = query.Where(o => o.Type == type);
            }

            if (!string.IsNullOrWhiteSpace(filter.PartnershipTier))
            {
                if (Enum.TryParse<PartnershipTier>(filter.PartnershipTier, out var tier))
                    query = query.Where(o => o.PartnershipTier == tier);
            }

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                if (Enum.TryParse<PartnershipStatus>(filter.Status, out var status))
                    query = query.Where(o => o.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(filter.City))
            {
                query = query.Where(o => o.City == filter.City);
            }

            if (filter.MinRating.HasValue)
            {
                query = query.Where(o => o.Rating >= filter.MinRating.Value);
            }

            // Apply sorting
            query = filter.SortBy?.ToLower() switch
            {
                "name" => filter.IsDescending ? query.OrderByDescending(o => o.Name) : query.OrderBy(o => o.Name),
                "rating" => filter.IsDescending ? query.OrderByDescending(o => o.Rating) : query.OrderBy(o => o.Rating),
                "events" => filter.IsDescending ? query.OrderByDescending(o => o.TotalEvents) : query.OrderBy(o => o.TotalEvents),
                _ => query.OrderBy(o => o.Name)
            };

            // Apply pagination
            var organizations = query
                .Skip((filter.PageNumber.GetValueOrDefault(1) - 1) * filter.PageSize.GetValueOrDefault(10))
                .Take(filter.PageSize.GetValueOrDefault(10))
                .ToList();

            return _mapper.Map<IEnumerable<OrganizationListDto>>(organizations);
        }

        public async Task<IEnumerable<OrganizationListDto>> GetFeaturedOrganizationsAsync()
        {
            var organizations = await _organizationRepo.GetFeaturedOrganizationsAsync();
            return _mapper.Map<IEnumerable<OrganizationListDto>>(organizations);
        }

        public async Task<IEnumerable<OrganizationListDto>> GetActivePartnersAsync()
        {
            var organizations = await _organizationRepo.GetActivePartnersAsync();
            return _mapper.Map<IEnumerable<OrganizationListDto>>(organizations);
        }

        public async Task<OrganizationDto> UpdateOrganizationAsync(int id, UpdateOrganizationDto dto, string updatedBy)
        {
            var organization = await _organizationRepo.GetByIdAsync(id);
            if (organization == null)
                throw new KeyNotFoundException($"Organization with ID {id} not found");

            // Check name uniqueness if changed
            if (dto.Name != organization.Name && !await _organizationRepo.IsNameUniqueAsync(dto.Name, id))
                throw new InvalidOperationException($"Organization name '{dto.Name}' already exists");

            _mapper.Map(dto, organization);
            organization.UpdatedBy = updatedBy;
            organization.UpdatedAt = DateTime.UtcNow;
            organization.Location = $"{dto.Address}, {dto.District}, {dto.City}, {dto.Country}";

            _organizationRepo.Update(organization);
            await _unitOfWork.SaveChangesAsync();

            // Log activity
            await _activityRepo.LogActivityAsync(id, ActivityType.Updated, $"Organization information was updated");

            return _mapper.Map<OrganizationDto>(organization);
        }

        public async Task<bool> DeleteOrganizationAsync(int id, string deletedBy)
        {
            var organization = await _organizationRepo.GetByIdAsync(id);
            if (organization == null)
                throw new KeyNotFoundException($"Organization with ID {id} not found");

            organization.IsDeleted = true;
            organization.DeletedAt = DateTime.UtcNow;
            organization.UpdatedBy = deletedBy;

            _organizationRepo.Update(organization);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateOrganizationMediaAsync(UpdateOrganizationMediaDto dto)
        {
            var organization = await _organizationRepo.GetByIdAsync(dto.OrganizationId);
            if (organization == null)
                throw new KeyNotFoundException($"Organization with ID {dto.OrganizationId} not found");

            if (dto.MediaType.ToLower() == "logo")
                organization.LogoUrl = dto.MediaUrl;
            else if (dto.MediaType.ToLower() == "cover")
                organization.CoverImageUrl = dto.MediaUrl;
            else
                throw new ArgumentException($"Invalid media type: {dto.MediaType}");

            organization.UpdatedAt = DateTime.UtcNow;

            _organizationRepo.Update(organization);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<OrganizationMemberDto>> GetOrganizationMembersAsync(int organizationId)
        {
            var members = await _memberRepo.GetMembersByOrganizationAsync(organizationId);
            return _mapper.Map<IEnumerable<OrganizationMemberDto>>(members);
        }

        public async Task<bool> RemoveMemberAsync(int organizationId, int memberId, string removedBy, int removedByUserId)
        {
            var member = await _memberRepo.GetMemberWithDetailsAsync(memberId);
            if (member == null || member.OrganizationId != organizationId)
                throw new KeyNotFoundException($"Member not found in organization");

            // Check permission
            if (!await _memberRepo.HasRoleInOrganizationAsync(removedByUserId, organizationId, OrganizationRole.Admin))
                throw new UnauthorizedAccessException("Only admins can remove members");

            member.IsDeleted = true;
            member.DeletedAt = DateTime.UtcNow;
            member.UpdatedBy = removedBy;

            _memberRepo.Update(member);
            await _unitOfWork.SaveChangesAsync();

            // Log activity
            await _activityRepo.LogActivityAsync(organizationId, ActivityType.MemberRemoved,
                $"User '{member.User?.FullName}' was removed from organization", removedByUserId);

            return true;
        }

        public async Task<IEnumerable<OrganizationListDto>> GetUserOrganizationsAsync(int userId)
        {
            var memberships = await _memberRepo.GetOrganizationsForUserAsync(userId);
            var organizations = memberships.Select(m => m.Organization).ToList();
            return _mapper.Map<IEnumerable<OrganizationListDto>>(organizations);
        }

        // Partnership Methods
        public async Task<PartnershipApplicationDto> CreatePartnershipApplicationAsync(CreatePartnershipApplicationDto dto, string createdBy)
        {
            // Check for pending application
            if (await _applicationRepo.HasPendingApplicationAsync(dto.OrganizationId))
                throw new InvalidOperationException("Organization already has a pending application");

            var application = _mapper.Map<PartnershipApplication>(dto);
            application.ApplicationDate = DateTime.UtcNow;
            application.Status = ApplicationStatus.Pending;
            application.CreatedBy = createdBy;
            application.CreatedAt = DateTime.UtcNow;

            var created = await _applicationRepo.AddAsync(application);
            await _unitOfWork.SaveChangesAsync();

            // Log activity
            await _activityRepo.LogActivityAsync(dto.OrganizationId, ActivityType.ProposalSubmitted,
                $"Partnership application for {dto.RequestedTier} tier submitted");

            return _mapper.Map<PartnershipApplicationDto>(created);
        }

        public async Task<IEnumerable<PartnershipApplicationDto>> GetPendingApplicationsAsync()
        {
            var applications = await _applicationRepo.GetPendingApplicationsAsync();
            return _mapper.Map<IEnumerable<PartnershipApplicationDto>>(applications);
        }

        public async Task<IEnumerable<PartnershipApplicationDto>> GetOrganizationApplicationsAsync(int organizationId)
        {
            var applications = await _applicationRepo.GetApplicationsByOrganizationAsync(organizationId);
            return _mapper.Map<IEnumerable<PartnershipApplicationDto>>(applications);
        }

        public async Task<bool> ReviewPartnershipApplicationAsync(ReviewPartnershipApplicationDto dto, int reviewerId, string reviewedBy)
        {
            var application = await _applicationRepo.GetApplicationWithDetailsAsync(dto.ApplicationId);
            if (application == null)
                throw new KeyNotFoundException($"Application with ID {dto.ApplicationId} not found");

            application.Status = Enum.Parse<ApplicationStatus>(dto.Status);
            application.ReviewNotes = dto.ReviewNotes;
            application.ReviewedByUserId = reviewerId;
            application.ReviewDate = DateTime.UtcNow;
            application.UpdatedBy = reviewedBy;
            application.UpdatedAt = DateTime.UtcNow;

            _applicationRepo.Update(application);

            // Update organization if approved
            if (application.Status == ApplicationStatus.Approved)
            {
                var organization = await _organizationRepo.GetByIdAsync(application.OrganizationId);
                organization.PartnershipTier = application.RequestedTier;
                organization.Status = PartnershipStatus.Active;
                organization.LicenseActiveUntil = DateTime.UtcNow.AddMonths(12);
                organization.UpdatedAt = DateTime.UtcNow;

                _organizationRepo.Update(organization);

                await _activityRepo.LogActivityAsync(application.OrganizationId, ActivityType.PartnershipApproved,
                    $"Partnership application approved for {application.RequestedTier} tier");
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpgradePartnershipTierAsync(int organizationId, string tier, string upgradedBy)
        {
            var organization = await _organizationRepo.GetByIdAsync(organizationId);
            if (organization == null)
                throw new KeyNotFoundException($"Organization with ID {organizationId} not found");

            var oldTier = organization.PartnershipTier;
            organization.PartnershipTier = Enum.Parse<PartnershipTier>(tier);
            organization.UpdatedBy = upgradedBy;
            organization.UpdatedAt = DateTime.UtcNow;

            _organizationRepo.Update(organization);
            await _unitOfWork.SaveChangesAsync();

            // Log activity
            await _activityRepo.LogActivityAsync(organizationId, ActivityType.PartnershipUpgraded,
                $"Partnership tier upgraded from {oldTier} to {tier}");

            return true;
        }

        public async Task<bool> RenewPartnershipAsync(int organizationId, int months, string renewedBy)
        {
            var organization = await _organizationRepo.GetByIdAsync(organizationId);
            if (organization == null)
                throw new KeyNotFoundException($"Organization with ID {organizationId} not found");

            if (organization.LicenseActiveUntil == null || organization.LicenseActiveUntil < DateTime.UtcNow)
                organization.LicenseActiveUntil = DateTime.UtcNow;

            organization.LicenseActiveUntil = organization.LicenseActiveUntil.Value.AddMonths(months);
            organization.Status = PartnershipStatus.Active;
            organization.UpdatedBy = renewedBy;
            organization.UpdatedAt = DateTime.UtcNow;

            _organizationRepo.Update(organization);
            await _unitOfWork.SaveChangesAsync();

            // Log activity
            await _activityRepo.LogActivityAsync(organizationId, ActivityType.LicenseRenewed,
                $"Partnership license renewed for {months} months");

            return true;
        }

        // Activity Methods
        public async Task<IEnumerable<OrganizationActivityDto>> GetOrganizationActivitiesAsync(int organizationId, int take = 10)
        {
            var activities = await _activityRepo.GetActivitiesByOrganizationAsync(organizationId, take);
            return _mapper.Map<IEnumerable<OrganizationActivityDto>>(activities);
        }

        public async Task LogActivityAsync(int organizationId, string activityType, string description, int? userId = null)
        {
            var type = Enum.Parse<ActivityType>(activityType);
            await _activityRepo.LogActivityAsync(organizationId, type, description, userId);
        }

    }
}