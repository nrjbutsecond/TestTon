using Domain.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ServicePlan
{
    public  class ServicePlanModel : BaseEntity
    {
        [Required]
        [MaxLength(20)]
        public string Code { get; set; } // BASIC|STANDARD|PREMIUM

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public decimal MonthlyPrice { get; set; }

        public decimal YearlyPrice { get; set; }

        public int MaxEvents { get; set; }

        public int MaxMerchandiseItems { get; set; }

        public bool IncludesConsultation { get; set; }

        public bool IncludesPersonnelSupport { get; set; }

        public int ConsultationHours { get; set; }

        public string Features { get; set; } // JSON array

        public bool IsActive { get; set; } = true;

        public bool IsPopular { get; set; } = false;

        public int DisplayOrder { get; set; }

        public decimal? DiscountPercentage { get; set; }

        // Navigation properties
        public virtual ICollection<ContractNegotiationModel> ContractNegotiations { get; set; }
        public virtual ICollection<UserServicePlanSubscriptionModel> UserSubscriptions { get; set; }
        public virtual ICollection<ConsultationRequest> ConsultationRequests { get; set; }
    }
}
