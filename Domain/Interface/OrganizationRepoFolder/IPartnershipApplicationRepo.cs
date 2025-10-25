using Domain.Entities.Organize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.OrganizationRepoFolder
{
    public interface IPartnershipApplicationRepo : IRepo<PartnershipApplication>
    {
        Task<IEnumerable<PartnershipApplication>> GetPendingApplicationsAsync();
        Task<IEnumerable<PartnershipApplication>> GetApplicationsByOrganizationAsync(int organizationId);
        Task<PartnershipApplication> GetApplicationWithDetailsAsync(int applicationId);
        Task<bool> HasPendingApplicationAsync(int organizationId);
    }
}
