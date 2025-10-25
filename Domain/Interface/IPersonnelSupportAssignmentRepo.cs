using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IPersonnelSupportAssignmentRepo : IRepo<PersonnelSupportAssignment>
    {
        Task<PersonnelSupportAssignment?> GetByRequestAndPersonnelAsync(int requestId, int personnelId);
        Task<IEnumerable<PersonnelSupportAssignment>> GetByRequestIdAsync(int requestId);
        Task<IEnumerable<PersonnelSupportAssignment>> GetByPersonnelIdAsync(int personnelId);
        Task<bool> IsPersonnelAssignedAsync(int requestId, int personnelId);
        Task<int> GetAssignmentCountByRequestAsync(int requestId);
    }
}
