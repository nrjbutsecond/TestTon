using Domain.Entities;
using Domain.Entities.MerchandiseEntity;
using Domain.Entities.Organize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.OrganizationRepoFolder
{
    public interface IOrganizationRepo : IRepo<OrganizationModel>
    {

        Task<OrganizationModel> GetOrganizationWithDetailsAsync(int id);
        Task<OrganizationModel> GetOrganizationWithMembersAsync(int id);

        // Search and filter
        Task<IEnumerable<OrganizationModel>> GetActivePartnersAsync();
        Task<IEnumerable<OrganizationModel>> GetFeaturedOrganizationsAsync();

        // Validation
        Task<bool> IsNameUniqueAsync(string name, int? excludeId = null);

        // Statistics
        Task UpdateStatisticsAsync(int organizationId);
        Task<int> GetOrganizationEventCountAsync(int organizationId);
        Task<decimal> GetOrganizationRevenueAsync(int organizationId, DateTime startDate, DateTime endDate);

        // Relationships through OrganizationMember
        Task<IEnumerable<TalkEventModel>> GetOrganizationEventsAsync(int organizationId);
        Task<IEnumerable<Merchandise>> GetOrganizationMerchandiseAsync(int organizationId);
        Task<bool> HasActiveEventsAsync(int organizationId);
        Task<bool> HasActiveMerchandiseAsync(int organizationId);
    }
}
