using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Mentor
{
    public class MentoringSessionParticipant :BaseEntity
    {
        public int MentoringRecordId { get; set; }
        public int UserId { get; set; }
        public bool HasJoined { get; set; } = false;
        public DateTime? JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
        public string? ParticipantNotes { get; set; }
        public int? Rating { get; set; }
        public string? Feedback { get; set; }

        public virtual MentoringRecord MentoringRecord { get; set; } = null!;
        public virtual UserModel User { get; set; } = null!;
    }
}
