using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Admin
{
    public class RevenueBreakdown : BaseEntity
    {
        public DateTime PeriodDate { get; set; }
        public decimal ServiceRevenue { get; set; }
        public decimal WorkshopRevenue { get; set; }
        public decimal EventRevenue { get; set; }
        public decimal MerchandiseRevenue { get; set; }
        public decimal ConsultationRevenue { get; set; }
        public decimal MentoringRevenue { get; set; }
        public decimal AdvertisementRevenue { get; set; }
        public decimal TotalRevenue { get; set; }
        public string PeriodType { get; set; } // Daily, Weekly, Monthly, Quarterly, Yearly
    }
}
