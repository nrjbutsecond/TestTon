using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Domain.Interface;
namespace Ticket.Domain.Interface
{
    public interface ITicketTypeRepo: IRepo<TicketTypeModel>
    {
        //use IEnumeable for lite(not waste memory keeping the entire list) and it only for read cannot change
        Task<IEnumerable<TicketTypeModel>> GetByTalkEventIdAsync(int talkEventId);
        Task<IEnumerable<TicketTypeModel>> GetByWorkshopIdAsync(int workthopId);

        // Get Available for Purchase
        Task<IEnumerable<TicketTypeModel>> GetAvailableByTalkEventIdAsync(int talkEventId);
        Task<IEnumerable<TicketTypeModel>> GetAvailableByWorkshopIdAsync(int workthopId);

        // Check availability
        Task<bool> IsAvailableForPurchaseAsync(int ticketTypeId, int quantity = 1);

        // Update sold quantity
        Task<bool> IncrementSoldQuantityAsync(int ticketTypeId, int quantity);
        Task<bool> DecrementSoldQuantityAsync(int ticketTypeId, int quantity);

        // Get with related data
        Task<TicketTypeModel?> GetWithEventDetailsAsync(int ticketTypeId);

        // Statistics
        Task<decimal> GetTotalRevenueByTicketTypeAsync(int ticketTypeId);
        Task<int> GetTotalSoldByTalkEventAsync(int talkEventId);
        Task<int> GetTotalSoldByWorkshopAsync(int workshopId);
    }
}
