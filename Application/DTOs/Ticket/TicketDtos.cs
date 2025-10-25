using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Ticket
{
    public class TicketDto
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string QRCode { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsValid { get; set; }
        public bool IsExpired { get; set; }

        // Event/Workshop info
        public int EventId { get; set; }
        public TicketableTypes EventType { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string EventLocation { get; set; }

        // Ticket type info
        public string TicketTypeName { get; set; }
        public decimal Price { get; set; }
        public string Benefits { get; set; }

        // User info
        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }

    public class CreateTicketDto
    {
        public int UserId { get; set; }
        public int TicketTypeId { get; set; }
        public int EventId { get; set; }
        public string EventType { get; set; } // "TalkEvent" or "Workshop"
    }

    public class ReserveTicketDto
    {
        public int UserId { get; set; }
        public int TicketTypeId { get; set; }
    }

    public class ConfirmTicketPaymentDto
    {
        public Guid TicketGuid { get; set; }
        public string PaymentReference { get; set; }
    }

    public class ValidateTicketDto
    {
        public string QRCode { get; set; }
    }

    public class TicketValidationResultDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public TicketDto Ticket { get; set; }
    }

    public class ScanTicketResultDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public TicketDto Ticket { get; set; }
        public DateTime? LastScannedAt { get; set; }
    }
}
