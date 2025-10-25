using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Admin
{
    public class ConversionFunnel : BaseEntity
    {
        public DateTime Date { get; set; }
        public string FunnelStage { get; set; } // Visit, Registration, Browse, AddToCart, Checkout, Purchase
        public int UserCount { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal DropOffRate { get; set; }
        public string UserSegment { get; set; } // New, Returning, Premium
        public string Source { get; set; } // Organic, Paid, Direct, Referral
    }
}
