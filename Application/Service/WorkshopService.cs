using Application.DTOs.Activity;
using Application.DTOs.Common;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Helper;
namespace Application.Service
{
    public class WorkshopService : IWorkshopService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<WorkshopService> _logger;


        public WorkshopService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<WorkshopService> logger)
       
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
         
        }

        public async Task<PagedResult<WorkshopListDto>> GetUpcomingWorkshopsAsync(int pageNumber, int pageSize)
        {
            var workshops = await _unitOfWork.Workshops.GetUpcomingWorkshopsAsync(pageNumber, pageSize);
            var totalCount = await _unitOfWork.Workshops.CountAsync(
                w => w.StartDateTime > DateTime.UtcNow && w.Status == WorkshopStatus.Published); // FIXED: Upcoming → Published

            return new PagedResult<WorkshopListDto>
            {
                Items = _mapper.Map<IEnumerable<WorkshopListDto>>(workshops).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<WorkshopDto?> GetWorkshopByIdAsync(int id)
        {
            var workshop = await _unitOfWork.Workshops.GetByIdAsync(id);
            if (workshop == null) return null;

            var dto = _mapper.Map<WorkshopDto>(workshop);

            // Load organizer details if not official
            if (!workshop.IsOfficial && workshop.OrganizerId.HasValue)
            {
                var organizer = await _unitOfWork.Users.GetByIdAsync(workshop.OrganizerId.Value);
                dto.Organizer = _mapper.Map<OrganizerDto>(organizer);
            }

            return dto;
        }

        public async Task<IEnumerable<WorkshopListDto>> GetOfficialWorkshopsAsync()
        {
            var workshops = await _unitOfWork.Workshops.GetOfficialWorkshopsAsync();
            return _mapper.Map<IEnumerable<WorkshopListDto>>(workshops);
        }

        public async Task<IEnumerable<WorkshopListDto>> GetWorkshopsByOrganizerAsync(int organizerId)
        {
            var workshops = await _unitOfWork.Workshops.GetWorkshopsByOrganizerAsync(organizerId);
            return _mapper.Map<IEnumerable<WorkshopListDto>>(workshops);
        }

        public async Task<IEnumerable<WorkshopListDto>> GetAvailableWorkshopsAsync()
        {
            var workshops = await _unitOfWork.Workshops.GetAvailableWorkshopsAsync();
            return _mapper.Map<IEnumerable<WorkshopListDto>>(workshops);
        }

        public async Task<WorkshopDto> CreateWorkshopAsync(CreateWorkshopDto dto, int organizerId)
        {
            // Validate dates
            if (dto.StartDateTime <= DateTime.UtcNow)
            {
                throw new BadRequestException("Workshop start date must be in the future");
            }

            if (dto.EndDateTime <= dto.StartDateTime)
            {
                throw new BadRequestException("Workshop end date must be after start date");
            }

            if (dto.RegistrationDeadline >= dto.StartDateTime)
            {
                throw new BadRequestException("Registration deadline must be before workshop start date");
            }

            var workshop = _mapper.Map<WorkshopModel>(dto);
            workshop.OrganizerId = organizerId;
            workshop.IsOfficial = true; // CHANGED: Admin-created workshops are official
            workshop.Status = WorkshopStatus.Draft;
            workshop.CurrentParticipants = 0;
            workshop.CreatedBy = organizerId.ToString(); // Set audit field

            await _unitOfWork.Workshops.AddAsync(workshop);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Workshop created with ID: {WorkshopId} by organizer: {OrganizerId}",
                workshop.Id, organizerId);

            return _mapper.Map<WorkshopDto>(workshop);
        }

        public async Task<WorkshopDto?> UpdateWorkshopAsync(UpdateWorkshopDto dto, int organizerId)
        {
            var workshop = await _unitOfWork.Workshops.GetByIdAsync(dto.Id);
            if (workshop == null) return null;

            // Admin can update any workshop
            // Remove ownership check since only Admin can access this

            // Don't allow changes if workshop has started
            if (workshop.StartDateTime <= DateTime.UtcNow && workshop.Status != WorkshopStatus.Draft)
            {
                throw new BadRequestException("Cannot update a workshop that has already started");
            }

            _mapper.Map(dto, workshop);
            workshop.UpdatedBy = organizerId.ToString(); // Set audit field
            workshop.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Workshops.Update(workshop);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<WorkshopDto>(workshop);
        }

        public async Task<bool> DeleteWorkshopAsync(int id, int organizerId)
        {
            var workshop = await _unitOfWork.Workshops.GetByIdAsync(id);
            if (workshop == null) return false;

            // Admin can delete any workshop
            // Remove ownership check since only Admin can access this

            // Check if workshop has registrations
            var hasRegistrations = await _unitOfWork.Workshops.GetRegisteredCountAsync(id) > 0;
            if (hasRegistrations)
            {
                throw new BadRequestException("Cannot delete workshop with existing registrations");
            }

            _unitOfWork.Workshops.Remove(workshop);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Workshop deleted with ID: {WorkshopId}", id);
            return true;
        }

        public async Task<bool> PublishWorkshopAsync(int id, int organizerId)
        {
            var workshop = await _unitOfWork.Workshops.GetByIdAsync(id);
            if (workshop == null) return false;

            // Admin can publish any workshop
            // Remove ownership check since only Admin can access this

            workshop.Status = WorkshopStatus.Published; //  Upcoming → Published
            workshop.UpdatedBy = organizerId.ToString();
            workshop.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Workshops.Update(workshop);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CancelWorkshopAsync(int id, string reason, int organizerId)
        {
            var workshop = await _unitOfWork.Workshops.GetByIdAsync(id);
            if (workshop == null) return false;

            // Admin can cancel any workshop
            // Remove ownership check since only Admin can access this

            workshop.Status = WorkshopStatus.Cancelled;
            workshop.CancellationReason = reason;
            workshop.UpdatedBy = organizerId.ToString();
            workshop.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Workshops.Update(workshop);

            // Cancel all tickets
       /*     var tickets = await _unitOfWork.Tickets.GetWorkshopTicketsAsync(id); //ticket haven't done
            foreach (var ticket in tickets)
            {
                ticket.Status = TicketStatus.Cancelled;
                _unitOfWork.Tickets.Update(ticket);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Workshop cancelled with ID: {WorkshopId}, Reason: {Reason}", id, reason);
       */
            return true;
        }

        public async Task<bool> IsUserRegisteredAsync(int workshopId, int userId)
        {
            // FIXED: Convert int userId to string for repository interface
            return await _unitOfWork.Workshops.IsUserRegisteredAsync(workshopId, userId);
        }

        public async Task<int> GetRegisteredCountAsync(int workshopId)
        {
            return await _unitOfWork.Workshops.GetRegisteredCountAsync(workshopId);
        }
    }
}