using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CollaboratorModel :BaseEntity
    {
        public int OrganizerId { get; set; }
        public UserModel Organizer { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Skills { get; set; }
        public bool IsAvailable { get; set; }

        // For personnel support pool
        public bool IsInSupportPool { get; set; }
    }
}
