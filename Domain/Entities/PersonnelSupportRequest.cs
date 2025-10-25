using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PersonnelSupportRequest :BaseEntity
    {
        public int OrganizerId { get; set; }
        public int EventId { get; set; }
        public string SupportType { get; set; } = string.Empty; // Technical|Logistics|Marketing|Other
        public int RequiredPersonnel { get; set; }
        public string Requirements { get; set; } = string.Empty; // JSON
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "Pending"; // Pending|Approved|Assigned|InProgress|Completed|Cancelled
        public int? ApprovedBy { get; set; }
        public string? FulfillmentNotes { get; set; }

        // Navigation properties
        public virtual UserModel? Organizer { get; set; }
        public virtual TalkEventModel? Event { get; set; }
        public virtual UserModel? ApprovedByUser { get; set; }
        public virtual ICollection<PersonnelSupportAssignment>? Assignments { get; set; }
    }

    public enum SupportRequestStatus
    {
        Pending,
        Approved,
        Assigned,
        InProgress,
        Completed,
        Cancelled
    }

    public enum SupportType
    {
        Technical,
        Logistics,
        Marketing,
        Other
    }
}