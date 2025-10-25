using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Admin
{
    public class RevenueAnalytics : BaseEntity

    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public string PeriodType { get; set; } // Daily, Monthly, Yearly
        public int TransactionCount { get; set; }
        public decimal AverageOrderValue { get; set; }
        public string Category { get; set; } // Service, Workshop, Merchandise, Event
        public int OrganizationId { get; set; }
    }
}
