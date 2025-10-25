using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Admin
{
    public class ActivityLog : BaseEntity
    {
        public string ActivityType { get; set; } // Revenue Spike, Milestone, Alert
        public string Description { get; set; }
        public string Severity { get; set; } // Info, Warning, Critical
        public string Category { get; set; } // Revenue, User, Order, System
        public DateTime OccurredAt { get; set; }
        public string UserId { get; set; }
        public string EntityType { get; set; } // User, Order, Event, etc.
        public int? EntityId { get; set; }
        public string Metadata { get; set; } // JSON data for additional info
    }
}
