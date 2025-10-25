using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Admin
{
    public class GeographicAnalytics : BaseEntity
    {
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; } = "Vietnam";
        public int UserCount { get; set; }
        public decimal UserPercentage { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
        public DateTime PeriodDate { get; set; }
        public decimal GrowthRate { get; set; }
        public int ActiveOrganizations { get; set; }
    }
}
