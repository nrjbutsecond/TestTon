using Domain.Entities.Organize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.OrganizationRepoFolder
{
    public interface IOrganizationMemberRepo : IRepo<OrganizationMember>
    {
        Task<IEnumerable<OrganizationMember>> GetMembersByOrganizationAsync(int organizationId);
        Task<OrganizationMember> GetMemberWithDetailsAsync(int memberId);
        Task<OrganizationMember> GetMemberByUserAndOrgAsync(int userId, int organizationId);

        // User organizations
        Task<IEnumerable<OrganizationMember>> GetOrganizationsForUserAsync(int userId);

        // Authorization checks
        Task<bool> IsMemberOfOrganizationAsync(int userId, int organizationId);
        Task<bool> HasRoleInOrganizationAsync(int userId, int organizationId, OrganizationRole role);
        Task<bool> CanUserManageOrganizationContentAsync(int userId, int organizationId);
        Task<OrganizationMember> GetOrganizerMemberAsync(int userId, int organizationId);

        // Statistics
        Task<int> GetMemberCountByRoleAsync(int organizationId, OrganizationRole role);
    }
}
