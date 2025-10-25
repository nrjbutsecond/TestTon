using Domain.common;
using Domain.Entities.MerchandiseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Organize
{
    public class OrganizationModel : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public OrganizationType Type { get; set; } = OrganizationType.Standard;
        public string Website { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        // Address
        public string Address { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Country { get; set; }
        public string Location { get; set; } // Full location string

        // Partnership
        public PartnershipTier PartnershipTier { get; set; } = PartnershipTier.None;
        public PartnershipStatus Status { get; set; } = PartnershipStatus.Pending;
        public DateTime? LicenseActiveUntil { get; set; }
        public DateTime FoundedDate { get; set; }

        // Media
        public string LogoUrl { get; set; }
        public string CoverImageUrl { get; set; }

        // Statistics (cached values)
        public decimal Rating { get; set; } = 0;
        public int TotalEvents { get; set; } = 0;
        public int ActivePartners { get; set; } = 0;
        public int TotalAttendees { get; set; } = 0;
        public decimal MonthlyRevenue { get; set; } = 0;

        // Relationships
        public virtual ICollection<OrganizationMember> Members { get; set; } = new List<OrganizationMember>();
        public virtual ICollection<TalkEventModel> Events { get; set; } = new List<TalkEventModel>();
        public virtual ICollection<PartnershipApplication> Applications { get; set; } = new List<PartnershipApplication>();
        public virtual ICollection<OrganizationActivity> Activities { get; set; } = new List<OrganizationActivity>();
        public virtual ICollection<Merchandise> Merchandises { get; set; } = new List<Merchandise>();
    }
}
