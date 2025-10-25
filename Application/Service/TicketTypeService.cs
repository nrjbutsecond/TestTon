using Application.DTOs.Common;
using Application.DTOs.Ticket;
using Application.Helper;
using AutoMapper;
using Domain.Entities;
using Domain.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Ticket.Application.Interface;
using Ticket.Domain.Interface;
namespace Ticket.Application.Service
{
    public class TicketTypeService : ITicketTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TicketTypeService> _logger;

        public TicketTypeService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<TicketTypeService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TicketTypeDto?> GetByIdAsync(int id)
        {
            var ticketType = await _unitOfWork.TicketTypes.GetWithEventDetailsAsync(id);
            if (ticketType == null || ticketType.IsDeleted)
                return null;

            return _mapper.Map<TicketTypeDto>(ticketType);
        }

        public async Task<IEnumerable<TicketTypeDto>> GetByTalkEventAsync(int talkEventId)
        {
            var ticketTypes = await _unitOfWork.TicketTypes.GetByTalkEventIdAsync(talkEventId);
            return _mapper.Map<IEnumerable<TicketTypeDto>>(ticketTypes);
        }

        public async Task<IEnumerable<TicketTypeDto>> GetByWorkshopAsync(int workshopId)
        {
            var ticketTypes = await _unitOfWork.TicketTypes.GetByWorkshopIdAsync(workshopId);
            return _mapper.Map<IEnumerable<TicketTypeDto>>(ticketTypes);
        }

        public async Task<IEnumerable<TicketTypeDto>> GetAvailableByTalkEventAsync(int talkEventId)
        {
            var ticketTypes = await _unitOfWork.TicketTypes.GetAvailableByTalkEventIdAsync(talkEventId);
            return _mapper.Map<IEnumerable<TicketTypeDto>>(ticketTypes);
        }

        public async Task<IEnumerable<TicketTypeDto>> GetAvailableByWorkshopAsync(int workshopId)
        {
            var ticketTypes = await _unitOfWork.TicketTypes.GetAvailableByWorkshopIdAsync(workshopId);
            return _mapper.Map<IEnumerable<TicketTypeDto>>(ticketTypes);
        }

