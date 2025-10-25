using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ReviewResponseModel :BaseEntity
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsSellerResponse { get; set; }

        // Navigation properties
        public virtual ReviewModel Review { get; set; }
        public virtual UserModel User { get; set; }
    }
}
