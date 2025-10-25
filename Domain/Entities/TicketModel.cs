using Domain.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TicketModel :BaseEntity
    {
        [Required]
        public int UserId { get; set; }
        public virtual UserModel User { get; set; }

        [Required]
        public int TicketableId { get; set; }

        [Required]
        [MaxLength(50)]
        public TicketableTypes TicketableType { get; set; } // "TalkEvent" or "Workshop"

        [Required]
        public int TicketTypeId { get; set; }
        public virtual TicketTypeModel TicketType { get; set; }

        [Required]
        public Guid Guid { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(500)]
        public string QRCode { get; set; }

        [Required]
        
        public TicketStatus Status { get; set; } = TicketStatus.Reserved;

        public DateTime? PurchaseDate { get; set; }

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidUntil { get; set; }

        public string? ReasonDelete { get; set; }

        // Computed properties
        public bool IsValid => DateTime.UtcNow >= ValidFrom && DateTime.UtcNow <= ValidUntil;
        public bool IsExpired => DateTime.UtcNow > ValidUntil;
    }

   
    public enum TicketStatus
    {
        Reserved,
        Paid,
        Used,
        Cancelled,
        Expired
    }

   

    public enum TicketableTypes
    {
        TalkEvent,
        Workshop
    }
}