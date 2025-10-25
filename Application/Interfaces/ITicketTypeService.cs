using Application.DTOs.Common;
using Application.DTOs.Ticket;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Ticket.Application.Interface
{
    public interface ITicketTypeService
    {
        Task<TicketTypeDto?> GetByIdAsync(int id);
        Task<IEnumerable<TicketTypeDto>> GetByTalkEventAsync(int talkEventId);
        Task<IEnumerable<TicketTypeDto>> GetByWorkshopAsync(int workshopId);
        Task<IEnumerable<TicketTypeDto>> GetAvailableByTalkEventAsync(int talkEventId);
        Task<IEnumerable<TicketTypeDto>> GetAvailableByWorkshopAsync(int workshopId);

        // Paged operations
        Task<PagedResult<TicketTypeDto>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            bool? onlyAvailable = null,
            string? orderBy = null);

        // CRUD operations
        Task<TicketTypeDto> CreateForTalkEventAsync(CreateTicketTypeForTalkEventDto dto, string createdBy);
        Task<TicketTypeDto> CreateForWorkshopAsync(CreateTicketTypeForWorkshopDto dto, string createdBy);
        Task<TicketTypeDto?> UpdateAsync(int id, UpdateTicketTypeDto dto, string updatedBy);
        Task<bool> DeleteAsync(int id, string deletedBy, string reason);

        // Business operations
        Task<bool> CheckAvailabilityAsync(int ticketTypeId, int quantity);
        Task<bool> UpdateSoldQuantityAsync(int ticketTypeId, int quantity, bool isIncrement);
        Task<decimal> GetRevenueAsync(int ticketTypeId);
        Task<int> GetTotalSoldByTalkEventAsync(int talkEventId);
        Task<int> GetTotalSoldByWorkshopAsync(int workshopId);
    }


    /*
     
     
       
        
        
     
     */
}
