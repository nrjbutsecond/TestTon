using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TalkEventModel : BaseEntity
    {
        public int OrganizerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public int MaxAttendees { get; set; }
        public bool HasTicketSale { get; set; }
        public TalkEventStatus Status { get; set; } = TalkEventStatus.Draft;
        public string? BannerImage { get; set; }
        public string? ThumbnailImage { get; set; }
        public string? CancellationReason { get; set; }

        // Navigation properties
        public virtual UserModel Organizer { get; set; } = null!;
        public virtual ICollection<TicketTypeModel> TicketTypes { get; set; } = new List<TicketTypeModel>();
        public virtual ICollection<TicketModel> Tickets { get; set; } = new List<TicketModel>();
    }

    

    public enum TalkEventStatus
    {
        Draft,
        Published,
        Ongoing,
        Completed,
        Cancelled
    }
}
