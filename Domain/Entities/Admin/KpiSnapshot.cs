using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Admin
{
    public class KpiSnapshot :BaseEntity
    {
        public DateTime SnapshotDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalActiveUsers { get; set; }
        public int TotalOrders { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal RevenueGrowthPercent { get; set; }
        public decimal UserGrowthPercent { get; set; }
        public decimal OrderGrowthPercent { get; set; }
        public decimal ConversionGrowthPercent { get; set; }
        public string Period { get; set; } // Daily, Monthly, Yearly
    }
}
