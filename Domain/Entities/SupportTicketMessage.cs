using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SupportTicketMessage : BaseEntity
    {
        public int SupportTicketId { get; set; }
        public SupportTicket SupportTicket { get; set; } = null!;

        public int SenderId { get; set; }
        public UserModel Sender { get; set; } = null!;

        public string Content { get; set; } = null!;

        // Attachments (stored as JSON array of URLs)
        public string? Attachments { get; set; }

        public bool IsInternal { get; set; } = false; // Internal staff notes
        public bool IsCustomerMessage { get; set; } = true;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
