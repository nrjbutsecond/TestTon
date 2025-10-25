using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.MerchandiseEntity
{
    public class MerchandiseVariant :BaseEntity
    {
        public int MerchandiseId { get; set; }
        public string Name { get; set; } = string.Empty; // "Size", "Color"
        public string Value { get; set; } = string.Empty; // "L", "Red"
        public decimal PriceModifier { get; set; }
        public int StockQuantity { get; set; }

        // Navigation
        public virtual Merchandise Merchandise { get; set; } = null!;
    }
}
