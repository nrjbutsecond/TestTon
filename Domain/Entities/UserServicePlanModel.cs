using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.ServicePlan;
namespace Domain.Entities
{
    public class UserServicePlanModel : BaseEntity
    {
        public int UserId { get; set; }
        public virtual UserModel User { get; set; }

        public int ServicePlanId { get; set; }
        public virtual ServicePlanModel ServicePlan { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string PaymentStatus { get; set; } = "Pending"; // Pending, paid, Cancelled, Expired
        public decimal AmountPaid { get; set; }
        public string? TransactionId { get; set; }
    }
}
