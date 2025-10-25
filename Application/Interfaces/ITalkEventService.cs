using Application.DTOs.Common;
using Application.DTOs.TalkEvent;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITalkEventService
    {
        Task<TalkEventResponseDto?> GetByIdAsync(int id);
        Task<PagedResult<TalkEventListDto>> GetPagedEventsAsync(
            int pageNumber,
            int pageSize,
            TalkEventStatus? status = null,
            string? orderBy = null);
        Task<IEnumerable<TalkEventListDto>> GetUpcomingEventsAsync(int limit = 10);
        Task<IEnumerable<TalkEventListDto>> GetPartneredEventsAsync();
        Task<IEnumerable<TalkEventListDto>> GetOrganizerEventsAsync(int organizerId);
        Task<TalkEventResponseDto> CreateAsync(CreateTalkEventDto dto, int organizerId, string createdBy);
        Task<TalkEventResponseDto?> UpdateAsync(int id, UpdateTalkEventDto dto, string updatedBy);
        Task<bool> UpdateStatusAsync(int id, TalkEventStatus status, string updatedBy, string? reason = null);
        Task<bool> DeleteAsync(int id, string deletedBy);
        Task<bool> CanUserManageEventAsync(int eventId, int userId, UserRoles userRole);
        Task<(bool Success, string? Error)> RestoreAsync(int id, string restoredBy);
        Task<PagedResult<DeletedTalkEventDto>> GetDeletedEventsAsync(
            int pageNumber,
            int pageSize,
            DeletedEventFilterDto? filter = null);
        Task<DeletedTalkEventDto?> GetDeletedEventDetailsAsync(int id);
        Task<RestoreValidationDto> ValidateRestoreAsync(int id);



        Task<bool> CanUserCreateEventAsync(int userId, int organizationId);
        Task ValidateAndLogEventCreationAsync(int userId, int organizationId, string eventTitle);
        Task LogEventStatusChangeAsync(int eventId, int userId, string oldStatus, string newStatus);
    }
}
