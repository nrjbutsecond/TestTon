using Domain.Entities;
using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Ticket.Domain.Interface
{
    public interface ITicketRepo : IRepo<TicketModel>
    {

        /*
        // Get tickets by user
        Task<IEnumerable<TicketModel>> GetTicketsByUserIdAsync(int userId);
        Task<IEnumerable<TicketModel>> GetActiveTicketsByUserIdAsync(int userId);

        // Get tickets by event/workshop
        Task<IEnumerable<TicketModel>> GetTicketsByEventAsync(int eventId, string ticketableType);
        Task<IEnumerable<TicketModel>> GetTicketsByTalkEventAsync(int talkEventId);
        Task<IEnumerable<TicketModel>> GetTicketsByWorkshopAsync(int workshopId);

        // Get tickets by status
        Task<IEnumerable<TicketModel>> GetTicketsByStatusAsync(string status);
        Task<IEnumerable<TicketModel>> GetExpiredTicketsAsync();

        // Get ticket by unique identifiers --- use for scan
        Task<TicketModel?> GetByGuidAsync(Guid guid);
        Task<TicketModel?> GetByQRCodeAsync(string qrCode);

        // Check ticket validity 
        Task<bool> IsTicketValidAsync(int ticketId);
        Task<bool> IsQRCodeUniqueAsync(string qrCode, int? excludeTicketId = null);

        // Update ticket status
        Task<bool> UpdateTicketStatusAsync(int ticketId, string newStatus);
        Task<bool> MarkTicketAsUsedAsync(int ticketId);
        Task<bool> CancelTicketAsync(int ticketId, string reason);

        // Batch operations
        Task<int> ExpireOldTicketsAsync();
        Task<IEnumerable<TicketModel>> GetTicketsByTicketTypeAsync(int ticketTypeId);

        // Statistics
        Task<int> CountTicketsByStatusAsync(string status);
        Task<int> CountTicketsByEventAsync(int eventId, string ticketableType);


        */
        // (( i don't have time to do this now, but i will do it later ))
        Task<TicketModel?> GetByGuidAsync(Guid guid);
        Task<TicketModel?> GetByQRCodeAsync(string qrCode);
        Task<IEnumerable<TicketModel>> GetTicketsByUserIdAsync(int userId);
        Task<bool> IsQRCodeUniqueAsync(string qrCode);

        // For expiration job
        Task<IEnumerable<TicketModel>> GetExpiredTicketsAsync();
    }
}
