using Domain.Entities.Organize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.OrganizationRepoFolder
{
    public interface IOrganizationActivityRepo :IRepo<OrganizationActivity>
    {
        Task<IEnumerable<OrganizationActivity>> GetActivitiesByOrganizationAsync(int organizationId, int take = 10);
        Task<IEnumerable<OrganizationActivity>> GetActivitiesByDateRangeAsync(int organizationId, DateTime startDate, DateTime endDate);
        Task LogActivityAsync(int organizationId, ActivityType type, string description, int? userId = null, string details = null);

        // Activity helpers for related entities
        Task LogEventActivityAsync(int eventId, int userId, ActivityType type, string description);
        Task LogMerchandiseActivityAsync(int merchandiseId, int userId, ActivityType type, string description);
    }
}
