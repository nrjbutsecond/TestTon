using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Admin
{
    public class ServiceAnalytics : BaseEntity
    {
        public int ServicePlanId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceType { get; set; } // Basic, Event Planning, Mentoring, WOW Package
        public int OrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageRating { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal GrowthPercent { get; set; }
        public int UniqueCustomers { get; set; }
    }
}
