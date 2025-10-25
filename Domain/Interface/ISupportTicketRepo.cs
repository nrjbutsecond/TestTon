using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface ISupportTicketRepo : IRepo<SupportTicket>
    {
        Task<SupportTicket?> GetTicketWithDetailsAsync(int id);
        Task<SupportTicket?> GetTicketByNumberAsync(string ticketNumber);
        Task<IEnumerable<SupportTicket>> GetTicketsByCustomerAsync(int customerId);
        Task<IEnumerable<SupportTicket>> GetTicketsByAssigneeAsync(int assigneeId);
        Task<IEnumerable<SupportTicket>> GetTicketsByStatusAsync(SupportTicketStatus status);
        Task<IEnumerable<SupportTicket>> GetTicketsByPriorityAsync(TicketPriority priority);
        Task<IEnumerable<SupportTicket>> GetOpenTicketsAsync();
        Task<IEnumerable<SupportTicket>> GetUnassignedTicketsAsync();
        Task<int> GetOpenTicketCountAsync();
        Task<double> GetAverageResponseTimeAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<double> GetAverageResolutionTimeAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<double> GetSatisfactionScoreAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<Dictionary<SupportTicketStatus, int>> GetTicketCountByStatusAsync();
        Task<Dictionary<TicketPriority, int>> GetTicketCountByPriorityAsync();
        Task<IEnumerable<SupportTicket>> SearchTicketsAsync(string searchTerm);
    }
}
