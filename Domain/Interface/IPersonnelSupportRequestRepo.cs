using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IPersonnelSupportRequestRepo : IRepo<PersonnelSupportRequest>
    {
        Task<PersonnelSupportRequest?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<PersonnelSupportRequest>> GetAllWithDetailsAsync(string? status = null);
        Task<IEnumerable<PersonnelSupportRequest>> GetByOrganizerAsync(int organizerId);
        Task<IEnumerable<PersonnelSupportRequest>> GetByEventAsync(int eventId);
        Task<Dictionary<string, int>> GetStatisticsAsync();
    }
}
