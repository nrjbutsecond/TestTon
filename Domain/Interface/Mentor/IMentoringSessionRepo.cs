using Domain.Entities.Mentor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.Mentor
{
    public interface IMentoringSessionRepo: IRepo<MentoringRecord>
    {
        // Calendar queries
        Task<IEnumerable<MentoringRecord>> GetSessionsByMentorAndDateRangeAsync(
            int mentorId, DateTime startDate, DateTime endDate);

        Task<IEnumerable<MentoringRecord>> GetSessionsByMenteeAndDateRangeAsync(
            int menteeId, DateTime startDate, DateTime endDate);

        Task<IEnumerable<MentoringRecord>> GetUpcomingSessionsAsync(int userId);

        // Session queries
        Task<MentoringRecord?> GetSessionWithDetailsAsync(int sessionId);

        Task<bool> HasConflictingSessionAsync(
            int mentorId, DateTime startTime, DateTime endTime, int? excludeSessionId = null);

        Task<IEnumerable<MentoringRecord>> GetMentorSessionsAsync(
            int mentorId, DateTime? startDate = null, DateTime? endDate = null,
            MentoringSessionStatus? status = null);

        Task<IEnumerable<MentoringRecord>> GetMenteeSessionsAsync(
            int menteeId, DateTime? startDate = null, DateTime? endDate = null);

        // Statistics
        Task<int> GetTotalSessionCountAsync(int mentorId);
        Task<int> GetCompletedSessionCountAsync(int mentorId);
        Task<int> GetUpcomingSessionCountAsync(int mentorId);
        Task<double> GetAverageRatingAsync(int mentorId);
        Task<int> GetTotalMenteesCountAsync(int mentorId);
    }
}
