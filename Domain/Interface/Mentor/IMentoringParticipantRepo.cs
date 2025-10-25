using Domain.Entities.Mentor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.Mentor
{
    public interface IMentoringParticipantRepo : IRepo<MentoringSessionParticipant>
    {
        Task<MentoringSessionParticipant?> GetBySessionAndUserAsync(int sessionId, int userId);
        Task<IEnumerable<MentoringSessionParticipant>> GetBySessionIdAsync(int sessionId);
        Task<int> GetParticipantCountAsync(int sessionId);
    }
}
