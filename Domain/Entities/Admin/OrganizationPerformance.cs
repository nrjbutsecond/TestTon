using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Admin
{
    public class OrganizationPerformance : BaseEntity

    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public decimal TotalRevenue { get; set; }
        public int EventCount { get; set; }
        public int TicketsSold { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int Ranking { get; set; }
        public string Region { get; set; }
    }
}
