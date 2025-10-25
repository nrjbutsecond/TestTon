using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.MerchandiseEntity
{
    public class CartItemModel :BaseEntity
    {
        public int CartId {  get; set; }
        public int MerchandiseId { get; set; }
        public int? VariantId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public virtual CartModel CartModel { get; set; }
        public virtual Merchandise Merchandise { get; set; }
        public virtual MerchandiseVariant? Variant { get; set; }
    }
}
