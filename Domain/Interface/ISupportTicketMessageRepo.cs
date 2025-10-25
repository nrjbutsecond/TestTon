using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface ISupportTicketMessageRepo : IRepo<SupportTicketMessage>
    {
        Task<IEnumerable<SupportTicketMessage>> GetMessagesByTicketAsync(int ticketId);
        Task<SupportTicketMessage?> GetLatestMessageAsync(int ticketId);
        Task<int> GetMessageCountAsync(int ticketId);
    }
}
