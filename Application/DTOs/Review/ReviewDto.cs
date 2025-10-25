using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Review
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int MerchandiseId { get; set; }
        public int Rating { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new();
        public bool IsVerifiedPurchase { get; set; }
        public int HelpfulCount { get; set; }
        public int UnhelpfulCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ReviewResponseDto> Responses { get; set; } = new();
        public bool? UserVote { get; set; } // null, true (helpful), false (unhelpful)
    }

    public class CreateReviewDto
    {
        public int MerchandiseId { get; set; }
        public int Rating { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new();
    }

    public class UpdateReviewDto
    {
        public int Rating { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new();
    }

    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsSellerResponse { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateReviewResponseDto
    {
        public string Content { get; set; } = string.Empty;
    }

    public class ReviewStatsDto
    {
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new();
        public int VerifiedPurchaseCount { get; set; }
        public double VerifiedPurchasePercentage { get; set; }
    }

    public class ReviewVoteDto
    {
        public bool IsHelpful { get; set; }
    }
}