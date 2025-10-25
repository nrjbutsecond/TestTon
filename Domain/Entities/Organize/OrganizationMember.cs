using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Organize
{
    public class OrganizationMember :BaseEntity
    {
        public int OrganizationId { get; set; }
        public int UserId { get; set; }
        public OrganizationRole Role { get; set; } = OrganizationRole.Member;
        public DateTime JoinedDate { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual OrganizationModel Organization { get; set; }
        public virtual UserModel User { get; set; }
    }
}
