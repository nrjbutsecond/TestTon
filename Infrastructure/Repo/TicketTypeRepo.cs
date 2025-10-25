using Domain.Entities;
using Infrastructure.Repo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Ticket.Domain.Interface;
using Ticket.Infrastructure.Data;
namespace Ticket.Infrastructure.Repo
{
    public class TicketTypeRepo : Repo<TicketTypeModel>, ITicketTypeRepo
    {
        //Inheritance Pattern
        public TicketTypeRepo(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<TicketTypeModel>> GetByTalkEventIdAsync(int talkEventId)
        {
            return await _context.TicketTypes
                .Where(tt => tt.TalkEventId == talkEventId && !tt.IsDeleted)
                .OrderBy(tt => tt.Price)
                .ToListAsync();
        }

        public async Task<IEnumerable<TicketTypeModel>> GetByWorkshopIdAsync(int workshopId)
        {
            return await _context.TicketTypes
                .Where(tt => tt.WorkshopId == workshopId && !tt.IsDeleted)
                .OrderBy(tt => tt.Price)
                .ToListAsync();
        }

        public async Task<IEnumerable<TicketTypeModel>> GetAvailableByTalkEventIdAsync(int talkEventId)
        {
            var currentDate = DateTime.UtcNow;
            return await _context.TicketTypes
                .Where(tt => tt.TalkEventId == talkEventId &&
                            !tt.IsDeleted &&
                            tt.SaleStartDate <= currentDate &&
                            tt.SaleEndDate >= currentDate &&
                            tt.SoldQuantity < tt.MaxQuantity)
                .OrderBy(tt => tt.Price)
                .ToListAsync();
        }

        public async Task<IEnumerable<TicketTypeModel>> GetAvailableByWorkshopIdAsync(int workshopId)
        {
            var currentDate = DateTime.UtcNow;
            return await _context.TicketTypes
                .Where(tt => tt.WorkshopId == workshopId &&
                            !tt.IsDeleted &&
                            tt.SaleStartDate <= currentDate &&
                            tt.SaleEndDate >= currentDate &&
                            tt.SoldQuantity < tt.MaxQuantity)
                .OrderBy(tt => tt.Price)
                .ToListAsync();
        }

        public async Task<bool> IsAvailableForPurchaseAsync(int ticketTypeId, int quantity = 1)
        {
            var ticketType = await GetByIdAsync(ticketTypeId);
            if (ticketType == null || ticketType.IsDeleted) return false;

            var currentDate = DateTime.UtcNow;
            return ticketType.SaleStartDate <= currentDate &&
                   ticketType.SaleEndDate >= currentDate &&
                   (ticketType.MaxQuantity - ticketType.SoldQuantity) >= quantity;
        }

        public async Task<bool> IncrementSoldQuantityAsync(int ticketTypeId, int quantity)
        {
            var ticketType = await GetByIdAsync(ticketTypeId);
            if (ticketType == null || ticketType.IsDeleted) return false;

            if (ticketType.SoldQuantity + quantity > ticketType.MaxQuantity) return false;

            ticketType.SoldQuantity += quantity;
            ticketType.UpdatedAt = DateTime.UtcNow;

            Update(ticketType);
            return true;
        }

        public async Task<bool> DecrementSoldQuantityAsync(int ticketTypeId, int quantity)
        {
            var ticketType = await GetByIdAsync(ticketTypeId);
            if (ticketType == null) return false;

            if (ticketType.SoldQuantity - quantity < 0) return false;

            ticketType.SoldQuantity -= quantity;
            ticketType.UpdatedAt = DateTime.UtcNow;

            Update(ticketType);
            return true;
        }

        public async Task<TicketTypeModel?> GetWithEventDetailsAsync(int ticketTypeId)
        {
            return await _context.TicketTypes
                .Include(tt => tt.TalkEvent)
                .Include(tt => tt.Workshop)
                .FirstOrDefaultAsync(tt => tt.Id == ticketTypeId && !tt.IsDeleted);
        }

        public async Task<decimal> GetTotalRevenueByTicketTypeAsync(int ticketTypeId)
        {
            var ticketType = await GetByIdAsync(ticketTypeId);
            if (ticketType == null) return 0;

            return ticketType.Price * ticketType.SoldQuantity;
        }

        public async Task<int> GetTotalSoldByTalkEventAsync(int talkEventId)
        {
            return await _context.TicketTypes
                .Where(tt => tt.TalkEventId == talkEventId && !tt.IsDeleted)
                .SumAsync(tt => tt.SoldQuantity);
        }

        public async Task<int> GetTotalSoldByWorkshopAsync(int workshopId)
        {
            return await _context.TicketTypes
                .Where(tt => tt.WorkshopId == workshopId && !tt.IsDeleted)
                .SumAsync(tt => tt.SoldQuantity);
        }
    }
}