using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Organize
{
    public class PartnershipApplication :BaseEntity
    {
        public int OrganizationId { get; set; }
        public PartnershipTier RequestedTier { get; set; }
        public string ApplicationReason { get; set; }
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
        public DateTime ApplicationDate { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string ReviewNotes { get; set; }
        public int? ReviewedByUserId { get; set; }

        public virtual OrganizationModel Organization { get; set; }
        public virtual UserModel ReviewedBy { get; set; }
    }
}
