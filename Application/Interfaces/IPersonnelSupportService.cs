using Application.DTOs.SupportPersonnel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPersonnelSupportService
    {
        // ============== Support Personnel Management ==============
        Task<SupportPersonnelDto> GetSupportPersonnelByIdAsync(int id);
        Task<IEnumerable<SupportPersonnelListDto>> GetAllSupportPersonnelAsync(bool? isActive = null);
        Task<IEnumerable<SupportPersonnelListDto>> GetPersonnelByOrganizerAsync(int organizerId);
        Task<SupportPersonnelDto> CreateSupportPersonnelAsync(CreateSupportPersonnelDto dto, int registeredBy);
        Task<SupportPersonnelDto> UpdateSupportPersonnelAsync(int id, UpdateSupportPersonnelDto dto);
        Task<bool> DeleteSupportPersonnelAsync(int id);
        Task<bool> TogglePersonnelStatusAsync(int id);

        // ============== Personnel Support Request Management ==============
        Task<PersonnelSupportRequestDto> GetRequestByIdAsync(int id);
        Task<IEnumerable<PersonnelSupportRequestListDto>> GetAllRequestsAsync(string? status = null);
        Task<IEnumerable<PersonnelSupportRequestListDto>> GetRequestsByOrganizerAsync(int organizerId);
        Task<IEnumerable<PersonnelSupportRequestListDto>> GetRequestsByEventAsync(int eventId);
        Task<PersonnelSupportRequestDto> CreateRequestAsync(CreatePersonnelSupportRequestDto dto, int organizerId);
        Task<PersonnelSupportRequestDto> UpdateRequestAsync(int id, UpdatePersonnelSupportRequestDto dto, int userId);
        Task<bool> DeleteRequestAsync(int id, int userId);

        // ============== Request Approval & Assignment (Admin/Community Staff) ==============
        Task<PersonnelSupportRequestDto> ApproveRequestAsync(int requestId, int approvedBy, string? notes = null);
        Task<PersonnelSupportRequestDto> RejectRequestAsync(int requestId, int rejectedBy, string reason);
        Task<PersonnelSupportRequestDto> AssignPersonnelToRequestAsync(AssignPersonnelDto dto, int assignedBy);
        Task<bool> RemovePersonnelFromRequestAsync(int requestId, int personnelId);
        Task<bool> UpdateAssignmentStatusAsync(UpdateAssignmentStatusDto dto);

        // ============== Status Management ==============
        Task<bool> MarkRequestInProgressAsync(int requestId);
        Task<bool> CompleteRequestAsync(int requestId, string? notes = null);
        Task<bool> CancelRequestAsync(int requestId, string reason);

        // ============== Statistics & Reports ==============
        Task<Dictionary<string, int>> GetRequestStatisticsAsync();
        Task<IEnumerable<SupportPersonnelListDto>> GetAvailablePersonnelAsync(DateTime startDate, DateTime endDate);
    }
}