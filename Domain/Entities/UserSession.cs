using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserSession : BaseEntity
    {
        public int UserId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime LastPingAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public bool IsActive { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? DeviceInfo { get; set; }

        public virtual UserModel User { get; set; } = null!;
    }
}
