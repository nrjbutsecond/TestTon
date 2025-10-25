using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Admin
{
    public class OrderAnalytics : BaseEntity
    {
        public DateTime Date { get; set; }
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal CompletionRate { get; set; }
        public decimal CancellationRate { get; set; }
        public decimal AverageOrderValue { get; set; }
        public TimeSpan AverageProcessingTime { get; set; }
        public string TopCategory { get; set; }
        public decimal RefundRate { get; set; }
    }
}
