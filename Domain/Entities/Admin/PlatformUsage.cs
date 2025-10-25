using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Admin
{
    public class PlatformUsage : BaseEntity
    {
        public DateTime Date { get; set; }
        public string DeviceType { get; set; } // Desktop, Mobile, Tablet
        public int SessionCount { get; set; }
        public int UniqueUsers { get; set; }
        public decimal UsagePercentage { get; set; }
        public TimeSpan AverageSessionDuration { get; set; }
        public int PageViewsPerSession { get; set; }
        public decimal BounceRate { get; set; }
        public string Browser { get; set; }
        public string OperatingSystem { get; set; }
    }
    // :v using when have mobile,
}
