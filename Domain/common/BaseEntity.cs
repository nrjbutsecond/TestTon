using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}
