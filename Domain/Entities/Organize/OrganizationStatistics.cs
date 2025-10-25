using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Organize
{
    public class OrganizationStatistics :BaseEntity
    {
        public int OrganizationId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalEvents { get; set; }
        public int TotalAttendees { get; set; }
        public decimal Revenue { get; set; }
        public int NewMembers { get; set; }
        public decimal AverageRating { get; set; }

        public virtual OrganizationModel Organization { get; set; }
    }
}
