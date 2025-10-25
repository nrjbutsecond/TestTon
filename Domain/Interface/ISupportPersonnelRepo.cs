using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface ISupportPersonnelRepo : IRepo<SupportPersonnel>
    {
        Task<SupportPersonnel?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<SupportPersonnel>> GetAllWithDetailsAsync(bool? isActive = null);
        Task<IEnumerable<SupportPersonnel>> GetByOrganizerAsync(int organizerId);
        Task<IEnumerable<SupportPersonnel>> GetAvailablePersonnelAsync(DateTime startDate, DateTime endDate);
        Task<bool> ExistsByUserIdAsync(int userId);
        Task<bool> HasActiveAssignmentsAsync(int personnelId);
    }
}
