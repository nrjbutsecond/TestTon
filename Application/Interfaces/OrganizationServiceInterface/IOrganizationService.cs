using Application.DTOs.Merchandise;
using Application.DTOs.Organization;
using Domain.Entities.Organize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.OrganizationServiceInterface
{
    public interface IOrganizationService
    {
        // Organization CRUD
        Task<OrganizationDto> GetOrganizationByIdAsync(int id);
        Task<IEnumerable<OrganizationListDto>> GetOrganizationsAsync(OrganizationFilterDto filter);
        Task<IEnumerable<OrganizationListDto>> GetFeaturedOrganizationsAsync();
        Task<IEnumerable<OrganizationListDto>> GetActivePartnersAsync();
        Task<OrganizationDto> CreateOrganizationAsync(CreateOrganizationDto dto, string createdBy, int creatorUserId);
        Task<OrganizationDto> UpdateOrganizationAsync(int id, UpdateOrganizationDto dto, string updatedBy);
        Task<bool> DeleteOrganizationAsync(int id, string deletedBy);

        // Dashboard
        Task<OrganizationDashboardDto> GetOrganizationDashboardAsync(int id);

        // Media
        Task<bool> UpdateOrganizationMediaAsync(UpdateOrganizationMediaDto dto);

        // Statistics
        Task UpdateOrganizationStatisticsAsync(int organizationId);
        Task<IEnumerable<OrganizationStatisticsDto>> GetOrganizationStatisticsAsync(int organizationId, int year);

        // Member Management
        Task<IEnumerable<OrganizationMemberDto>> GetOrganizationMembersAsync(int organizationId);
        Task<OrganizationMemberDto> AddMemberAsync(int organizationId, AddOrganizationMemberDto dto, string addedBy, int addedByUserId);
        Task<bool> UpdateMemberRoleAsync(UpdateMemberRoleDto dto, string updatedBy, int updatedByUserId);
        Task<bool> RemoveMemberAsync(int organizationId, int memberId, string removedBy, int removedByUserId);

        // Authorization checks
        Task<bool> IsMemberOfOrganizationAsync(int userId, int organizationId);
        Task<bool> CanUserCreateContentForOrganization(int userId, int organizationId);
        Task<bool> IsUserOrganizerInOrganization(int userId, int organizationId);
        Task<OrganizationMember> GetOrganizerMemberAsync(int userId, int organizationId);

        // User organizations
        Task<IEnumerable<OrganizationListDto>> GetUserOrganizationsAsync(int userId);

        // Partnership
        Task<PartnershipApplicationDto> CreatePartnershipApplicationAsync(CreatePartnershipApplicationDto dto, string createdBy);
        Task<IEnumerable<PartnershipApplicationDto>> GetPendingApplicationsAsync();
        Task<IEnumerable<PartnershipApplicationDto>> GetOrganizationApplicationsAsync(int organizationId);
        Task<bool> ReviewPartnershipApplicationAsync(ReviewPartnershipApplicationDto dto, int reviewerId, string reviewedBy);
        Task<bool> UpgradePartnershipTierAsync(int organizationId, string tier, string upgradedBy);
        Task<bool> RenewPartnershipAsync(int organizationId, int months, string renewedBy);

        // Activity Logs
        Task<IEnumerable<OrganizationActivityDto>> GetOrganizationActivitiesAsync(int organizationId, int take = 10);
        Task LogActivityAsync(int organizationId, string activityType, string description, int? userId = null);

        // Related content (through member relationships)
        Task<IEnumerable<TalkEventDto>> GetOrganizationEventsAsync(int organizationId);
        Task<IEnumerable<MerchandiseOrganizationDto>> GetOrganizationMerchandiseAsync(int organizationId);

        // Activity helpers for other services
        Task LogEventCreatedAsync(int userId, int eventId, string eventTitle);
        Task LogMerchandiseAddedAsync(int userId, int merchandiseId, string merchandiseName);
    }
}
