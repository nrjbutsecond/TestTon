using Application.DTOs.SupportPersonnel;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class PersonnelSupportService : IPersonnelSupportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PersonnelSupportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Support Personnel Management

        public async Task<SupportPersonnelDto> GetSupportPersonnelByIdAsync(int id)
        {
            var personnel = await _unitOfWork.SupportPersonnel.GetByIdWithDetailsAsync(id);

            if (personnel == null)
                throw new KeyNotFoundException("Support personnel not found");

            return _mapper.Map<SupportPersonnelDto>(personnel);
        }

        public async Task<IEnumerable<SupportPersonnelListDto>> GetAllSupportPersonnelAsync(bool? isActive = null)
        {
            var personnel = await _unitOfWork.SupportPersonnel.GetAllWithDetailsAsync(isActive);
            return _mapper.Map<IEnumerable<SupportPersonnelListDto>>(personnel);
        }

        public async Task<IEnumerable<SupportPersonnelListDto>> GetPersonnelByOrganizerAsync(int organizerId)
        {
            var personnel = await _unitOfWork.SupportPersonnel.GetByOrganizerAsync(organizerId);
            return _mapper.Map<IEnumerable<SupportPersonnelListDto>>(personnel);
        }

        public async Task<SupportPersonnelDto> CreateSupportPersonnelAsync(CreateSupportPersonnelDto dto, int registeredBy)
        {
            // Check if user exists
            var userExists = await _unitOfWork.Users.ExistsAsync(u => u.Id == dto.UserId && !u.IsDeleted);
            if (!userExists)
                throw new KeyNotFoundException("User not found");

            // Check if personnel already exists for this user
            var exists = await _unitOfWork.SupportPersonnel.ExistsByUserIdAsync(dto.UserId);
            if (exists)
                throw new InvalidOperationException("This user is already registered as support personnel");

            var personnel = _mapper.Map<SupportPersonnel>(dto);
            personnel.RegisteredBy = registeredBy;
            personnel.CreatedBy = registeredBy.ToString();
            personnel.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.SupportPersonnel.AddAsync(personnel);
            await _unitOfWork.SaveChangesAsync();

            return await GetSupportPersonnelByIdAsync(personnel.Id);
        }

        public async Task<SupportPersonnelDto> UpdateSupportPersonnelAsync(int id, UpdateSupportPersonnelDto dto)
        {
            var personnel = await _unitOfWork.SupportPersonnel.GetByIdAsync(id);
            if (personnel == null || personnel.IsDeleted)
                throw new KeyNotFoundException("Support personnel not found");

            _mapper.Map(dto, personnel);
            personnel.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.SupportPersonnel.Update(personnel);
            await _unitOfWork.SaveChangesAsync();

            return await GetSupportPersonnelByIdAsync(id);
        }

        public async Task<bool> DeleteSupportPersonnelAsync(int id)
        {
            var personnel = await _unitOfWork.SupportPersonnel.GetByIdAsync(id);
            if (personnel == null || personnel.IsDeleted)
                return false;

            // Check for active assignments
            var hasActiveAssignments = await _unitOfWork.SupportPersonnel.HasActiveAssignmentsAsync(id);
            if (hasActiveAssignments)
                throw new InvalidOperationException("Cannot delete personnel with active assignments");

            personnel.IsDeleted = true;
            personnel.DeletedAt = DateTime.UtcNow;

            _unitOfWork.SupportPersonnel.Update(personnel);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> TogglePersonnelStatusAsync(int id)
        {
            var personnel = await _unitOfWork.SupportPersonnel.GetByIdAsync(id);
            if (personnel == null || personnel.IsDeleted)
                return false;

            personnel.IsActive = !personnel.IsActive;
            personnel.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.SupportPersonnel.Update(personnel);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        #endregion

        #region Personnel Support Request Management

        public async Task<PersonnelSupportRequestDto> GetRequestByIdAsync(int id)
        {
            var request = await _unitOfWork.PersonnelSupportRequest.GetByIdWithDetailsAsync(id);

            if (request == null)
                throw new KeyNotFoundException("Personnel support request not found");

            return _mapper.Map<PersonnelSupportRequestDto>(request);
        }

        public async Task<IEnumerable<PersonnelSupportRequestListDto>> GetAllRequestsAsync(string? status = null)
        {
            var requests = await _unitOfWork.PersonnelSupportRequest.GetAllWithDetailsAsync(status);
            return _mapper.Map<IEnumerable<PersonnelSupportRequestListDto>>(requests);
        }

        public async Task<IEnumerable<PersonnelSupportRequestListDto>> GetRequestsByOrganizerAsync(int organizerId)
        {
            var requests = await _unitOfWork.PersonnelSupportRequest.GetByOrganizerAsync(organizerId);
            return _mapper.Map<IEnumerable<PersonnelSupportRequestListDto>>(requests);
        }

        public async Task<IEnumerable<PersonnelSupportRequestListDto>> GetRequestsByEventAsync(int eventId)
        {
            var requests = await _unitOfWork.PersonnelSupportRequest.GetByEventAsync(eventId);
            return _mapper.Map<IEnumerable<PersonnelSupportRequestListDto>>(requests);
        }

        public async Task<PersonnelSupportRequestDto> CreateRequestAsync(CreatePersonnelSupportRequestDto dto, int organizerId)
        {
            // Validate event exists and belongs to organizer
            var eventExists = await _unitOfWork.TalkEvent.ExistsAsync(
                e => e.Id == dto.EventId && e.OrganizerId == organizerId && !e.IsDeleted);

            if (!eventExists)
                throw new KeyNotFoundException("Event not found or you don't have permission");

            // Check if organizer has eligible service plan
            var user = await _unitOfWork.Users.GetByIdAsync(organizerId);
            if (user == null || user.Role != UserRoles.Organizer)
                throw new InvalidOperationException("Only organizers can create personnel support requests");

            // Validate dates
            if (dto.StartDate >= dto.EndDate)
                throw new InvalidOperationException("Start date must be before end date");

            if (dto.StartDate < DateTime.UtcNow)
                throw new InvalidOperationException("Start date cannot be in the past");

            var request = _mapper.Map<PersonnelSupportRequest>(dto);
            request.OrganizerId = organizerId;
            request.CreatedBy = organizerId.ToString();
            request.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.PersonnelSupportRequest.AddAsync(request);
            await _unitOfWork.SaveChangesAsync();

            return await GetRequestByIdAsync(request.Id);
        }

        public async Task<PersonnelSupportRequestDto> UpdateRequestAsync(int id, UpdatePersonnelSupportRequestDto dto, int userId)
        {
            var request = await _unitOfWork.PersonnelSupportRequest.GetByIdAsync(id);
            if (request == null || request.IsDeleted)
                throw new KeyNotFoundException("Request not found");

            // Only allow updates if status is Pending or Cancelled
            if (request.Status != "Pending" && request.Status != "Cancelled")
                throw new InvalidOperationException("Cannot update request in current status");

            // Verify organizer owns the request
            if (request.OrganizerId != userId)
                throw new UnauthorizedAccessException("You don't have permission to update this request");

            _mapper.Map(dto, request);
            request.UpdatedAt = DateTime.UtcNow;
            request.UpdatedBy = userId.ToString();

            _unitOfWork.PersonnelSupportRequest.Update(request);
            await _unitOfWork.SaveChangesAsync();

            return await GetRequestByIdAsync(id);
        }

        public async Task<bool> DeleteRequestAsync(int id, int userId)
        {
            var request = await _unitOfWork.PersonnelSupportRequest.GetByIdAsync(id);
            if (request == null || request.IsDeleted)
                return false;

            // Only allow deletion if status is Pending or Cancelled
            if (request.Status != "Pending" && request.Status != "Cancelled")
                throw new InvalidOperationException("Cannot delete request in current status");

            // Verify organizer owns the request
            if (request.OrganizerId != userId)
                throw new UnauthorizedAccessException("You don't have permission to delete this request");

            request.IsDeleted = true;
            request.DeletedAt = DateTime.UtcNow;

            _unitOfWork.PersonnelSupportRequest.Update(request);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        #endregion

        #region Request Approval & Assignment

        public async Task<PersonnelSupportRequestDto> ApproveRequestAsync(int requestId, int approvedBy, string? notes = null)
        {
            var request = await _unitOfWork.PersonnelSupportRequest.GetByIdAsync(requestId);
            if (request == null || request.IsDeleted)
                throw new KeyNotFoundException("Request not found");

            if (request.Status != "Pending")
                throw new InvalidOperationException("Only pending requests can be approved");

            request.Status = "Approved";
            request.ApprovedBy = approvedBy;
            request.FulfillmentNotes = notes;
            request.UpdatedAt = DateTime.UtcNow;
            request.UpdatedBy = approvedBy.ToString();

            _unitOfWork.PersonnelSupportRequest.Update(request);
            await _unitOfWork.SaveChangesAsync();

            return await GetRequestByIdAsync(requestId);
        }

        public async Task<PersonnelSupportRequestDto> RejectRequestAsync(int requestId, int rejectedBy, string reason)
        {
            var request = await _unitOfWork.PersonnelSupportRequest.GetByIdAsync(requestId);
            if (request == null || request.IsDeleted)
                throw new KeyNotFoundException("Request not found");

            if (request.Status != "Pending")
                throw new InvalidOperationException("Only pending requests can be rejected");

            request.Status = "Cancelled";
            request.ApprovedBy = rejectedBy;
            request.FulfillmentNotes = $"Rejected: {reason}";
            request.UpdatedAt = DateTime.UtcNow;
            request.UpdatedBy = rejectedBy.ToString();

            _unitOfWork.PersonnelSupportRequest.Update(request);
            await _unitOfWork.SaveChangesAsync();

            return await GetRequestByIdAsync(requestId);
        }

        public async Task<PersonnelSupportRequestDto> AssignPersonnelToRequestAsync(AssignPersonnelDto dto, int assignedBy)
        {
            var request = await _unitOfWork.PersonnelSupportRequest.GetByIdWithDetailsAsync(dto.RequestId);
            if (request == null)
                throw new KeyNotFoundException("Request not found");

            if (request.Status != "Approved")
                throw new InvalidOperationException("Can only assign personnel to approved requests");

            foreach (var personnelId in dto.PersonnelIds)
            {
                // Check if personnel exists and is active
                var personnel = await _unitOfWork.SupportPersonnel.GetByIdAsync(personnelId);
                if (personnel == null || !personnel.IsActive || personnel.IsDeleted)
                    continue;

                // Check if already assigned
                var alreadyAssigned = await _unitOfWork.PersonnelSupportAssignment
                    .IsPersonnelAssignedAsync(dto.RequestId, personnelId);

                if (alreadyAssigned)
                    continue;

                var assignment = new PersonnelSupportAssignment
                {
                    PersonnelSupportRequestId = dto.RequestId,
                    SupportPersonnelId = personnelId,
                    AssignedDate = DateTime.UtcNow,
                    Status = "Assigned",
                    Notes = dto.Notes,
                    CreatedBy = assignedBy.ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.PersonnelSupportAssignment.AddAsync(assignment);
            }

            // Update request status to Assigned
            request.Status = "Assigned";
            request.UpdatedAt = DateTime.UtcNow;
            request.UpdatedBy = assignedBy.ToString();
            _unitOfWork.PersonnelSupportRequest.Update(request);

            await _unitOfWork.SaveChangesAsync();

            return await GetRequestByIdAsync(dto.RequestId);
        }

        public async Task<bool> RemovePersonnelFromRequestAsync(int requestId, int personnelId)
        {
            var assignment = await _unitOfWork.PersonnelSupportAssignment
                .GetByRequestAndPersonnelAsync(requestId, personnelId);

            if (assignment == null)
                return false;

            // Can only remove if not in progress or completed
            if (assignment.Status == "InProgress" || assignment.Status == "Completed")
                throw new InvalidOperationException("Cannot remove personnel in progress or completed assignments");

            assignment.IsDeleted = true;
            assignment.DeletedAt = DateTime.UtcNow;

            _unitOfWork.PersonnelSupportAssignment.Update(assignment);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateAssignmentStatusAsync(UpdateAssignmentStatusDto dto)
        {
            var assignment = await _unitOfWork.PersonnelSupportAssignment.GetByIdAsync(dto.AssignmentId);
            if (assignment == null || assignment.IsDeleted)
                return false;

            assignment.Status = dto.Status;
            assignment.Notes = dto.Notes ?? assignment.Notes;
            assignment.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.PersonnelSupportAssignment.Update(assignment);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        #endregion

        #region Status Management

        public async Task<bool> MarkRequestInProgressAsync(int requestId)
        {
            var request = await _unitOfWork.PersonnelSupportRequest.GetByIdAsync(requestId);
            if (request == null || request.IsDeleted)
                return false;

            if (request.Status != "Assigned")
                throw new InvalidOperationException("Can only mark assigned requests as in progress");

            request.Status = "InProgress";
            request.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.PersonnelSupportRequest.Update(request);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CompleteRequestAsync(int requestId, string? notes = null)
        {
            var request = await _unitOfWork.PersonnelSupportRequest.GetByIdAsync(requestId);
            if (request == null || request.IsDeleted)
                return false;

            if (request.Status != "InProgress" && request.Status != "Assigned")
                throw new InvalidOperationException("Can only complete in progress or assigned requests");

            request.Status = "Completed";
            request.FulfillmentNotes = notes ?? request.FulfillmentNotes;
            request.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.PersonnelSupportRequest.Update(request);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CancelRequestAsync(int requestId, string reason)
        {
            var request = await _unitOfWork.PersonnelSupportRequest.GetByIdAsync(requestId);
            if (request == null || request.IsDeleted)
                return false;

            if (request.Status == "Completed")
                throw new InvalidOperationException("Cannot cancel completed requests");

            request.Status = "Cancelled";
            request.FulfillmentNotes = $"Cancelled: {reason}";
            request.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.PersonnelSupportRequest.Update(request);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        #endregion

        #region Statistics & Reports

        public async Task<Dictionary<string, int>> GetRequestStatisticsAsync()
        {
            return await _unitOfWork.PersonnelSupportRequest.GetStatisticsAsync();
        }

        public async Task<IEnumerable<SupportPersonnelListDto>> GetAvailablePersonnelAsync(DateTime startDate, DateTime endDate)
        {
            var availablePersonnel = await _unitOfWork.SupportPersonnel
                .GetAvailablePersonnelAsync(startDate, endDate);

            return _mapper.Map<IEnumerable<SupportPersonnelListDto>>(availablePersonnel);
        }

        #endregion
    }
}