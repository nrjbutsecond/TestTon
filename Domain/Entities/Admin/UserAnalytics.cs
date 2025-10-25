using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Admin
{
    public class UserAnalytics : BaseEntity

    {
        public DateTime Date { get; set; }
        public int NewUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int ReturnUsers { get; set; }
        public int ChurnedUsers { get; set; }
        public decimal RetentionRate { get; set; }
        public decimal GrowthRate { get; set; }
        public string UserType { get; set; } // Guest, Member, Organizer, Staff, Admin
        public string AcquisitionChannel { get; set; } // Organic, Paid, Referral, Direct
    }
}
