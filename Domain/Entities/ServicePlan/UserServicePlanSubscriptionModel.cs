using Domain.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ServicePlan
{
    public class UserServicePlanSubscriptionModel : BaseEntity
    {
        public int UserId { get; set; }
        public int ServicePlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PaidAmount { get; set; }

        [MaxLength(50)]
        public string PaymentMethod { get; set; }

        [MaxLength(50)]
        public string PaymentStatus { get; set; } // Pending|Completed|Failed|Refunded

        public bool IsActive { get; set; }
        public bool AutoRenew { get; set; }
        public int? ContractNegotiationId { get; set; }

        // Navigation properties
        public virtual UserModel User { get; set; }
        public virtual ServicePlanModel ServicePlan { get; set; }
        public virtual ContractNegotiationModel ContractNegotiation { get; set; }
    }
}
