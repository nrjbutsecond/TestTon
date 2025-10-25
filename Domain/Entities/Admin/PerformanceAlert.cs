using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Admin
{
    public class PerformanceAlert : BaseEntity
    {
        public string AlertType { get; set; } // Revenue Growth, User Engagement, Order Volume
        public string Title { get; set; }
        public string Message { get; set; }
        public string Severity { get; set; } // Low, Medium, High, Critical
        public DateTime TriggeredAt { get; set; }
        public bool IsResolved { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string ResolvedBy { get; set; }
        public decimal ThresholdValue { get; set; }
        public decimal ActualValue { get; set; }
        public string MetricName { get; set; }
    }
}
