using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Mentor
{
    public class MentorBlockedTime : BaseEntity
    {
        public int MentorId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? Reason { get; set; }
        public bool IsAllDay { get; set; } = false;

        public virtual UserModel Mentor { get; set; } = null!;
    }
}
