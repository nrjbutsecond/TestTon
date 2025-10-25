using Domain.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ServicePlan
{
    public class ContractNegotiationModel : BaseEntity
    {
        public int UserId { get; set; }
        public int ServicePlanId { get; set; }

        [MaxLength(50)]
        public string RequestType { get; set; } // NewContract|Renewal|Upgrade

        [MaxLength(50)]
        public string CurrentStatus { get; set; } // Pending|InNegotiation|Approved|Rejected|Completed

        public string Requirements { get; set; } // JSON or Text
        public string ProposedTerms { get; set; } // JSON
        public string NegotiationNotes { get; set; }
        public decimal ProposedPrice { get; set; }
        public int ContractDuration { get; set; } // in months
        public DateTime RequestDate { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string HandledBy { get; set; } // Admin/Sales staff ID

        // Navigation properties
        public virtual UserModel User { get; set; }
        public virtual ServicePlanModel ServicePlan { get; set; }
        public virtual ICollection<UserServicePlanSubscriptionModel> Subscriptions { get; set; }
    }

    public enum ServicePlanCode
    {
        BASIC,
        STANDARD,
        PREMIUM
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }

    public enum ContractRequestType
    {
        NewContract,
        Renewal,
        Upgrade
    }

    public enum ContractStatus
    {
        Pending,
        InNegotiation,
        Approved,
        Rejected,
        Completed
    }
}
