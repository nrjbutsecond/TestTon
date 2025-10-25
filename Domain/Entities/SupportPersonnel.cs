using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SupportPersonnel : BaseEntity
    {
        public int UserId { get; set; }
        public int RegisteredBy { get; set; }
        public string Skills { get; set; } = string.Empty; // JSON array
        public string Availability { get; set; } = string.Empty; // JSON schedule
        public bool IsActive { get; set; } = true;
        public int ExperienceLevel { get; set; } // 1-5
        public string? Bio { get; set; }

        // Navigation properties
        public virtual UserModel? User { get; set; }
        public virtual UserModel? RegisteredByUser { get; set; }
        public virtual ICollection<PersonnelSupportAssignment>? Assignments { get; set; }
    }

    // Junction table for many-to-many relationship
    public class PersonnelSupportAssignment : BaseEntity
    {
        public int PersonnelSupportRequestId { get; set; }
        public int SupportPersonnelId { get; set; }
        public DateTime AssignedDate { get; set; }
        public string Status { get; set; } = "Assigned"; // Assigned|Confirmed|InProgress|Completed|Cancelled
        public string? Notes { get; set; }

        // Navigation properties
        public virtual PersonnelSupportRequest? Request { get; set; }
        public virtual SupportPersonnel? Personnel { get; set; }
    }

    public enum AssignmentStatus
    {
        Assigned,
        Confirmed,
        InProgress,
        Completed,
        Cancelled
    }
}