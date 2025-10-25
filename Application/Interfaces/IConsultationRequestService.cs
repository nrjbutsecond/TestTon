using Application.DTOs.ConsultationRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IConsultationRequestService
    {

        Task<ConsultationRequestDto> CreateConsultationRequestAsync(int organizerId, CreateConsultationRequestDto dto);
        Task<ConsultationRequestDto> GetConsultationRequestByIdAsync(int id, int userId, string userRole);
        Task<IEnumerable<ConsultationRequestListDto>> GetMyConsultationRequestsAsync(int organizerId);
        Task<IEnumerable<ConsultationRequestListDto>> GetAllConsultationRequestsAsync(string? status = null);
        Task<IEnumerable<ConsultationRequestListDto>> GetAssignedConsultationsAsync(int staffId);
        Task<ConsultationRequestDto> UpdateConsultationRequestAsync(int id, int organizerId, UpdateConsultationRequestDto dto);
        Task<ConsultationRequestDto> ScheduleConsultationAsync(int id, ScheduleConsultationDto dto);
        Task<ConsultationRequestDto> UpdateConsultationStatusAsync(int id, UpdateConsultationStatusDto dto);
        Task<bool> CancelConsultationRequestAsync(int id, int userId, string userRole);
        Task<ConsultationStatsDto> GetConsultationStatsAsync();
    }
}
