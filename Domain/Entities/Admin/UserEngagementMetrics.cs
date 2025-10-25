using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Admin
{
    public class UserEngagementMetrics : BaseEntity
    {
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public int LoginCount { get; set; }
        public TimeSpan TotalSessionTime { get; set; }
        public int PageViews { get; set; }
        public int ActionsPerformed { get; set; }
        public DateTime LastActiveAt { get; set; }
        public int DaysSinceLastActivity { get; set; }
        public string EngagementLevel { get; set; } // Low, Medium, High, VeryHigh
        public decimal EngagementScore { get; set; }
    }
}
