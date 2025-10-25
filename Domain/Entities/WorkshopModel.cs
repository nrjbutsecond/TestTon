using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class WorkshopModel :BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public string? OnlineLink { get; set; }

        public bool IsOfficial { get; set; } // true = official, false = partnered
        public int? OrganizerId { get; set; } // null if official
        public virtual UserModel? Organizer { get; set; }

        public decimal Price { get; set; }
        public int MaxParticipants { get; set; }
        public int CurrentParticipants { get; set; } = 0;

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime RegistrationDeadline { get; set; }

        public WorkshopStatus Status { get; set; } = WorkshopStatus.Draft;
        public string? CancellationReason { get; set; }

        // Navigation
        public virtual ICollection<TicketTypeModel> TicketTypes { get; set; } = new List<TicketTypeModel>();
        public virtual ICollection<TicketModel> Tickets { get; set; } = new List<TicketModel>();
    }


    //idk when use this
    public enum WorkshopStatus
    {
        Draft,
        Published,
        InProgress,
        Completed,
        Cancelled
    }
}

