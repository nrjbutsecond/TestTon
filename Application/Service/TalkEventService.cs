using Application.DTOs.Common;
using Application.DTOs.TalkEvent;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Organize;
using Domain.Interface;
using Domain.Interface.OrganizationRepoFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class TalkEventService : ITalkEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrganizationMemberRepo _memberRepo;
        private readonly IOrganizationActivityRepo _activityRepo;
        private readonly IMapper _mapper;

        public TalkEventService(IUnitOfWork unitOfWork, IMapper mapper,
            IOrganizationMemberRepo memberRepo,
            IOrganizationActivityRepo activityRepo)
        {
            _unitOfWork = unitOfWork;
            _memberRepo = memberRepo;
            _activityRepo = activityRepo;
            _mapper = mapper;
        }

        public async Task<TalkEventResponseDto?> GetByIdAsync(int id)
        {
            var talkEvent = await _unitOfWork.TalkEvent.GetEventWithTicketsAsync(id);
            if (talkEvent == null) return null;

            var dto = _mapper.Map<TalkEventResponseDto>(talkEvent);
            dto.CurrentAttendees = talkEvent.Tickets.Count(t => t.Status == TicketStatus.Paid); 
            return dto;
        }

        public async Task<PagedResult<TalkEventListDto>> GetPagedEventsAsync(
            int pageNumber,
            int pageSize,
            TalkEventStatus? status = null,
            string? orderBy = null)
        {
            var filter = status != null
                ? (Expression<Func<TalkEventModel, bool>>)(e => e.Status == status)
                : null;

            var (items, totalCount) = await _unitOfWork.TalkEvent.GetPagedEventsAsync(
                pageNumber, pageSize, filter, orderBy);

            var dtos = items.Select(e => {
                var dto = _mapper.Map<TalkEventListDto>(e);
                dto.CurrentAttendees = e.Tickets.Count(t => t.Status == TicketStatus.Paid); 
                return dto;
            });

            return new PagedResult<TalkEventListDto>
            {
                Items = dtos.ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<IEnumerable<TalkEventListDto>> GetUpcomingEventsAsync(int limit = 10)
        {
            var events = await _unitOfWork.TalkEvent.GetUpcomingEventsAsync(limit);
            return _mapper.Map<IEnumerable<TalkEventListDto>>(events);
        }

        public async Task<IEnumerable<TalkEventListDto>> GetPartneredEventsAsync()
        {
            var events = await _unitOfWork.TalkEvent.GetPartneredEventsAsync();
            return _mapper.Map<IEnumerable<TalkEventListDto>>(events);
        }

        public async Task<IEnumerable<TalkEventListDto>> GetOrganizerEventsAsync(int organizerId)
        {
            var events = await _unitOfWork.TalkEvent.GetEventsByOrganizerAsync(organizerId);
            return _mapper.Map<IEnumerable<TalkEventListDto>>(events);
        }

        public async Task<TalkEventResponseDto> CreateAsync(
            CreateTalkEventDto dto,
            int organizerId,
            string createdBy)
        {
            // Validate dates
            if (dto.StartDate >= dto.EndDate)
                throw new ArgumentException("End date must be after start date");

            if (dto.StartDate <= DateTime.UtcNow)
                throw new ArgumentException("Start date must be in the future");

            var talkEvent = _mapper.Map<TalkEventModel>(dto);
            talkEvent.OrganizerId = organizerId;
            talkEvent.Status = TalkEventStatus.Draft;
            talkEvent.CreatedBy = createdBy;
            talkEvent.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.TalkEvent.AddAsync(talkEvent);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TalkEventResponseDto>(talkEvent);
        }

        public async Task<TalkEventResponseDto?> UpdateAsync(
            int id,
            UpdateTalkEventDto dto,
            string updatedBy)
        {
            var talkEvent = await _unitOfWork.TalkEvent.GetByIdAsync(id);
            if (talkEvent == null) return null;

            // Validate status allows updates
            if (talkEvent.Status == TalkEventStatus.Cancelled ||
                talkEvent.Status == TalkEventStatus.Completed)
                throw new InvalidOperationException($"Cannot update event with status {talkEvent.Status}");

            // Validate dates
            if (dto.StartDate >= dto.EndDate)
                throw new ArgumentException("End date must be after start date");

            // If event has started, don't allow date changes
            if (talkEvent.Status == TalkEventStatus.Ongoing &&
                (dto.StartDate != talkEvent.StartDate || dto.EndDate != talkEvent.EndDate))
                throw new InvalidOperationException("Cannot change dates for ongoing event");

            _mapper.Map(dto, talkEvent);
            talkEvent.UpdatedBy = updatedBy;
            talkEvent.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.TalkEvent.Update(talkEvent);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TalkEventResponseDto>(talkEvent);
        }

        public async Task<bool> UpdateStatusAsync(
            int id,
            TalkEventStatus status,
            string updatedBy,
            string? reason = null)
        {
            var talkEvent = await _unitOfWork.TalkEvent.GetByIdAsync(id);
            if (talkEvent == null) return false;

            // Validate status transition
            if (!IsValidStatusTransition(talkEvent.Status, status))
                throw new InvalidOperationException($"Invalid status transition from {talkEvent.Status} to {status}");

            talkEvent.Status = status;
            talkEvent.UpdatedBy = updatedBy;
            talkEvent.UpdatedAt = DateTime.UtcNow;

            if (status == TalkEventStatus.Cancelled)
                talkEvent.CancellationReason = reason;

            _unitOfWork.TalkEvent.Update(talkEvent);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id, string deletedBy)
        {
            var talkEvent = await _unitOfWork.TalkEvent.GetByIdAsync(id);
            if (talkEvent == null) return false;

            // Only allow deletion of draft events
            if (talkEvent.Status != TalkEventStatus.Draft)
                throw new InvalidOperationException("Can only delete draft events");

            talkEvent.IsDeleted = true;
            talkEvent.DeletedAt = DateTime.UtcNow;
            talkEvent.UpdatedBy = deletedBy;

            _unitOfWork.TalkEvent.Update(talkEvent);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CanUserManageEventAsync(int eventId, int userId, UserRoles userRole)
        {
            if (userRole == UserRoles.Admin) return true;

            var talkEvent = await _unitOfWork.TalkEvent.GetByIdAsync(eventId);
            if (talkEvent == null) return false;

            return talkEvent.OrganizerId == userId && userRole == UserRoles.Organizer;
        }

        private bool IsValidStatusTransition(TalkEventStatus currentStatus, TalkEventStatus newStatus)
        {
            return (currentStatus, newStatus) switch
            {
                (TalkEventStatus.Draft, TalkEventStatus.Published) => true,
                (TalkEventStatus.Published, TalkEventStatus.Ongoing) => true,
                (TalkEventStatus.Published, TalkEventStatus.Cancelled) => true,
                (TalkEventStatus.Ongoing, TalkEventStatus.Completed) => true,
                (TalkEventStatus.Ongoing, TalkEventStatus.Cancelled) => true,
                _ => false
            };
        }

        public async Task<(bool Success, string? Error)> RestoreAsync(int id, string restoredBy)
        {
            var talkEvent = await _unitOfWork.TalkEvent.GetDeletedByIdAsync(id);

            if (talkEvent == null)
                return (false, "Deleted event not found");

            // Validate restoration
            var validationResult = await ValidateRestoreInternal(talkEvent);
            if (!validationResult.CanRestore)
            {
                var errors = string.Join("; ", validationResult.ValidationErrors);
                return (false, errors);
            }

            // Restore the event
            talkEvent.IsDeleted = false;
            talkEvent.DeletedAt = null;
            talkEvent.UpdatedBy = restoredBy;
            talkEvent.UpdatedAt = DateTime.UtcNow;

            // Reset status if needed
            if (talkEvent.Status == TalkEventStatus.Published ||
                talkEvent.Status == TalkEventStatus.Ongoing ||
                talkEvent.Status == TalkEventStatus.Completed)
            {
                talkEvent.Status = TalkEventStatus.Draft;
            }

            _unitOfWork.TalkEvent.Update(talkEvent);
            await _unitOfWork.SaveChangesAsync();

            return (true, null);
        }

        public async Task<PagedResult<DeletedTalkEventDto>> GetDeletedEventsAsync(
            int pageNumber,
            int pageSize,
            DeletedEventFilterDto? filter = null)
        {
            // Build filter expression
            Expression<Func<TalkEventModel, bool>>? filterExpression = null;
            if (filter != null)
            {
                filterExpression = e =>
                    (string.IsNullOrEmpty(filter.SearchTerm) ||
                     e.Title.Contains(filter.SearchTerm) ||
                     e.Description.Contains(filter.SearchTerm)) &&
                    (!filter.DeletedFrom.HasValue || e.DeletedAt >= filter.DeletedFrom) &&
                    (!filter.DeletedTo.HasValue || e.DeletedAt <= filter.DeletedTo) &&
                    (!filter.HasTicketsSold.HasValue ||
                     (filter.HasTicketsSold.Value
                        ? e.Tickets.Any(t => t.Status == TicketStatus.Paid)
                        : !e.Tickets.Any(t => t.Status == TicketStatus.Paid)));
            }

            var deletedEvents = await _unitOfWork.TalkEvent.GetDeletedEventsAsync(
                filterExpression,
                filter?.OrderBy);

            var totalCount = await _unitOfWork.TalkEvent.CountDeletedEventsAsync(filterExpression);

            // Convert to DTOs with validation
            var eventsList = deletedEvents.ToList();
            var pagedEvents = eventsList
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var dtos = new List<DeletedTalkEventDto>();
            foreach (var evt in pagedEvents)
            {
                var validationResult = await ValidateRestoreInternal(evt);
                var dto = _mapper.Map<DeletedTalkEventDto>(evt);

                dto.CanRestore = validationResult.CanRestore;
                dto.RestoreBlockReason = validationResult.ValidationErrors.FirstOrDefault();
                dto.TicketsSold = evt.Tickets.Count(t => t.Status == TicketStatus.Paid);
                dto.TotalRevenue = evt.Tickets
                    .Where(t => t.Status == TicketStatus.Paid)
                    .Sum(t => t.TicketType?.Price ?? 0);

                dtos.Add(dto);
            }

            return new PagedResult<DeletedTalkEventDto>
            {
                Items = dtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<DeletedTalkEventDto?> GetDeletedEventDetailsAsync(int id)
        {
            var talkEvent = await _unitOfWork.TalkEvent.GetDeletedByIdAsync(id);
            if (talkEvent == null) return null;

            var validationResult = await ValidateRestoreInternal(talkEvent);
            var dto = _mapper.Map<DeletedTalkEventDto>(talkEvent);

            dto.CanRestore = validationResult.CanRestore;
            dto.RestoreBlockReason = string.Join("; ", validationResult.ValidationErrors);
            dto.TicketsSold = talkEvent.Tickets.Count(t => t.Status == TicketStatus.Paid);
            dto.TotalRevenue = talkEvent.Tickets
                .Where(t => t.Status == TicketStatus.Paid)
                .Sum(t => t.TicketType?.Price ?? 0);

            return dto;
        }

        public async Task<RestoreValidationDto> ValidateRestoreAsync(int id)
        {
            var talkEvent = await _unitOfWork.TalkEvent.GetDeletedByIdAsync(id);
            if (talkEvent == null)
            {
                return new RestoreValidationDto
                {
                    CanRestore = false,
                    ValidationErrors = new List<string> { "Event not found" }
                };
            }

            var result = await ValidateRestoreInternal(talkEvent);
            result.EventDetails = _mapper.Map<TalkEventResponseDto>(talkEvent);

            return result;
        }

        // Private validation method
        private async Task<RestoreValidationDto> ValidateRestoreInternal(TalkEventModel talkEvent)
        {
            var result = new RestoreValidationDto { CanRestore = true };

            // Rule 1: Cannot restore if event date has passed
            if (talkEvent.StartDate < DateTime.UtcNow)
            {
                result.ValidationErrors.Add("Cannot restore past events");
                result.CanRestore = false;
            }

            // Rule 2: Warn if event has paid tickets (but allow restoration)
            var paidTicketsCount = talkEvent.Tickets.Count(t => t.Status == TicketStatus.Paid);
            if (paidTicketsCount > 0)
            {
                result.ValidationErrors.Add($"Event has {paidTicketsCount} paid tickets. Restoration will reactivate all tickets.");
                // Note: Not setting CanRestore = false here, just warning
            }

            // Rule 3: Check organizer status
            if (talkEvent.Organizer == null || !talkEvent.Organizer.IsActive)
            {
                result.ValidationErrors.Add("Event organizer is no longer active");
                result.CanRestore = false;
            }

            // Rule 4: Check if venue/location conflicts with other events
            var conflictingEvents = await _unitOfWork.TalkEvent.FindAsync(e =>
                !e.IsDeleted &&
                e.Id != talkEvent.Id &&
                e.Location == talkEvent.Location &&
                e.StartDate < talkEvent.EndDate &&
                e.EndDate > talkEvent.StartDate);

            if (conflictingEvents.Any())
            {
                result.ValidationErrors.Add("Location has conflicting bookings for the selected dates");
                result.CanRestore = false;
            }

            return result;
        }


        public async Task<bool> CanUserCreateEventAsync(int userId, int organizationId)
        {
            return await _memberRepo.CanUserManageOrganizationContentAsync(userId, organizationId);
        }

        public async Task ValidateAndLogEventCreationAsync(int userId, int organizationId, string eventTitle)
        {
            // Validate user is organizer
            var member = await _memberRepo.GetOrganizerMemberAsync(userId, organizationId);
            if (member == null)
                throw new UnauthorizedAccessException("User is not an organizer in this organization");

            // Log activity
            await _activityRepo.LogActivityAsync(organizationId, ActivityType.EventCreated,
                $"Event '{eventTitle}' was created by {member.User?.FullName}", userId);
        }

        public async Task LogEventStatusChangeAsync(int eventId, int userId, string oldStatus, string newStatus)
        {
            await _activityRepo.LogEventActivityAsync(eventId, userId, ActivityType.EventUpdated,
                $"Event status changed from {oldStatus} to {newStatus}");
        }
    }
}
