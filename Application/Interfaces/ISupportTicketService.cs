using Application.DTOs.SupportTicket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISupportTicketService
    {
        // Ticket CRUD
        Task<SupportTicketDetailDto?> GetTicketByIdAsync(int id, int? userId = null);
        Task<SupportTicketDetailDto?> GetTicketByNumberAsync(string ticketNumber);
        Task<IEnumerable<SupportTicketListDto>> GetAllTicketsAsync(TicketFilterDto? filter = null);
        Task<IEnumerable<SupportTicketListDto>> GetMyTicketsAsync(int customerId);
        Task<IEnumerable<SupportTicketListDto>> GetAssignedTicketsAsync(int assigneeId);
        Task<SupportTicketDetailDto> CreateTicketAsync(CreateSupportTicketDto dto, int customerId);
        Task<SupportTicketDetailDto> UpdateTicketAsync(int id, UpdateSupportTicketDto dto);
        Task<bool> DeleteTicketAsync(int id);

        // Ticket assignment
        Task<bool> AssignTicketAsync(int ticketId, int assigneeId);
        Task<bool> UnassignTicketAsync(int ticketId);

        // Ticket status
        Task<bool> UpdateTicketStatusAsync(int ticketId, string status);
        Task<bool> CloseTicketAsync(int ticketId);
        Task<bool> ReopenTicketAsync(int ticketId);
        Task<bool> EscalateTicketAsync(int ticketId);

        // Messages
        Task<TicketMessageDto> AddMessageAsync(int ticketId, AddTicketMessageDto dto, int senderId);
        Task<IEnumerable<TicketMessageDto>> GetTicketMessagesAsync(int ticketId);

        // Satisfaction
        Task<bool> RateTicketAsync(int ticketId, RateTicketDto dto, int customerId);

        // Statistics
        Task<TicketStatisticsDto> GetStatisticsAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<TeamPerformanceDto>> GetTeamPerformanceAsync();

        // Search
        Task<IEnumerable<SupportTicketListDto>> SearchTicketsAsync(string searchTerm);
    }
}
