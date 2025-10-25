using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Organize
{
    public enum OrganizationType
    {
        Standard,
        Featured
    }

    public enum PartnershipTier
    {
        None,
        Standard,
        VIP
    }

    public enum PartnershipStatus
    {
        Pending,
        Active,
        Suspended,
        Expired
    }

    public enum OrganizationRole
    {
        Member,
        Curator,
        MarketingLead,
        Organizer,
        Admin
    }

    public enum ApplicationStatus
    {
        Pending,
        UnderReview,
        Approved,
        Rejected,
        Cancelled
    }

    public enum ActivityType
    {
        Created,
        Updated,
        EventCreated,
        EventUpdated,
        EventCompleted,
        EventCancelled,
        MemberAdded,
        MemberRemoved,
        RoleChanged,
        PartnershipApproved,
        PartnershipUpgraded,
        MerchandiseAdded,
        MerchandiseUpdated,
        LicenseRenewed,
        ProposalSubmitted
    }
}
