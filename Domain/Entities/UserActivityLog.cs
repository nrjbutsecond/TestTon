using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserActivityLog : BaseEntity
    {
        public int UserId { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public string? Path { get; set; }
        public string? Method { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Details { get; set; }

        public virtual UserModel User { get; set; } = null!;
    }
}
