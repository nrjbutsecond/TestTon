using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.MerchandiseEntity
{
    public class ShippingConfig: BaseEntity
    {
        public string City { get; set; } = string.Empty;
        public string? District { get; set; }
        public string? Ward {  get; set; }

        //business rule
     
        public decimal FreeShippingThreshold { get; set; } = 500000m;
        public decimal BulkOrderThreshold { get; set; } = 10;
        public decimal BulkOrderExtraFee { get; set; } = 10000m;

        // Provider Configuration
        public string? PreferredProvider { get; set; } // "local", "ghn", "ghtk", etc.
        public decimal MarkupPercentage { get; set; } = 0; // Markup on provider rates
        public decimal MarkupFixedAmount { get; set; } = 0; // Fixed markup amount
        public decimal MinShippingFee { get; set; } = 0; // Minimum fee regardless of provider
        public decimal MaxShippingFee { get; set; } = 1000000m; // Maximum fee cap

        //

        public decimal BaseFee { get; set; }// Used when provider = "local"

        // Promotional Rules
        public decimal DiscountPercentage { get; set; } = 0; // Shipping discount
        public DateTime? PromotionStartDate { get; set; }
        public DateTime? PromotionEndDate { get; set; }


        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public int Priority { get; set; } = 0; // Higher priority configs override lower ones
    }
}
