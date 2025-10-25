using Domain.Entities.ServicePlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.ServicePlan
{
    public interface IConsultationRequestRepo : IRepo<ConsultationRequest>
    {
        Task<IEnumerable<ConsultationRequest>> GetByOrganizerIdAsync(int organizerId, bool includeDeleted = false);
        Task<IEnumerable<ConsultationRequest>> GetByStatusAsync(ConsultationStatus status, bool includeDeleted = false);
        Task<IEnumerable<ConsultationRequest>> GetByAssignedStaffIdAsync(int staffId, bool includeDeleted = false);
        Task<IEnumerable<ConsultationRequest>> GetPendingRequestsAsync();
        Task<IEnumerable<ConsultationRequest>> GetUpcomingScheduledAsync(DateTime fromDate);
        Task<ConsultationRequest?> GetByIdWithDetailsAsync(int id);
        Task<bool> HasActiveConsultationAsync(int organizerId, int servicePlanId);
        Task<Dictionary<string, int>> GetRequestCountByTypeAsync();
        Task<int> GetPendingCountAsync();
    }
}
