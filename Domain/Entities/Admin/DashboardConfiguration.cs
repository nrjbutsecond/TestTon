using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Admin
{
    public class DashboardConfiguration : BaseEntity
    {
        public string UserId { get; set; }
        public string DashboardType { get; set; } // Admin, Staff, Organizer
        public string WidgetConfiguration { get; set; } // JSON configuration
        public string DateRangeDefault { get; set; } // Last30Days, Last90Days, LastYear
        public bool ShowRevenueTrend { get; set; } = true;
        public bool ShowUserGrowth { get; set; } = true;
        public bool ShowGeographic { get; set; } = true;
        public bool ShowTopServices { get; set; } = true;
        public bool ShowAlerts { get; set; } = true;
        public string Theme { get; set; } = "Light";
    }
    // dashboard setting... ;-;
}
