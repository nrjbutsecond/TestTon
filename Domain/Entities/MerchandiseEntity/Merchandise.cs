using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.MerchandiseEntity
{
    public class Merchandise: BaseEntity
    {
        public int? SellerId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public int StockQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public bool IsOfficial { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Images { get; set; }  // TODO: now base64 will change to other in future

        // Navigation properties
        public virtual UserModel? Seller { get; set; }
        public virtual ICollection<MerchandiseVariant> Variants { get; set; } = new List<MerchandiseVariant>(); //haven't finish
    }

    public enum MerchandiseCategory //TODO: use in future (phase 3, 4 and 5)
    {
        Apparel,
        Accessories,
        Books,
        Other
    }
}
