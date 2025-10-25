using Application.DTOs.Ticket;
using Application.Helper;
using Application.Interfaces;
using Application.Service;
using AutoMapper;
using Domain.Entities;
using Domain.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ticket.Application.Helper;
using Ticket.Application.Interface;
using Ticket.Domain.Interface;
namespace Ticket.Application.Service
{

    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TicketService> _logger;
        private readonly IEmailService _emailService;
        private readonly IQrCodeService _qrCodeService;
        public TicketService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IEmailService emailService,
            ILogger<TicketService> logger,
            IQrCodeService qrCode)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _emailService = emailService;
            _qrCodeService = qrCode;
        }

        public async Task<TicketDto> ReserveTicketAsync(ReserveTicketDto dto)
        {
            _logger.LogError(">>> RESERVE START - NO EMAIL SHOULD BE SENT <<<");
            // Validate user
            var user = await _unitOfWork.Users.GetByIdAsync(dto.UserId);
            if (user == null)
                throw new ArgumentException("User not found");

            // Validate ticket type
            var ticketType = await _unitOfWork.TicketTypes.GetByIdAsync(dto.TicketTypeId);
            if (ticketType == null || ticketType.IsDeleted)
                throw new ArgumentException("Ticket type not found");

            // Check availability
            if (!ticketType.IsOnSale)
                throw new InvalidOperationException("Ticket type is not currently on sale");

            if (ticketType.IsSoldOut)
                throw new InvalidOperationException("Ticket type is sold out");

            // Get event info
            var eventInfo = await GetEventInfo(ticketType);

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Create ticket
                var ticket = new TicketModel
                {
                    UserId = dto.UserId,
                    TicketTypeId = dto.TicketTypeId,
                    TicketableId = eventInfo.EventId,
                    TicketableType = eventInfo.EventType,
                    Status = TicketStatus.Reserved,
                    ValidFrom = eventInfo.EventDate.AddHours(-2),
                    ValidUntil = eventInfo.EventDate.AddHours(6),
                    QRCode = $"TEMP-{Guid.NewGuid()}",
                    CreatedBy = user.Email
                };

                var createdTicket = await _unitOfWork.Tickets.AddAsync(ticket);         
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                _logger.LogError(">>> RESERVE END - CHECK IF EMAIL WAS SENT <<<");
                return await BuildTicketDto(createdTicket, eventInfo);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<TicketDto> ConfirmPaymentAsync(Guid ticketGuid)
        {
            _logger.LogError(">>> CONFIRM PAYMENT START <<<");
            await _unitOfWork.BeginTransactionAsync();
            try { 
            var ticket = await _unitOfWork.Tickets.GetByGuidAsync(ticketGuid);
            if (ticket == null)
                throw new KeyNotFoundException("Ticket not found");
            if (ticket.Status != TicketStatus.Reserved)
                throw new InvalidOperationException($"Cannot confirm payment. Ticket status is {ticket.Status}");

            // Get related data
            var ticketType = await _unitOfWork.TicketTypes.GetByIdAsync(ticket.TicketTypeId);
            var user = await _unitOfWork.Users.GetByIdAsync(ticket.UserId);
            var eventInfo = await GetEventInfoByTicket(ticket);

            // Generate final QR code
            ticket.QRCode = GenerateQRCode();
            while (!await _unitOfWork.Tickets.IsQRCodeUniqueAsync(ticket.QRCode))
            {
                ticket.QRCode = GenerateQRCode();
            }

            // Update ticket status
            ticket.Status = TicketStatus.Paid;
            ticket.PurchaseDate = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;

            // udpate sold quantity after payment
            ticketType.SoldQuantity++;
            _unitOfWork.TicketTypes.Update(ticketType);

            _unitOfWork.Tickets.Update(ticket);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
            // Generate QR code image
            var qrResult = await _qrCodeService.GenerateQrCodeAsync(ticket);

            // Convert  data URL to byte array
            var base64String = qrResult.ImageUrl.Replace("data:image/png;base64,", "");
            var qrBytes = Convert.FromBase64String(base64String);

            // send email with ticket Qr
            await _emailService.SendTicketAsync(
                email: user.Email,
                fullName: user.FullName,
                eventName: eventInfo.EventName,
                qrCode: qrBytes,
                ticketCode: ticket.QRCode
            );

            return await BuildTicketDto(ticket, eventInfo);
        }
        catch(Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error in ConfirmPayment");
                throw;
            }
        }

        public async Task<List<TicketDto>> GetUserTicketsAsync(int userId)
        {
            var tickets = await _unitOfWork.Tickets.GetTicketsByUserIdAsync(userId);
            var ticketDtos = new List<TicketDto>();

            foreach (var ticket in tickets)
            {
                var eventInfo = await GetEventInfoByTicket(ticket);
                var dto = await BuildTicketDto(ticket, eventInfo);
                ticketDtos.Add(dto);
            }

            return ticketDtos.OrderByDescending(t => t.EventDate).ToList();
        }

        public async Task<TicketDto> GetTicketByQRCodeAsync(string qrCode)
        {
            var ticket = await _unitOfWork.Tickets.GetByQRCodeAsync(qrCode);
            if (ticket == null)
                throw new KeyNotFoundException("Ticket not found");

            var eventInfo = await GetEventInfoByTicket(ticket);
            return await BuildTicketDto(ticket, eventInfo);
        }

        public async Task<ScanTicketResultDto> ScanTicketAsync(string qrCode, string scannedBy)
        {
            var result = new ScanTicketResultDto();

            try
            {
                // Get ticket
                var ticket = await _unitOfWork.Tickets.GetByQRCodeAsync(qrCode);
                if (ticket == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Invalid QR code";
                    return result;
                }

                // Get ticket details
                var eventInfo = await GetEventInfoByTicket(ticket);
                result.Ticket = await BuildTicketDto(ticket, eventInfo);

                // Check if already used
                if (ticket.Status == TicketStatus.Used)
                {
                    var lastScan = await _unitOfWork.TicketScanLogs.GetLastScanAsync(ticket.Id);
                    result.IsSuccess = false;
                    result.Message = "Ticket has already been used";
                    result.LastScannedAt = lastScan?.ScannedAt;
                    return result;
                }

                // Check if valid status
                if (ticket.Status != TicketStatus.Paid)
                {
                    result.IsSuccess = false;
                    result.Message = $"Invalid ticket status: {ticket.Status}";
                    return result;
                }

                // Check validity period
                var now = DateTime.UtcNow;
                if (now < ticket.ValidFrom)
                {
                    result.IsSuccess = false;
                    result.Message = $"Ticket not valid yet. Valid from {ticket.ValidFrom:yyyy-MM-dd HH:mm}";
                    return result;
                }

                if (now > ticket.ValidUntil)
                {
                    result.IsSuccess = false;
                    result.Message = "Ticket has expired";
                    ticket.Status = TicketStatus.Expired;
                    _unitOfWork.Tickets.Update(ticket);
                    await _unitOfWork.SaveChangesAsync();
                    return result;
                }

                // Mark as used
                ticket.Status = TicketStatus.Used;
                ticket.UpdatedAt = now;
                _unitOfWork.Tickets.Update(ticket);

                // Log scan
                var scanLog = new TicketScanLogModel
                {
                    TicketId = ticket.Id,
                    ScannedAt = now,
                    ScannedBy = scannedBy,
                    CreatedBy = scannedBy
                };
                await _unitOfWork.TicketScanLogs.AddAsync(scanLog);

                await _unitOfWork.SaveChangesAsync();

                result.IsSuccess = true;
                result.Message = "Ticket successfully validated";
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scanning ticket");
                result.IsSuccess = false;
                result.Message = "An error occurred while scanning";
                return result;
            }
        }

        public async Task<bool> CanOrganizerCancelTicketAsync(int ticketId, int organizerId)
        {
            var ticket = await _unitOfWork.Tickets
                .GetQueryable()
                .Include(t => t.TicketType)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null) return false;
            if (ticket.TicketType.TalkEventId.HasValue)
            {
                var talkEvent = await _unitOfWork.TalkEvent.GetByIdAsync(ticket.TicketType.TalkEventId.Value);
                return talkEvent?.OrganizerId == organizerId;
            }
            if (ticket.TicketType.WorkshopId.HasValue)
            {
                var workshop = await _unitOfWork.Workshops.GetByIdAsync(ticket.TicketType.WorkshopId.Value);
                return workshop?.OrganizerId == organizerId;
            }

            return false;
        }


        public async Task<bool> CancelTicketAsync(int ticketId, string reason, int userId, string role)
        {

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Cancellation reason is required");

            var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new KeyNotFoundException("Ticket not found");

            if (role == "Organizer")
            {
                var canCancel = await CanOrganizerCancelTicketAsync(ticketId, userId);
                if (!canCancel)
                    throw new UnauthorizedException("You don't have permission to cancel this ticket");
            }

            if (ticket.Status == TicketStatus.Used)
                throw new InvalidOperationException("Cannot cancel used ticket");

            if (ticket.Status == TicketStatus.Cancelled)
                throw new InvalidOperationException("Ticket already cancelled");
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var previousStatus = ticket.Status;
                ticket.Status = TicketStatus.Cancelled;
                ticket.UpdatedAt = DateTime.UtcNow;
                ticket.UpdatedBy = userId.ToString();
                ticket.ReasonDelete = $"{reason} (by {role})";

                _unitOfWork.Tickets.Update(ticket);

                // Restore quantity if ticket was paid
                if (previousStatus == TicketStatus.Paid)
                {
                    var ticketType = await _unitOfWork.TicketTypes.GetByIdAsync(ticket.TicketTypeId);
                    if (ticketType != null && ticketType.SoldQuantity > 0)
                    {
                        ticketType.SoldQuantity--;
                        _unitOfWork.TicketTypes.Update(ticketType);
                    }

                    // TODO: Process refund if needed
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();



                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<int> ExpireOldTicketsAsync()
        {
            var expiredTickets = await _unitOfWork.Tickets.GetExpiredTicketsAsync();
            var count = 0;

            foreach (var ticket in expiredTickets)
            {
                ticket.Status = TicketStatus.Expired;
                ticket.UpdatedAt = DateTime.UtcNow;
                ticket.UpdatedBy = "System";

                _unitOfWork.Tickets.Update(ticket);

                // Restore quantity for reserved tickets
                if (ticket.Status == TicketStatus.Reserved)
                {
                    var ticketType = await _unitOfWork.TicketTypes.GetByIdAsync(ticket.TicketTypeId);
                    if (ticketType != null && ticketType.SoldQuantity > 0)
                    {
                        ticketType.SoldQuantity--;
                        _unitOfWork.TicketTypes.Update(ticketType);
                    }
                }
                count++;
            }

            if (count > 0)
            {
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation($"Expired {count} tickets");
            }

            return count;
        }

        // Helper methods
        private string GenerateQRCode()
        {
            return $"TKT-{Guid.NewGuid():N}-{DateTime.UtcNow.Ticks}";
        }

        private async Task<(int EventId, TicketableTypes EventType, string EventName, DateTime EventDate)> GetEventInfo(TicketTypeModel ticketType)
        {
            if (ticketType.TalkEventId.HasValue)
            {
                var talkEvent = await _unitOfWork.TalkEvent.GetByIdAsync(ticketType.TalkEventId.Value);
                if (talkEvent == null)
                    throw new InvalidOperationException("Event not found");

                return (talkEvent.Id, TicketableTypes.TalkEvent, talkEvent.Title, talkEvent.StartDate);
            }
            else if (ticketType.WorkshopId.HasValue)
            {
                var workshop = await _unitOfWork.Workshops.GetByIdAsync(ticketType.WorkshopId.Value);
                if (workshop == null)
                    throw new InvalidOperationException("Workshop not found");

                return (workshop.Id, TicketableTypes.Workshop, workshop.Title, workshop.StartDateTime);
            }

            throw new InvalidOperationException("Ticket type has no associated event");
        }

        private async Task<(int EventId, TicketableTypes EventType, string EventName, DateTime EventDate)> GetEventInfoByTicket(TicketModel ticket)
        {
            if (ticket.TicketType == null)
                ticket.TicketType = await _unitOfWork.TicketTypes.GetByIdAsync(ticket.TicketTypeId);

            return await GetEventInfo(ticket.TicketType);
        }

        private async Task<TicketDto> BuildTicketDto(TicketModel ticket, (int EventId, TicketableTypes EventType, string EventName, DateTime EventDate) eventInfo)
        {
            return new TicketDto
            {
                Id = ticket.Id,
                Guid = ticket.Guid,
                QRCode = ticket.QRCode,
                Status = ticket.Status,
                ValidFrom = ticket.ValidFrom,
                ValidUntil = ticket.ValidUntil,
                EventName = eventInfo.EventName,
                EventDate = eventInfo.EventDate,
                EventType = eventInfo.EventType,
                TicketTypeName = ticket.TicketType?.Name ?? "Unknown",
                Price = ticket.TicketType?.Price ?? 0,
                UserEmail = ticket.User?.Email ?? "Unknown",
                UserName = ticket.User?.FullName ?? "Unknown"
            };
        }
    }
}