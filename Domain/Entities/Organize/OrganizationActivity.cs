using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Organize
{
    public class OrganizationActivity :BaseEntity
    {
        public int OrganizationId { get; set; }
        public int? UserId { get; set; }
        public ActivityType Type { get; set; }
        public string Description { get; set; }
        public DateTime ActivityDate { get; set; }
        public string Details { get; set; } // JSON for additional data

        public virtual OrganizationModel Organization { get; set; }
        public virtual UserModel User { get; set; }
    }
}
