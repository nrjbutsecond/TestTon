using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Admin
{
    public class SystemStatistics : BaseEntity
    {
        public DateTime SnapshotTime { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveOrders { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int TotalEvents { get; set; }
        public int TotalWorkshops { get; set; }
        public int TotalMerchandise { get; set; }
        public int TotalOrganizations { get; set; }
        public decimal SystemUptime { get; set; } // Percentage
        public int ApiCallCount { get; set; }
        public decimal AverageResponseTime { get; set; } // Milliseconds
    }
}
