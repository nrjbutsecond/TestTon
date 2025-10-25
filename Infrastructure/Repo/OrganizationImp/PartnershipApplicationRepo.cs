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
    public class PartnershipApplicationRepo :Repo<PartnershipApplication>, IPartnershipApplicationRepo
    {
        public PartnershipApplicationRepo(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<PartnershipApplication>> GetPendingApplicationsAsync()
        {
            return await _context.PartnershipApplications
                .Include(a => a.Organization)
                .Where(a => a.Status == ApplicationStatus.Pending && !a.IsDeleted)
                .OrderBy(a => a.ApplicationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PartnershipApplication>> GetApplicationsByOrganizationAsync(int organizationId)
        {
            return await _context.PartnershipApplications
                .Include(a => a.ReviewedBy)
                .Where(a => a.OrganizationId == organizationId && !a.IsDeleted)
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();
        }

        public async Task<PartnershipApplication> GetApplicationWithDetailsAsync(int applicationId)
        {
            return await _context.PartnershipApplications
                .Include(a => a.Organization)
                .Include(a => a.ReviewedBy)
                .FirstOrDefaultAsync(a => a.Id == applicationId && !a.IsDeleted);
        }

        public async Task<bool> HasPendingApplicationAsync(int organizationId)
        {
            return await _context.PartnershipApplications
                .AnyAsync(a => a.OrganizationId == organizationId
                    && (a.Status == ApplicationStatus.Pending || a.Status == ApplicationStatus.UnderReview)
                    && !a.IsDeleted);
        }

    }
}
