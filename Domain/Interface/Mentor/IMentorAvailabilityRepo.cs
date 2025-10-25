using Domain.Entities.Mentor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.Mentor
{
    public interface IMentorAvailabilityRepo : IRepo<MentorAvailability>
    {
        Task<IEnumerable<MentorAvailability>> GetByMentorIdAsync(int mentorId);
        Task<MentorAvailability?> GetByIdAndMentorAsync(int availabilityId, int mentorId);
        Task<bool> HasOverlapAsync(int mentorId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime, int? excludeId = null);
    }
}
