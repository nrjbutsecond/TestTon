using Application.DTOs.SupportTicket;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Service
{
    public class SupportTicketService : ISupportTicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISupportTicketRepo _ticketRepo;
        private readonly ISupportTicketMessageRepo _messageRepo;

        public SupportTicketService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ISupportTicketRepo ticketRepo,
            ISupportTicketMessageRepo messageRepo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _ticketRepo = ticketRepo;
            _messageRepo = messageRepo;
        }

        public async Task<SupportTicketDetailDto?> GetTicketByIdAsync(int id, int? userId = null)
        {
            var ticket = await _ticketRepo.GetTicketWithDetailsAsync(id);
            if (ticket == null) return null;

            // Check permission: customer can only see their own tickets
            if (userId.HasValue && ticket.CustomerId != userId.Value)
            {
                // Check if user is staff (simplified - should check role properly)
                var user = await _unitOfWork.Users.GetByIdAsync(userId.Value);
                if (user == null || user.Role == UserRoles.Enthusiast || user.Role == UserRoles.Organizer)
                    return null;
            }

            return _mapper.Map<SupportTicketDetailDto>(ticket);
        }

        public async Task<SupportTicketDetailDto?> GetTicketByNumberAsync(string ticketNumber)
        {
            var ticket = await _ticketRepo.GetTicketByNumberAsync(ticketNumber);
            return ticket == null ? null : _mapper.Map<SupportTicketDetailDto>(ticket);
        }

        public async Task<IEnumerable<SupportTicketListDto>> GetAllTicketsAsync(TicketFilterDto? filter = null)
        {
            var query = _ticketRepo.GetQueryable().Where(t => !t.IsDeleted);

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.Status) && Enum.TryParse<SupportTicketStatus>(filter.Status, out var status))
                    query = query.Where(t => t.Status == status);

                if (!string.IsNullOrEmpty(filter.Priority) && Enum.TryParse<TicketPriority>(filter.Priority, out var priority))
                    query = query.Where(t => t.Priority == priority);

                if (!string.IsNullOrEmpty(filter.Category) && Enum.TryParse<TicketCategory>(filter.Category, out var category))
                    query = query.Where(t => t.Category == category);

                if (filter.AssignedToId.HasValue)
                    query = query.Where(t => t.AssignedToId == filter.AssignedToId.Value);

                if (filter.CustomerId.HasValue)
                    query = query.Where(t => t.CustomerId == filter.CustomerId.Value);

                if (filter.FromDate.HasValue)
                    query = query.Where(t => t.CreatedAt >= filter.FromDate.Value);

                if (filter.ToDate.HasValue)
                    query = query.Where(t => t.CreatedAt <= filter.ToDate.Value);

                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    query = query.Where(t => t.TicketNumber.Contains(filter.SearchTerm) ||
                                            t.Subject.Contains(filter.SearchTerm) ||
                                            t.Description.Contains(filter.SearchTerm));
                }
            }

            var tickets = await Task.Run(() => query
                .OrderByDescending(t => t.Priority)
                .ThenByDescending(t => t.CreatedAt)
                .Skip((filter?.PageNumber ?? 1 - 1) * (filter?.PageSize ?? 20))
                .Take(filter?.PageSize ?? 20)
                .ToList());

            return _mapper.Map<IEnumerable<SupportTicketListDto>>(tickets);
        }

        public async Task<IEnumerable<SupportTicketListDto>> GetMyTicketsAsync(int customerId)
        {
            var tickets = await _ticketRepo.GetTicketsByCustomerAsync(customerId);
            return _mapper.Map<IEnumerable<SupportTicketListDto>>(tickets);
        }

        public async Task<IEnumerable<SupportTicketListDto>> GetAssignedTicketsAsync(int assigneeId)
        {
            var tickets = await _ticketRepo.GetTicketsByAssigneeAsync(assigneeId);
            return _mapper.Map<IEnumerable<SupportTicketListDto>>(tickets);
        }

        public async Task<SupportTicketDetailDto> CreateTicketAsync(CreateSupportTicketDto dto, int customerId)
        {
            var ticket = new SupportTicket
            {
                TicketNumber = GenerateTicketNumber(),
                Subject = dto.Subject,
                Description = dto.Description,
                CustomerId = customerId,
                Category = Enum.Parse<TicketCategory>(dto.Category),
                Priority = string.IsNullOrEmpty(dto.Priority)
                    ? TicketPriority.Medium
                    : Enum.Parse<TicketPriority>(dto.Priority),
                Status = SupportTicketStatus.Open,
                Tags = dto.Tags != null ? string.Join(",", dto.Tags) : null,
                RelatedOrderId = dto.RelatedOrderId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _ticketRepo.AddAsync(ticket);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SupportTicketDetailDto>(created);
        }

        public async Task<SupportTicketDetailDto> UpdateTicketAsync(int id, UpdateSupportTicketDto dto)
        {
            var ticket = await _ticketRepo.GetByIdAsync(id);
            if (ticket == null)
                throw new KeyNotFoundException($"Ticket with ID {id} not found");

            if (!string.IsNullOrEmpty(dto.Subject))
                ticket.Subject = dto.Subject;

            if (!string.IsNullOrEmpty(dto.Priority))
                ticket.Priority = Enum.Parse<TicketPriority>(dto.Priority);

            if (!string.IsNullOrEmpty(dto.Status))
            {
                var newStatus = Enum.Parse<SupportTicketStatus>(dto.Status);
                if (newStatus == SupportTicketStatus.Resolved && ticket.Status != SupportTicketStatus.Resolved)
                    ticket.ResolvedAt = DateTime.UtcNow;
                ticket.Status = newStatus;
            }

            if (!string.IsNullOrEmpty(dto.Category))
                ticket.Category = Enum.Parse<TicketCategory>(dto.Category);

            if (dto.Tags != null)
                ticket.Tags = string.Join(",", dto.Tags);

            if (dto.AssignedToId.HasValue)
                ticket.AssignedToId = dto.AssignedToId.Value;

            if (!string.IsNullOrEmpty(dto.InternalNotes))
                ticket.InternalNotes = dto.InternalNotes;

            ticket.UpdatedAt = DateTime.UtcNow;
            _ticketRepo.Update(ticket);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SupportTicketDetailDto>(ticket);
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            var ticket = await _ticketRepo.GetByIdAsync(id);
            if (ticket == null) return false;

            ticket.IsDeleted = true;
            ticket.DeletedAt = DateTime.UtcNow;
            _ticketRepo.Update(ticket);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignTicketAsync(int ticketId, int assigneeId)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId);
            if (ticket == null) return false;

            ticket.AssignedToId = assigneeId;
            if (ticket.Status == SupportTicketStatus.Open)
                ticket.Status = SupportTicketStatus.InProgress;

            ticket.UpdatedAt = DateTime.UtcNow;
            _ticketRepo.Update(ticket);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnassignTicketAsync(int ticketId)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId);
            if (ticket == null) return false;

            ticket.AssignedToId = null;
            ticket.Status = SupportTicketStatus.Open;
            ticket.UpdatedAt = DateTime.UtcNow;
            _ticketRepo.Update(ticket);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTicketStatusAsync(int ticketId, string status)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId);
            if (ticket == null) return false;

            var newStatus = Enum.Parse<SupportTicketStatus>(status);

            if (newStatus == SupportTicketStatus.Resolved && ticket.Status != SupportTicketStatus.Resolved)
                ticket.ResolvedAt = DateTime.UtcNow;

            ticket.Status = newStatus;
            ticket.UpdatedAt = DateTime.UtcNow;
            _ticketRepo.Update(ticket);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CloseTicketAsync(int ticketId)
        {
            return await UpdateTicketStatusAsync(ticketId, SupportTicketStatus.Closed.ToString());
        }

        public async Task<bool> ReopenTicketAsync(int ticketId)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId);
            if (ticket == null) return false;

            ticket.Status = SupportTicketStatus.Open;
            ticket.ResolvedAt = null;
            ticket.UpdatedAt = DateTime.UtcNow;
            _ticketRepo.Update(ticket);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EscalateTicketAsync(int ticketId)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId);
            if (ticket == null) return false;

            ticket.Status = SupportTicketStatus.Escalated;
            ticket.Priority = TicketPriority.Urgent;
            ticket.UpdatedAt = DateTime.UtcNow;
            _ticketRepo.Update(ticket);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<TicketMessageDto> AddMessageAsync(int ticketId, AddTicketMessageDto dto, int senderId)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new KeyNotFoundException($"Ticket with ID {ticketId} not found");

            var message = new SupportTicketMessage
            {
                SupportTicketId = ticketId,
                SenderId = senderId,
                Content = dto.Content,
                Attachments = dto.Attachments != null ? JsonSerializer.Serialize(dto.Attachments) : null,
                IsInternal = dto.IsInternal,
                IsCustomerMessage = ticket.CustomerId == senderId,
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _messageRepo.AddAsync(message);

            // Update ticket metrics
            ticket.MessageCount++;
            ticket.LastReplyAt = DateTime.UtcNow;

            if (!ticket.FirstResponseAt.HasValue && !message.IsCustomerMessage)
                ticket.FirstResponseAt = DateTime.UtcNow;

            _ticketRepo.Update(ticket);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TicketMessageDto>(created);
        }

        public async Task<IEnumerable<TicketMessageDto>> GetTicketMessagesAsync(int ticketId)
        {
            var messages = await _messageRepo.GetMessagesByTicketAsync(ticketId);
            return _mapper.Map<IEnumerable<TicketMessageDto>>(messages);
        }

        public async Task<bool> RateTicketAsync(int ticketId, RateTicketDto dto, int customerId)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId);
            if (ticket == null || ticket.CustomerId != customerId) return false;

            ticket.SatisfactionRating = dto.Rating;
            ticket.SatisfactionComment = dto.Comment;
            ticket.UpdatedAt = DateTime.UtcNow;
            _ticketRepo.Update(ticket);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<TicketStatisticsDto> GetStatisticsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var statusCounts = await _ticketRepo.GetTicketCountByStatusAsync();
            var avgResponseTime = await _ticketRepo.GetAverageResponseTimeAsync(fromDate, toDate);
            var avgResolutionTime = await _ticketRepo.GetAverageResolutionTimeAsync(fromDate, toDate);
            var satisfactionScore = await _ticketRepo.GetSatisfactionScoreAsync(fromDate, toDate);

            var totalTickets = statusCounts.Values.Sum();
            var resolvedTickets = statusCounts.GetValueOrDefault(SupportTicketStatus.Resolved, 0) +
                                 statusCounts.GetValueOrDefault(SupportTicketStatus.Closed, 0);

            return new TicketStatisticsDto
            {
                TotalTickets = totalTickets,
                OpenTickets = statusCounts.GetValueOrDefault(SupportTicketStatus.Open, 0),
                InProgressTickets = statusCounts.GetValueOrDefault(SupportTicketStatus.InProgress, 0),
                ResolvedTickets = resolvedTickets,
                EscalatedTickets = statusCounts.GetValueOrDefault(SupportTicketStatus.Escalated, 0),
                AverageResponseTimeHours = avgResponseTime,
                AverageResolutionTimeHours = avgResolutionTime,
                SatisfactionScore = satisfactionScore,
                ResolutionRate = totalTickets > 0 ? (double)resolvedTickets / totalTickets * 100 : 0
            };
        }

        public async Task<IEnumerable<TeamPerformanceDto>> GetTeamPerformanceAsync()
        {
            var staffMembers = await _unitOfWork.Users.FindAsync(u =>
                u.Role == UserRoles.Admin || u.Role == UserRoles.CommunityStaff|| u.Role == UserRoles.SalesStaff);

            var performances = new List<TeamPerformanceDto>();

            foreach (var staff in staffMembers)
            {
                var assignedTickets = await _ticketRepo.GetTicketsByAssigneeAsync(staff.Id);
                var tickets = assignedTickets.ToList();

                performances.Add(new TeamPerformanceDto
                {
                    UserId = staff.Id,
                    StaffName = staff.FullName,
                    Role = staff.Role.ToString(),
                    AssignedTickets = tickets.Count,
                    ResolvedTickets = tickets.Count(t => t.Status == SupportTicketStatus.Resolved ||
                                                         t.Status == SupportTicketStatus.Closed),
                    SatisfactionScore = tickets.Where(t => t.SatisfactionRating.HasValue)
                                              .Select(t => (double)t.SatisfactionRating!.Value)
                                              .DefaultIfEmpty(0)
                                              .Average()
                });
            }

            return performances;
        }

        public async Task<IEnumerable<SupportTicketListDto>> SearchTicketsAsync(string searchTerm)
        {
            var tickets = await _ticketRepo.SearchTicketsAsync(searchTerm);
            return _mapper.Map<IEnumerable<SupportTicketListDto>>(tickets);
        }

        private string GenerateTicketNumber()
        {
            return $"TIC{DateTime.UtcNow:yyyyMMdd}{new Random().Next(1000, 9999)}";
        }
    }
}