using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Mentor
{
    public class MentorAvailability :BaseEntity
    {
        public int MentorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsRecurring { get; set; } = true;
        public DateTime? RecurringEndDate { get; set; }

        // For one-time availability
        public DateTime? SpecificDate { get; set; }

        public virtual UserModel Mentor { get; set; } = null!;
    }
}
