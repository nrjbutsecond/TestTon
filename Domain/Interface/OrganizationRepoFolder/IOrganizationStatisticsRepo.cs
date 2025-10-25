using Domain.Entities.Organize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.OrganizationRepoFolder
{
    public interface IOrganizationStatisticsRepo :IRepo<OrganizationStatistics>
    {
        Task<OrganizationStatistics> GetStatisticsByPeriodAsync(int organizationId, int year, int month);
        Task<IEnumerable<OrganizationStatistics>> GetYearlyStatisticsAsync(int organizationId, int year);
        Task UpdateOrCreateStatisticsAsync(int organizationId, int year, int month);
        Task<OrganizationStatistics> CalculateCurrentStatisticsAsync(int organizationId);

    }
}
