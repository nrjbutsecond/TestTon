using Domain.Entities.Mentor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.Mentor
{
    public interface IMentorBlockedTimeRepo : IRepo<MentorBlockedTime>
    {
        Task<IEnumerable<MentorBlockedTime>> GetByMentorAndDateRangeAsync(
            int mentorId, DateTime startDate, DateTime endDate);

        Task<MentorBlockedTime?> GetByIdAndMentorAsync(int blockedTimeId, int mentorId);

        Task<bool> HasOverlapAsync(
            int mentorId, DateTime startTime, DateTime endTime, int? excludeId = null);
    }
}
