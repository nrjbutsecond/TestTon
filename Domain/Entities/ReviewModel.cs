using Domain.common;
using Domain.Entities.MerchandiseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ReviewModel: BaseEntity
    {
        public int UserId { get; set; }
        public int MerchandiseId { get; set; }
        public int? OrderItemId { get; set; } // Link to specific purchase
        public int Rating { get; set; } 
        public string? Title { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Images { get; set; } // JSON array of image URLs
        public bool IsVerifiedPurchase { get; set; }
        public int HelpfulCount { get; set; } = 0;
        public int UnhelpfulCount { get; set; } = 0;

        // Navigation properties
        public virtual UserModel User { get; set; }
        public virtual Merchandise Merchandise { get; set; }
        public virtual OrderItemModel? OrderItem { get; set; }
        public virtual ICollection<ReviewResponseModel> Responses { get; set; } = new List<ReviewResponseModel>();
        public virtual ICollection<ReviewVoteModel> Votes { get; set; } = new List<ReviewVoteModel>();
    }
    public class ReviewStats
    {
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public int FiveStarCount { get; set; }
        public int FourStarCount { get; set; }
        public int ThreeStarCount { get; set; }
        public int TwoStarCount { get; set; }
        public int OneStarCount { get; set; }
        public int VerifiedPurchaseCount { get; set; }
    }
}

