using Application.DTOs.Ticket;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITicketService
    {
        Task<TicketDto> ReserveTicketAsync(ReserveTicketDto dto);
        Task<TicketDto> ConfirmPaymentAsync(Guid ticketGuid);
        Task<List<TicketDto>> GetUserTicketsAsync(int userId);
        Task<TicketDto> GetTicketByQRCodeAsync(string qrCode);
        Task<bool> CanOrganizerCancelTicketAsync(int ticketId, int organizerId);
        // Scanning
        Task<ScanTicketResultDto> ScanTicketAsync(string qrCode, string scannedBy);

        // Admin operations
        Task<bool> CancelTicketAsync(int ticketId, string reason, int userId, string role);
        Task<int> ExpireOldTicketsAsync();
    }
}
