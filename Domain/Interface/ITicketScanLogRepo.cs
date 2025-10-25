using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface ITicketScanLogRepo : IRepo<TicketScanLogModel>
    {
        Task<TicketScanLogModel?> GetLastScanAsync(int ticketId);
        Task<IEnumerable<TicketScanLogModel>> GetTicketScansAsync(int ticketId);
    }
}
