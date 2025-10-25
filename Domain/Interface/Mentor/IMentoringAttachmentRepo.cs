using Domain.Entities.Mentor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.Mentor
{
    public interface IMentoringAttachmentRepo : IRepo<MentoringSessionAttachment>
    {
        Task<IEnumerable<MentoringSessionAttachment>> GetBySessionIdAsync(int sessionId);
        Task<MentoringSessionAttachment?> GetByIdWithSessionAsync(int attachmentId);
    }
}