        public async Task<PagedResult<TicketTypeDto>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            bool? onlyAvailable = null,
            string? orderBy = null)
        {
            var query = _unitOfWork.TicketTypes.GetQueryable()
                .Include(tt => tt.TalkEvent)
                .Include(tt => tt.Workshop)
                .Where(tt => !tt.IsDeleted);

            // Filter by availability
            if (onlyAvailable == true)
            {
                var currentDate = DateTime.UtcNow;
                query = query.Where(tt =>
                    tt.SaleStartDate <= currentDate &&
                    tt.SaleEndDate >= currentDate &&
                    tt.SoldQuantity < tt.MaxQuantity);
            }

            // Ordering
            query = orderBy?.ToLower() switch
            {
                "price" => query.OrderBy(tt => tt.Price),
                "price_desc" => query.OrderByDescending(tt => tt.Price),
                "name" => query.OrderBy(tt => tt.Name),
                "date" => query.OrderBy(tt => tt.SaleStartDate),
                _ => query.OrderBy(tt => tt.Id)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = _mapper.Map<List<TicketTypeDto>>(items);

            return new PagedResult<TicketTypeDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<TicketTypeDto> CreateForTalkEventAsync(CreateTicketTypeForTalkEventDto dto, string createdBy)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Validate talk event exists
                var talkEvent = await _unitOfWork.TalkEvent.GetByIdAsync(dto.TalkEventId);
                if (talkEvent == null || talkEvent.IsDeleted)
                    throw new NotFoundException($"Talk event with id {dto.TalkEventId} not found");

                // Validate sale dates
                if (dto.SaleEndDate <= dto.SaleStartDate)
                    throw new BusinessException("Sale end date must be after sale start date");

                var ticketType = new TicketTypeModel
                {
                    TalkEventId = dto.TalkEventId,
                    WorkshopId = null,
                    Name = dto.Name,
                    Price = dto.Price,
                    MaxQuantity = dto.MaxQuantity,
                    Benefits = dto.Benefits != null ? JsonConvert.SerializeObject(dto.Benefits) : null,
                    SaleStartDate = dto.SaleStartDate,
                    SaleEndDate = dto.SaleEndDate,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy
                };

                await _unitOfWork.TicketTypes.AddAsync(ticketType);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                var result = await _unitOfWork.TicketTypes.GetWithEventDetailsAsync(ticketType.Id);
                return _mapper.Map<TicketTypeDto>(result);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating ticket type for talk event {TalkEventId}", dto.TalkEventId);
                throw;
            }
        }

        public async Task<TicketTypeDto> CreateForWorkshopAsync(CreateTicketTypeForWorkshopDto dto, string createdBy)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Validate workshop exists
                var workshop = await _unitOfWork.Workshops.GetByIdAsync(dto.WorkshopId);
                if (workshop == null || workshop.IsDeleted)
                    throw new NotFoundException($"Workshop with id {dto.WorkshopId} not found");

                // Validate sale dates
                if (dto.SaleEndDate <= dto.SaleStartDate)
                    throw new BusinessException("Sale end date must be after sale start date");

                var ticketType = new TicketTypeModel
                {
                    TalkEventId = null,
                    WorkshopId = dto.WorkshopId,
                    Name = dto.Name,
                    Price = dto.Price,
                    MaxQuantity = dto.MaxQuantity,
                    Benefits = dto.Benefits != null ? JsonConvert.SerializeObject(dto.Benefits) : null,
                    SaleStartDate = dto.SaleStartDate,
                    SaleEndDate = dto.SaleEndDate,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy
                };

                await _unitOfWork.TicketTypes.AddAsync(ticketType);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                var result = await _unitOfWork.TicketTypes.GetWithEventDetailsAsync(ticketType.Id);
                return _mapper.Map<TicketTypeDto>(result);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating ticket type for workshop {WorkshopId}", dto.WorkshopId);
                throw;
            }
        }

        public async Task<TicketTypeDto?> UpdateAsync(int id, UpdateTicketTypeDto dto, string updatedBy)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var ticketType = await _unitOfWork.TicketTypes.GetByIdAsync(id);
                if (ticketType == null || ticketType.IsDeleted)
                    return null;

                // Validate MaxQuantity change
                if (dto.MaxQuantity < ticketType.SoldQuantity)
                    throw new BusinessException($"Cannot set max quantity lower than sold quantity ({ticketType.SoldQuantity})");

                // Validate sale dates
                if (dto.SaleEndDate <= dto.SaleStartDate)
                    throw new BusinessException("Sale end date must be after sale start date");

                ticketType.Name = dto.Name;
                ticketType.Price = dto.Price;
                ticketType.MaxQuantity = dto.MaxQuantity;
                ticketType.Benefits = dto.Benefits != null ? JsonConvert.SerializeObject(dto.Benefits) : null;
                ticketType.SaleStartDate = dto.SaleStartDate;
                ticketType.SaleEndDate = dto.SaleEndDate;
                ticketType.UpdatedAt = DateTime.UtcNow;
                ticketType.UpdatedBy = updatedBy;

                _unitOfWork.TicketTypes.Update(ticketType);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                var result = await _unitOfWork.TicketTypes.GetWithEventDetailsAsync(id);
                return _mapper.Map<TicketTypeDto>(result);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error updating ticket type {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id, string deletedBy, string reason)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var ticketType = await _unitOfWork.TicketTypes.GetByIdAsync(id);
                if (ticketType == null || ticketType.IsDeleted)
                    return false;

                if (ticketType.SoldQuantity > 0)
                    throw new BusinessException("Cannot delete ticket type with sold tickets");

                ticketType.IsDeleted = true;
                ticketType.DeletedAt = DateTime.UtcNow;
                ticketType.UpdatedBy = deletedBy;
                ticketType.ReasonDelete = reason;

                _unitOfWork.TicketTypes.Update(ticketType);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error deleting ticket type {Id}", id);
                throw;
            }
        }

        public async Task<bool> CheckAvailabilityAsync(int ticketTypeId, int quantity)
        {
            return await _unitOfWork.TicketTypes.IsAvailableForPurchaseAsync(ticketTypeId, quantity);
        }

        public async Task<bool> UpdateSoldQuantityAsync(int ticketTypeId, int quantity, bool isIncrement)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                bool result;
                if (isIncrement)
                    result = await _unitOfWork.TicketTypes.IncrementSoldQuantityAsync(ticketTypeId, quantity);
                else
                    result = await _unitOfWork.TicketTypes.DecrementSoldQuantityAsync(ticketTypeId, quantity);

                if (!result)
                    throw new BusinessException("Failed to update sold quantity");

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error updating sold quantity for ticket type {Id}", ticketTypeId);
                throw;
            }
        }

        public async Task<decimal> GetRevenueAsync(int ticketTypeId)
        {
            return await _unitOfWork.TicketTypes.GetTotalRevenueByTicketTypeAsync(ticketTypeId);
        }

        public async Task<int> GetTotalSoldByTalkEventAsync(int talkEventId)
        {
            return await _unitOfWork.TicketTypes.GetTotalSoldByTalkEventAsync(talkEventId);
        }

        public async Task<int> GetTotalSoldByWorkshopAsync(int workshopId)
        {
            return await _unitOfWork.TicketTypes.GetTotalSoldByWorkshopAsync(workshopId);
        }
    }
}