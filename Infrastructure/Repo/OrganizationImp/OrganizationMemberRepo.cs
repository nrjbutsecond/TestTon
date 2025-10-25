using Domain.Entities.Organize;
using Domain.Interface.OrganizationRepoFolder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;

namespace Infrastructure.Repo.OrganizationImp
{
    public class OrganizationMemberRepo : Repo<OrganizationMember>, IOrganizationMemberRepo
    {
        public OrganizationMemberRepo(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<OrganizationMember>> GetMembersByOrganizationAsync(int organizationId)
        {
            return await _context.OrganizationMembers
                .Include(m => m.User)
                .Where(m => m.OrganizationId == organizationId && !m.IsDeleted)
                .OrderBy(m => m.Role)
                .ThenBy(m => m.JoinedDate)
                .ToListAsync();
        }

        public async Task<bool> IsMemberOfOrganizationAsync(int userId, int organizationId)
        {
            return await _context.OrganizationMembers
                .AnyAsync(m => m.UserId == userId
                    && m.OrganizationId == organizationId
                    && m.IsActive
                    && !m.IsDeleted);
        }

        public async Task<bool> HasRoleInOrganizationAsync(int userId, int organizationId, OrganizationRole role)
        {
            return await _context.OrganizationMembers
                .AnyAsync(m => m.UserId == userId
                    && m.OrganizationId == organizationId
                    && m.Role == role
                    && m.IsActive
                    && !m.IsDeleted);
        }

        public async Task<bool> CanUserManageOrganizationContentAsync(int userId, int organizationId)
        {
            return await _context.OrganizationMembers
                .AnyAsync(m => m.UserId == userId
                    && m.OrganizationId == organizationId
                    && (m.Role == OrganizationRole.Organizer || m.Role == OrganizationRole.Admin)
                    && m.IsActive
                    && !m.IsDeleted);
        }

        public async Task<OrganizationMember> GetOrganizerMemberAsync(int userId, int organizationId)
        {
            return await _context.OrganizationMembers
                .Include(m => m.Organization)
                .FirstOrDefaultAsync(m => m.UserId == userId
                    && m.OrganizationId == organizationId
                    && (m.Role == OrganizationRole.Organizer || m.Role == OrganizationRole.Admin)
                    && m.IsActive
                    && !m.IsDeleted);
        }

        public async Task<OrganizationMember> GetMemberByUserAndOrgAsync(int userId, int organizationId)
        {
            return await _context.OrganizationMembers
                .Include(m => m.User)
                .Include(m => m.Organization)
                .FirstOrDefaultAsync(m => m.UserId == userId
                    && m.OrganizationId == organizationId
                    && !m.IsDeleted);
        }

        public async Task<IEnumerable<OrganizationMember>> GetOrganizationsForUserAsync(int userId)
        {
            return await _context.OrganizationMembers
                .Include(m => m.Organization)
                .Where(m => m.UserId == userId && m.IsActive && !m.IsDeleted)
                .ToListAsync();
        }

        public async Task<int> GetMemberCountByRoleAsync(int organizationId, OrganizationRole role)
        {
            return await _context.OrganizationMembers
                .CountAsync(m => m.OrganizationId == organizationId
                    && m.Role == role
                    && m.IsActive
                    && !m.IsDeleted);
        }
        public async Task<OrganizationMember> GetMemberWithDetailsAsync(int memberId)
        {
            return await _context.OrganizationMembers
                .Include(m => m.User)
                .Include(m => m.Organization)
                .FirstOrDefaultAsync(m => m.Id == memberId && !m.IsDeleted);
        }
    }
}
