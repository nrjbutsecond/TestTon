using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Mentor
{
    public class MentoringSessionAttachment : BaseEntity
    {
        public int MentoringRecordId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string? FileType { get; set; }
        public long FileSize { get; set; }
        public string UploadedBy { get; set; } = string.Empty;

        public virtual MentoringRecord MentoringRecord { get; set; } = null!;
    }
}
