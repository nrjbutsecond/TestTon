using Application.DTOs.ConsultationRequest;
using Application.Helper;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.ServicePlan;
using Domain.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class ConsultationRequestService : IConsultationRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ConsultationRequestService> _logger;

        public ConsultationRequestService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ConsultationRequestService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ConsultationRequestDto> CreateConsultationRequestAsync(int organizerId, CreateConsultationRequestDto dto)
        {
            // Validate service plan exists
            var servicePlan = await _unitOfWork.ServicePlan.GetByIdAsync(dto.ServicePlanId);
            if (servicePlan == null)
            {
                throw new NotFoundException("Service plan not found");
            }

            // Check if service plan includes consultation
            if (!servicePlan.IncludesConsultation)
            {
                throw new BadRequestException("This service plan does not include consultation services");
            }

            // Validate user has active subscription for this plan
            var hasActiveSubscription = await _unitOfWork.UserServicePlanSubscription
                .HasActiveSubscriptionAsync(organizerId);

            if (!hasActiveSubscription)
            {
                throw new BadRequestException("You don't have an active subscription for this service plan");
            }

            // Check if preferred date is in the future
            if (dto.PreferredDate <= DateTime.UtcNow)
            {
                throw new BadRequestException("Preferred date must be in the future");
            }

            // Check for duplicate active request
            var hasActiveRequest = await _unitOfWork.ConsultationRequests
                .HasActiveConsultationAsync(organizerId, dto.ServicePlanId);

            if (hasActiveRequest)
            {
                throw new BadRequestException("You already have an active consultation request for this service plan");
            }

            var consultationRequest = _mapper.Map<ConsultationRequest>(dto);
            consultationRequest.OrganizerId = organizerId;
            consultationRequest.Status = ConsultationStatus.Pending;
            consultationRequest.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.ConsultationRequests.AddAsync(consultationRequest);
            await _unitOfWork.SaveChangesAsync();

            var result = await GetConsultationRequestByIdAsync(consultationRequest.Id, organizerId, "Organizer");

            _logger.LogInformation("Consultation request {Id} created by organizer {OrganizerId}", consultationRequest.Id, organizerId);

            return result;
        }

        public async Task<ConsultationRequestDto> GetConsultationRequestByIdAsync(int id, int userId, string userRole)
        {
            var consultation = await _unitOfWork.ConsultationRequests.GetByIdWithDetailsAsync(id);

            if (consultation == null)
            {
                throw new NotFoundException("Consultation request not found");
            }

            // Check permissions
            if (userRole == "Organizer" && consultation.OrganizerId != userId)
            {
                throw new ForbiddenException("You don't have permission to view this consultation request");
            }

            if (userRole == "CommunityStaff" && consultation.AssignedStaffId != userId)
            {
                throw new ForbiddenException("You don't have permission to view this consultation request");
            }

            var dto = _mapper.Map<ConsultationRequestDto>(consultation);
            dto.CanEdit = userRole == "Organizer" && consultation.Status == ConsultationStatus.Pending;
            dto.CanCancel = (userRole == "Organizer" || userRole == "Admin") &&
                           (consultation.Status == ConsultationStatus.Pending || consultation.Status == ConsultationStatus.Scheduled);

            return dto;
        }

        public async Task<IEnumerable<ConsultationRequestListDto>> GetMyConsultationRequestsAsync(int organizerId)
        {
            var consultations = await _unitOfWork.ConsultationRequests.GetByOrganizerIdAsync(organizerId);
            return _mapper.Map<IEnumerable<ConsultationRequestListDto>>(consultations);
        }

        public async Task<IEnumerable<ConsultationRequestListDto>> GetAllConsultationRequestsAsync(string? status = null)
        {
            IEnumerable<ConsultationRequest> consultations;

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ConsultationStatus>(status, true, out var consultationStatus))
            {
                consultations = await _unitOfWork.ConsultationRequests.GetByStatusAsync(consultationStatus);
            }
            else
            {
                consultations = await _unitOfWork.ConsultationRequests.GetAllAsync();
            }

            return _mapper.Map<IEnumerable<ConsultationRequestListDto>>(consultations);
        }

        public async Task<IEnumerable<ConsultationRequestListDto>> GetAssignedConsultationsAsync(int staffId)
        {
            var consultations = await _unitOfWork.ConsultationRequests.GetByAssignedStaffIdAsync(staffId);
            return _mapper.Map<IEnumerable<ConsultationRequestListDto>>(consultations);
        }

        public async Task<ConsultationRequestDto> UpdateConsultationRequestAsync(int id, int organizerId, UpdateConsultationRequestDto dto)
        {
            var consultation = await _unitOfWork.ConsultationRequests.GetByIdAsync(id);

            if (consultation == null)
            {
                throw new NotFoundException("Consultation request not found");
            }

            if (consultation.OrganizerId != organizerId)
            {
                throw new ForbiddenException("You don't have permission to update this consultation request");
            }

            if (consultation.Status != ConsultationStatus.Pending)
            {
                throw new BadRequestException("Only pending consultation requests can be updated");
            }

            if (dto.PreferredDate <= DateTime.UtcNow)
            {
                throw new BadRequestException("Preferred date must be in the future");
            }

            consultation.Description = dto.Description;
            consultation.PreferredDate = dto.PreferredDate;
            consultation.Duration = dto.Duration;
            consultation.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ConsultationRequests.Update(consultation);
            await _unitOfWork.SaveChangesAsync();

            var result = await GetConsultationRequestByIdAsync(id, organizerId, "Organizer");

            _logger.LogInformation("Consultation request {Id} updated by organizer {OrganizerId}", id, organizerId);

            return result;
        }

        public async Task<ConsultationRequestDto> ScheduleConsultationAsync(int id, ScheduleConsultationDto dto)
        {
            var consultation = await _unitOfWork.ConsultationRequests.GetByIdAsync(id);

            if (consultation == null)
            {
                throw new NotFoundException("Consultation request not found");
            }

            if (consultation.Status != ConsultationStatus.Pending)
            {
                throw new BadRequestException("Only pending consultation requests can be scheduled");
            }

            // Validate staff exists and has appropriate role
            var staff = await _unitOfWork.Users.GetByIdAsync(dto.AssignedStaffId);
            if (staff == null)
            {
                throw new NotFoundException("Staff member not found");
            }

            if (staff.Role != UserRoles.CommunityStaff && staff.Role != UserRoles.Admin)
            {
                throw new BadRequestException("Selected user is not a staff member");
            }

            if (dto.ScheduledDate <= DateTime.UtcNow)
            {
                throw new BadRequestException("Scheduled date must be in the future");
            }

            consultation.AssignedStaffId = dto.AssignedStaffId;
            consultation.ScheduledDate = dto.ScheduledDate;
            consultation.Notes = dto.Notes;
            consultation.Status = ConsultationStatus.Scheduled;
            consultation.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ConsultationRequests.Update(consultation);
            await _unitOfWork.SaveChangesAsync();

            var result = await GetConsultationRequestByIdAsync(id, consultation.OrganizerId, "Admin");

            _logger.LogInformation("Consultation request {Id} scheduled with staff {StaffId}", id, dto.AssignedStaffId);

            return result;
        }

        public async Task<ConsultationRequestDto> UpdateConsultationStatusAsync(int id, UpdateConsultationStatusDto dto)
        {
            var consultation = await _unitOfWork.ConsultationRequests.GetByIdAsync(id);

            if (consultation == null)
            {
                throw new NotFoundException("Consultation request not found");
            }

            if (!Enum.TryParse<ConsultationStatus>(dto.Status, true, out var newStatus))
            {
                throw new BadRequestException("Invalid status value");
            }

            // Validate status transitions
            var validTransitions = new Dictionary<ConsultationStatus, ConsultationStatus[]>
            {
                { ConsultationStatus.Pending, new[] { ConsultationStatus.Scheduled, ConsultationStatus.Cancelled } },
                { ConsultationStatus.Scheduled, new[] { ConsultationStatus.InProgress, ConsultationStatus.Cancelled } },
                { ConsultationStatus.InProgress, new[] { ConsultationStatus.Completed, ConsultationStatus.Cancelled } },
                { ConsultationStatus.Completed, Array.Empty<ConsultationStatus>() },
                { ConsultationStatus.Cancelled, Array.Empty<ConsultationStatus>() }
            };

            if (!validTransitions[consultation.Status].Contains(newStatus))
            {
                throw new BadRequestException($"Cannot change status from {consultation.Status} to {newStatus}");
            }

            consultation.Status = newStatus;
            consultation.Notes = dto.Notes ?? consultation.Notes;
            consultation.UpdatedAt = DateTime.UtcNow;

            if (newStatus == ConsultationStatus.Completed && dto.CompletedDate.HasValue)
            {
                consultation.CompletedDate = dto.CompletedDate.Value;
            }
            else if (newStatus == ConsultationStatus.Completed && !consultation.CompletedDate.HasValue)
            {
                consultation.CompletedDate = DateTime.UtcNow;
            }

            _unitOfWork.ConsultationRequests.Update(consultation);
            await _unitOfWork.SaveChangesAsync();

            var result = await GetConsultationRequestByIdAsync(id, consultation.OrganizerId, "Admin");

            _logger.LogInformation("Consultation request {Id} status updated to {Status}", id, newStatus);

            return result;
        }

        public async Task<bool> CancelConsultationRequestAsync(int id, int userId, string userRole)
        {
            var consultation = await _unitOfWork.ConsultationRequests.GetByIdAsync(id);

            if (consultation == null)
            {
                throw new NotFoundException("Consultation request not found");
            }

            // Check permissions
            if (userRole == "Organizer" && consultation.OrganizerId != userId)
            {
                throw new ForbiddenException("You don't have permission to cancel this consultation request");
            }

            if (consultation.Status != ConsultationStatus.Pending && consultation.Status != ConsultationStatus.Scheduled)
            {
                throw new BadRequestException("Only pending or scheduled consultation requests can be cancelled");
            }

            consultation.Status = ConsultationStatus.Cancelled;
            consultation.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ConsultationRequests.Update(consultation);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Consultation request {Id} cancelled by user {UserId}", id, userId);

            return true;
        }

        public async Task<ConsultationStatsDto> GetConsultationStatsAsync()
        {
            var allConsultations = await _unitOfWork.ConsultationRequests.GetAllAsync();

            var stats = new ConsultationStatsDto
            {
                TotalRequests = allConsultations.Count(),
                PendingRequests = allConsultations.Count(c => c.Status == ConsultationStatus.Pending),
                ScheduledRequests = allConsultations.Count(c => c.Status == ConsultationStatus.Scheduled),
                InProgressRequests = allConsultations.Count(c => c.Status == ConsultationStatus.InProgress),
                CompletedRequests = allConsultations.Count(c => c.Status == ConsultationStatus.Completed),
                CancelledRequests = allConsultations.Count(c => c.Status == ConsultationStatus.Cancelled),
                AverageDuration = allConsultations.Any() ? allConsultations.Average(c => c.Duration) : 0,
                RequestsByType = await _unitOfWork.ConsultationRequests.GetRequestCountByTypeAsync()
            };

            return stats;
        }
    }
}