using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.MerchandiseEntity
{
    public class OrderFilter
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? OrderNumber { get; set; }
        public OrderStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? SortBy { get; set; } = "OrderDate";
        public bool SortDescending { get; set; } = true;
    }
}
