using Application.DTOs.Activity;
using Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IWorkshopService
    {
        Task<PagedResult<WorkshopListDto>> GetUpcomingWorkshopsAsync(int pageNumber, int pageSize);
        Task<WorkshopDto?> GetWorkshopByIdAsync(int id);
        Task<IEnumerable<WorkshopListDto>> GetOfficialWorkshopsAsync();
        Task<IEnumerable<WorkshopListDto>> GetWorkshopsByOrganizerAsync(int organizerId);
        Task<IEnumerable<WorkshopListDto>> GetAvailableWorkshopsAsync();
        Task<WorkshopDto> CreateWorkshopAsync(CreateWorkshopDto dto, int organizerId);
        Task<WorkshopDto?> UpdateWorkshopAsync(UpdateWorkshopDto dto, int organizerId);
        Task<bool> DeleteWorkshopAsync(int id, int organizerId);
        Task<bool> PublishWorkshopAsync(int id, int organizerId);
        Task<bool> CancelWorkshopAsync(int id, string reason, int organizerId);
        Task<bool> IsUserRegisteredAsync(int workshopId, int userId);
        Task<int> GetRegisteredCountAsync(int workshopId);
    }
}
