using Domain.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.MerchandiseEntity
{
    public class OrderItemModel :BaseEntity
    {
        public int OrderId { get; set; }
        public int MerchandiseId { get; set; }
        public int? VariantId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        // Navigation properties
        public virtual OrderModel Order { get; set; }
        public virtual Merchandise Merchandise { get; set; }
        public virtual MerchandiseVariant? Variant { get; set; }
    }
}
