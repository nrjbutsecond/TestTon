using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IWorkshopRepo : IRepo<WorkshopModel>
    {
        Task<IEnumerable<WorkshopModel>> GetUpcomingWorkshopsAsync(int pageNumber, int pageSize);
        Task<IEnumerable<WorkshopModel>> GetOfficialWorkshopsAsync();
        Task<IEnumerable<WorkshopModel>> GetWorkshopsByOrganizerAsync(int organizerId);
        Task<IEnumerable<WorkshopModel>> GetAvailableWorkshopsAsync(); // Not full
        Task<bool> IsUserRegisteredAsync(int workshopId, int userId);
        Task<int> GetRegisteredCountAsync(int workshopId);
        Task UpdateParticipantCountAsync(int workshopId, int count);
    }
}
